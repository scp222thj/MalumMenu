using System;
using System.Diagnostics;

namespace BepInEx.Preloader.Core.Patching;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[Conditional("CodeGeneration")]
internal sealed class PatcherAutoPluginAttribute : Attribute
{
	public PatcherAutoPluginAttribute(string id = null, string name = null, string version = null)
	{
	}
}
