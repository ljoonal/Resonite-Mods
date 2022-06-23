using System;
using System.Threading.Tasks;
using HarmonyLib;
using NeosModLoader;

namespace PrivacyShield
{
	[HarmonyPatch]
	class PrivacyShieldMod : NeosMod
	{
		public override string Name => BuildInfo.Name;
		public override string Author => BuildInfo.Author;
		public override string Version => BuildInfo.Version;
		public override string Link => BuildInfo.Link;

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> HostPermissionsEverything = new("HostPermissionsEverything", "If the hosts permission request check should be done for all assets.", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<float> SpoofFPS = new("FpsSpoof", "The FPS to spoof to. Set to 0 to disable.", () => 0f);
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<string> TimeZoneSpoof = new("TimeZoneSpoof", "The timezone to spoof to.", () => "UTC");
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> TimeZoneSpoofEnabled = new("TimeZoneSpoofEnabled", "If the TZ spoof is enabled or not.", () => false);


		private static ModConfiguration Config;
		//private static readonly System.Random rng = new();

		public override void OnEngineInit()
		{
			Config = GetConfiguration();
			Config.Save(true);
			if (Config.GetValue(TimeZoneSpoofEnabled))
			{
				try
				{

					Traverse.Create(typeof(TimeZoneInfo)).Field("local").SetValue(TimeZoneInfo.FindSystemTimeZoneById(Config.GetValue(TimeZoneSpoof)));
				}
				catch (Exception ex)
				{
					Error(ex);
				}
			}

			try
			{
				Harmony harmony = new(BuildInfo.GUID);
				harmony.PatchAll();
			}
			catch (Exception ex)
			{
				Error(ex);
			}
		}


		[HarmonyPatch(typeof(FrooxEngine.World), "RefreshStep")]
		[HarmonyPostfix]
		private static void PatchFPS(ref FrooxEngine.World __instance)
		{
			var targetFPS = Config.GetValue(SpoofFPS);
			// Short circuit with monkee values.
			// This is meant for privacy, not for "oh look number go brr"
			if (targetFPS <= 10 || targetFPS >= 144) return;
			//double random = rng.NextDouble() * config.GetValue(VarianceFPS);
			__instance.LocalUser.FPS = targetFPS;
		}


		[HarmonyPatch(typeof(FrooxEngine.AssetManager), nameof(FrooxEngine.AssetManager.RequestGather))]
		[HarmonyPrefix]
		private static bool PatchRequester(
			FrooxEngine.AssetManager __instance,
			FrooxEngine.AssetGatherer ___assetGatherer,
			ref ValueTask<string> __result,
			Uri __0,
			FrooxEngine.Priority __1,
			CloudX.Shared.NeosDB_Endpoint? __2
		)
		{
			if (!Config.GetValue(HostPermissionsEverything)) return true;
			__result = HandleRequest(__instance, ___assetGatherer, __0, __1, __2);
			return false;
		}

		private static async ValueTask<string> HandleRequest(
			FrooxEngine.AssetManager assetManager,
				FrooxEngine.AssetGatherer assetGatherer,
				Uri uri,
				FrooxEngine.Priority priority,
				CloudX.Shared.NeosDB_Endpoint? endpointOverwrite
		)
		{
			if (uri.Scheme == "neosdb" || uri.Scheme == "local" || uri.Host.EndsWith(".neos.com") || await AskForPermission(assetManager.Engine, uri, "PrivacyShield generic request"))
			{
				return await assetGatherer.Gather(uri, priority, endpointOverwrite);
			}
			else
			{
				throw new Exception("No permissions to load asset");
			}
		}


		private static async Task<bool> AskForPermission(FrooxEngine.Engine engine, Uri target, String accessReason)
		{
			Debug("Asking permissions for", target);
			FrooxEngine.HostAccessPermission perms = await
				engine.Security.RequestAccessPermission(target.Host, target.Port,
				accessReason);
			return perms == FrooxEngine.HostAccessPermission.Allowed;
		}
	}
}
