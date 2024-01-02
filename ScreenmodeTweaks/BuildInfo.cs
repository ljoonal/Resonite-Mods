using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(ScreenmodeTweaks.BuildInfo.Name)]
[assembly: AssemblyProduct(ScreenmodeTweaks.BuildInfo.GUID)]
[assembly: AssemblyVersion(ScreenmodeTweaks.BuildInfo.Version)]
[assembly: AssemblyCompany("com.munally.lj")]
namespace ScreenmodeTweaks
{
	public static class BuildInfo
	{
		public const string Version = "1.0.0";

		public const string Name = "Screenmode tweaks";

		public const string Author = "LJ";

		public const string Link = "https://lj.munally.com/resonite/mods";

		public const string GUID = "com.munally.lj.screenmodetweaks";
	}
}
