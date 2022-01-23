using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(ScreenmodeTweaks.BuildInfo.Name)]
[assembly: AssemblyProduct(ScreenmodeTweaks.BuildInfo.GUID)]
[assembly: AssemblyVersion(ScreenmodeTweaks.BuildInfo.Version)]
[assembly: AssemblyCompany("neos.ljoonal.xyz")]

namespace ScreenmodeTweaks
{
	public static class BuildInfo
	{
		public const string Version = "0.0.2";

		public const string Name = "Screenmode tweaks";

		public const string Author = "ljoonal";

		public const string Link = "https://neos.ljoonal.xyz/mods";

		public const string GUID = "xyz.ljoonal.neos.screenmodetweaks";
	}
}
