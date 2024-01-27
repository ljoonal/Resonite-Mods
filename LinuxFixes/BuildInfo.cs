using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(LinuxFixes.BuildInfo.Name)]
[assembly: AssemblyProduct(LinuxFixes.BuildInfo.GUID)]
[assembly: AssemblyVersion(LinuxFixes.BuildInfo.Version)]
[assembly: AssemblyCompany("com.munally.lj")]
namespace LinuxFixes
{
	public static class BuildInfo
	{
		public const string Version = "1.2.0";

		public const string Name = "Linux Fixes";

		public const string Author = "LJ";

		public const string Link = "https://lj.munally.com/resonite/mods";

		public const string GUID = "com.munally.lj.linuxfixes";
	}
}
