namespace Modularity.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using System.ComponentModel.Composition.ReflectionModel;
	using Xunit;

	public class ModuleCatalogFacts : IDisposable
	{
		ComposablePartCatalog _inner_catalog;
		ModuleCatalog _catalog;

		public ModuleCatalogFacts()
		{
		}

		public void setup_two_modules_two_parts()
		{
			_inner_catalog = new TypeCatalog(
				typeof(TestModules.Test1.Test1Module), typeof(TestModules.Test1.Test1Part),
				typeof(TestModules.Test2.Test2Module), typeof(TestModules.Test2.Test2Part));
			_catalog = new ModuleCatalog(_inner_catalog);
		}

		public void Dispose()
		{
			if (_inner_catalog != null)
				_inner_catalog.Dispose();

			if (_catalog != null)
				_catalog.Dispose();
		}

		[Fact]
		public void can_create_with_inner_catalog()
		{
			setup_two_modules_two_parts();
		}

		[Fact]
		public void can_get_modules_from_catalog()
		{
			setup_two_modules_two_parts();

			Assert.Equal(2, _catalog.Modules.Count());
			Assert.True(_catalog.Modules.Any(m => m.Name == "Test1"));
			Assert.True(_catalog.Modules.Any(m => m.Name == "Test2"));
		}

		[Fact]
		public void parts_list_only_contains_modules_when_no_modules_loaded()
		{
			setup_two_modules_two_parts();

			var exports = _catalog.GetExports(
				new ImportDefinition(d => true, typeof(IModule).FullName,
					ImportCardinality.ZeroOrMore, true, true));

			Assert.Equal(2, _catalog.Parts.Count());
			Assert.Equal(2, exports.Count());
		}

		[Fact]
		public void parts_list_contains_new_part_after_module_loaded()
		{
			setup_two_modules_two_parts();

			var part1_found_before = _catalog.Parts.Where(d => d.ExportDefinitions.Any(e => e.ContractName == typeof(TestModules.Test1.Test1Part).FullName)).ToArray();
			var part2_found_before = _catalog.Parts.Where(d => d.ExportDefinitions.Any(e => e.ContractName == typeof(TestModules.Test2.Test2Part).FullName)).ToArray();
			_catalog.Load(m => m.Name == "Test1");
			var part1_found_after = _catalog.Parts.Where(d => d.ExportDefinitions.Any(e => e.ContractName == typeof(TestModules.Test1.Test1Part).FullName)).ToArray();
			var part2_found_after = _catalog.Parts.Where(d => d.ExportDefinitions.Any(e => e.ContractName == typeof(TestModules.Test2.Test2Part).FullName)).ToArray();

			Assert.Empty(part1_found_before);
			Assert.NotEmpty(part1_found_after);
			Assert.Empty(part2_found_before);
			Assert.Empty(part2_found_after);
		}
	}
}
