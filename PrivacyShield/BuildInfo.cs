using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(PrivacyShield.BuildInfo.Name)]
[assembly: AssemblyProduct(PrivacyShield.BuildInfo.GUID)]
[assembly: AssemblyVersion(PrivacyShield.BuildInfo.Version)]
[assembly: AssemblyCompany("neos.ljoonal.xyz")]

namespace PrivacyShield
{
	public static class BuildInfo
	{
		public const string Version = "0.3.0";

		public const string Name = "Privacy Shield";

		public const string Author = "ljoonal";

		public const string Link = "https://neos.ljoonal.xyz/mods";

		public const string GUID = "xyz.ljoonal.neos.privacyshield";
	}
}
