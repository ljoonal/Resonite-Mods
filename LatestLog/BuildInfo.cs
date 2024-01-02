using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(LatestLog.BuildInfo.Name)]
[assembly: AssemblyProduct(LatestLog.BuildInfo.GUID)]
[assembly: AssemblyVersion(LatestLog.BuildInfo.Version)]
[assembly: AssemblyCompany("com.munally.lj")]
namespace LatestLog
{
	public static class BuildInfo
	{
		public const string Version = "1.0.0";

		public const string Name = "Latest Log";

		public const string Author = "LJ";

		public const string Link = "https://lj.munally.com/resonite/mods";

		public const string GUID = "com.munally.lj.latestlog";
	}
}
