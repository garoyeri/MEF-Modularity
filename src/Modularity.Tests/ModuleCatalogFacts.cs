﻿namespace Modularity.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using Xunit;

	public class ModuleCatalogFacts : IDisposable
	{
		ComposablePartCatalog _inner_catalog;
		ModuleCatalog _catalog;

		public ModuleCatalogFacts()
		{
		}

		public void SetupScenario1()
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
			SetupScenario1();
		}

		[Fact]
		public void can_get_modules_from_catalog()
		{
			SetupScenario1();

			Assert.Equal(2, _catalog.Modules.Count());
			Assert.True(_catalog.Modules.Any(m => m.Name == "Test1"));
			Assert.True(_catalog.Modules.Any(m => m.Name == "Test2"));
		}

		[Fact]
		public void parts_list_only_contains_modules_when_no_modules_loaded()
		{
			SetupScenario1();

			var exports = _catalog.GetExports(
				new ImportDefinition(d => true, typeof(IModule).FullName,
					ImportCardinality.ZeroOrMore, true, true));

			Assert.Equal(2, _catalog.Parts.Count());
			Assert.Equal(2, exports.Count());
		}
	}
}
