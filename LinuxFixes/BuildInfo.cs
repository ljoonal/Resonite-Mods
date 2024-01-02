using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(PenguNotifs.BuildInfo.Name)]
[assembly: AssemblyProduct(PenguNotifs.BuildInfo.GUID)]
[assembly: AssemblyVersion(PenguNotifs.BuildInfo.Version)]
[assembly: AssemblyCompany("com.munally.lj")]
namespace PenguNotifs
{
	public static class BuildInfo
	{
		public const string Version = "1.0.0";

		public const string Name = "Linux Fixes";

		public const string Author = "LJ";

		public const string Link = "https://lj.munally.com/resonite/mods";

		public const string GUID = "com.munally.lj.linuxfixes";
	}
}
