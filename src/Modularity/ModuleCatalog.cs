namespace Modularity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.ComponentModel.Composition.Primitives;
	using System.ComponentModel.Composition.ReflectionModel;

	public class ModuleCatalog : ComposablePartCatalog
	{
		ComposablePartCatalog _inner_catalog;
		IEnumerable<ModuleInfo> _modules;
		bool _need_to_refresh_parts = false;
        IQueryable<ComposablePartDefinition> _parts = Enumerable.Empty<ComposablePartDefinition>().AsQueryable();
		HashSet<string> _loaded_modules = new HashSet<string>();

		private ModuleCatalog()
		{
		}

		public ModuleCatalog(ComposablePartCatalog inner_catalog)
		{
			_inner_catalog = inner_catalog;

			RefreshModules();
			RefreshParts();
		}

		public override IQueryable<ComposablePartDefinition> Parts
		{
			get
			{
				if (_need_to_refresh_parts)
				{
					RefreshParts();
					_need_to_refresh_parts = false;
				}

				return _parts;
			}
		}

		public IEnumerable<IModuleInfo> Modules
		{
			get
			{
				return _modules.Cast<IModuleInfo>();
			}
		}

		void RefreshModules()
		{
			var constraint = new ImportDefinition(d => true, typeof(IModule).FullName,
				ImportCardinality.ZeroOrMore, true, true);

			var exports = _inner_catalog.GetExports(constraint);

			_modules = exports.Select(
				t => 
					{
						var moduleinfo = new ModuleInfo
							{
								Name = t.Item2.Metadata.ContainsKey("Name") ? t.Item2.Metadata["Name"].ToString() : "",
								ModuleType = ReflectionModelServices.GetPartType(t.Item1).Value
							};

						moduleinfo.Loaded += Module_Loaded;

						return moduleinfo;
					});
		}

		void Module_Loaded(IModuleInfo module)
		{
			_loaded_modules.Add(module.Name);

			_need_to_refresh_parts = true;
		}

		void RefreshParts()
		{
			var parts_found = Enumerable.Empty<ComposablePartDefinition>();

			foreach (var module in Modules)
			{
				if (!_loaded_modules.Contains(module.Name)) continue;

				var base_namespace = module.ModuleType.Namespace;

				var parts_found_for_namespace = _inner_catalog.Parts
					.Where(d => IsTypeInNamespaceTree(ReflectionModelServices.GetPartType(d).Value, base_namespace));

				parts_found = parts_found.Concat(parts_found_for_namespace);
			}

			// include all the modules from the catalog
			var modules_found = _inner_catalog.GetExports(
				new ImportDefinition(e => true, typeof(IModule).FullName, ImportCardinality.ZeroOrMore, true, true)); 
			parts_found = parts_found.Concat(modules_found.Select(t => t.Item1));

			_parts = parts_found.Distinct().ToArray().AsQueryable();
		}

		public void Load(Predicate<IModuleInfo> criteria)
		{
			foreach (var module in Modules)
			{
				if (criteria(module))
					module.Load();
			}
		}

		static bool IsTypeInNamespaceTree(Type type, string namespace_root)
		{
			return type.Namespace == namespace_root ||
				type.Namespace.StartsWith(namespace_root + ".");
		}
	}
}
