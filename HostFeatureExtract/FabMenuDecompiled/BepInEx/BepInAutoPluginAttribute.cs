using System;
using System.Diagnostics;

namespace BepInEx;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[Conditional("CodeGeneration")]
internal sealed class BepInAutoPluginAttribute : Attribute
{
	public BepInAutoPluginAttribute(string id = null, string name = null, string version = null)
	{
	}
}
