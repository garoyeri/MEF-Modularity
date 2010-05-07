namespace Modularity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class ModuleInfo : IModuleInfo
	{
		public string Name { get; set; }
		public Type ModuleType { get; set; }

		public void Load()
		{
			var loaded_callbacks = this.Loaded;
			if (loaded_callbacks != null)
				loaded_callbacks(this);
		}

		public event Action<IModuleInfo> Loaded;
	}
}
