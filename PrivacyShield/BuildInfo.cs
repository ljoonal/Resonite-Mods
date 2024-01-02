using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(PrivacyShield.BuildInfo.Name)]
[assembly: AssemblyProduct(PrivacyShield.BuildInfo.GUID)]
[assembly: AssemblyVersion(PrivacyShield.BuildInfo.Version)]
[assembly: AssemblyCompany("com.munally.lj")]
namespace PrivacyShield
{
	public static class BuildInfo
	{
		public const string Version = "1.0.0";

		public const string Name = "Privacy Shield";

		public const string Author = "LJ & hazre";

		public const string Link = "https://lj.munally.com/resonite/mods";

		public const string GUID = "com.munally.lj.privacyshield";
	}
}
