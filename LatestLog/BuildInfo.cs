using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(LatestLog.BuildInfo.Name)]
[assembly: AssemblyProduct(LatestLog.BuildInfo.GUID)]
[assembly: AssemblyVersion(LatestLog.BuildInfo.Version)]
[assembly: AssemblyCompany("neos.ljoonal.xyz")]

namespace LatestLog
{
	public static class BuildInfo
	{
		public const string Version = "0.0.1";

		public const string Name = "Latest Log";

		public const string Author = "ljoonal";

		public const string Link = "https://neos.ljoonal.xyz/mods";

		public const string GUID = "xyz.ljoonal.neos.latestlog";
	}
}
