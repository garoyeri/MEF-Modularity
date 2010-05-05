namespace Modularity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.ComponentModel.Composition;

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
	public class ModuleAttribute : ExportAttribute
	{
		public ModuleAttribute()
			: base(typeof(IModule))
		{
		}

		public string Name { get; set; }
	}
}
