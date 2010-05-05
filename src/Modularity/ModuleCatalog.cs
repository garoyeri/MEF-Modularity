﻿namespace Modularity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.ComponentModel.Composition.Primitives;

	public class ModuleCatalog : ComposablePartCatalog
	{
		ComposablePartCatalog _inner_catalog;
		IEnumerable<IModuleInfo> _modules;

		private ModuleCatalog()
		{
		}

		public ModuleCatalog(ComposablePartCatalog inner_catalog)
		{
			_inner_catalog = inner_catalog;

			RefreshModules();
		}

		public override IQueryable<ComposablePartDefinition> Parts
		{
			get
			{
				return _inner_catalog.Parts;
			}
		}

		public IEnumerable<IModuleInfo> Modules
		{
			get
			{
				return _modules;
			}
		}

		void RefreshModules()
		{
			var constraint = new ImportDefinition(d => true, typeof(IModule).FullName,
				ImportCardinality.ZeroOrMore, true, true);

			var exports = _inner_catalog.GetExports(constraint);

			_modules = exports.Select(
				t => new ModuleInfo
				{
					Name = t.Item2.Metadata.ContainsKey("Name") ? t.Item2.Metadata["Name"].ToString() : ""
				}).Cast<IModuleInfo>();
		}
	}
}
