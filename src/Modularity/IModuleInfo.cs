namespace Modularity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public interface IModuleInfo
	{
		string Name { get; }
		Type ModuleType { get; }

		void Load();
	}
}
