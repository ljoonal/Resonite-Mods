using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Elements.Core;
using FrooxEngine;
using HarmonyLib;
using MonkeyLoader.Configuration;
using MonkeyLoader.Patching;
using MonkeyLoader.Resonite;
using SkyFrost.Base;

namespace PrivacyShield
{

	public class PrivacyShieldConfig : ConfigSection
	{
		public override string Description => Assembly.GetExecutingAssembly().GetName().FullName;
		public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		protected override IncompatibleConfigHandling IncompatibilityHandling => IncompatibleConfigHandling.ForceLoad;

		public override string Id => this.GetType().Name;

		public readonly DefiningConfigKey<bool> HostPermissionsEverything = new("HostPermissionsEverything", "If the hosts permission request check should be done for all assets.", () => true);
		public readonly DefiningConfigKey<float> SpoofFPS = new("FpsSpoof", "The FPS to spoof to. Set to 0 to disable.", () => 0f);
		public readonly DefiningConfigKey<string> TimeZoneSpoof = new("TimeZoneSpoof", "The timezone to spoof to.", () => "UTC");
		public readonly DefiningConfigKey<bool> TimeZoneSpoofEnabled = new("TimeZoneSpoofEnabled", "If the TZ spoof is enabled or not.", () => false);

		public PrivacyShieldConfig() { }
	}


	[HarmonyPatch]
	class PrivacyShieldMonkey : ConfiguredResoniteMonkey<PrivacyShieldMonkey, PrivacyShieldConfig>
	{

		protected override bool OnEngineInit()
		{
			if (ConfigSection.TimeZoneSpoofEnabled.GetValue())
			{
				try
				{
					Traverse.Create(typeof(TimeZoneInfo)).Field("local").SetValue(TimeZoneInfo.FindSystemTimeZoneById(ConfigSection.TimeZoneSpoof.GetValue()));
				}
				catch (Exception ex)
				{
					Logger.Error(() => ex.ToString());
					return false;
				}
			}

			Harmony harmony = new(Assembly.GetExecutingAssembly().GetName().FullName);
			harmony.PatchAll();

			return true;
		}


		[HarmonyPatch(typeof(World), "RefreshStep")]
		[HarmonyPostfix]
		private static void PatchFPS(ref World __instance)
		{
			var targetFPS = ConfigSection.SpoofFPS.GetValue();
			// Short circuit with monkee values.
			// This is meant for privacy, not for "oh look number go brr"
			if (targetFPS <= 10 || targetFPS >= 144) return;
			//double random = rng.NextDouble() * config.GetValue(VarianceFPS);
			__instance.LocalUser.FPS = targetFPS;
		}


		[HarmonyPatch(typeof(AssetManager), nameof(AssetManager.GatherAsset))]
		[HarmonyPrefix]
		private static bool PatchRequester(
				AssetManager __instance,
				EngineAssetGatherer ___assetGatherer,
				ref ValueTask<GatherResult> __result,
				Uri __0,
				float __1,
				DB_Endpoint? __2
		)
		{
			if (!ConfigSection.HostPermissionsEverything.GetValue()) return true;
			__result = HandleRequest<GatherResult>(__instance, ___assetGatherer, __0, __1, __2);
			return false;
		}

		[HarmonyPatch(typeof(AssetManager), nameof(AssetManager.GatherAssetFile))]
		[HarmonyPrefix]
		private static bool PatchRequester2(
				AssetManager __instance,
				EngineAssetGatherer ___assetGatherer,
			ref ValueTask<string> __result,
			Uri __0,
				float __1,
				DB_Endpoint? __2
		)
		{
			if (!ConfigSection.HostPermissionsEverything.GetValue()) return true;
			__result = HandleRequest<string>(__instance, ___assetGatherer, __0, __1, __2);
			return false;
		}

		private static async ValueTask<T> HandleRequest<T>(
			AssetManager assetManager,
				EngineAssetGatherer assetGatherer,
				Uri uri,
				float priority,
				DB_Endpoint? endpointOverwrite
		)
		{
			if (uri.Scheme == "resdb" || uri.Scheme == "local" || uri.Host.EndsWith(".resonite.com") || await AskForPermission(assetManager.Engine, uri, "PrivacyShield generic request"))
			{
				if (typeof(T) == typeof(string))
				{
					return (T)(object)await (await assetGatherer.Gather(uri, priority, endpointOverwrite).ConfigureAwait(false)).GetFile().ConfigureAwait(false);
				}
				else if (typeof(T) == typeof(GatherResult))
				{
					return (T)(object)await assetGatherer.Gather(uri, priority, endpointOverwrite);
				}
			}
			else
			{
				throw new Exception("No permissions to load asset");
			}
			return default;
		}

		private static async Task<bool> AskForPermission(Engine engine, Uri target, string accessReason)
		{
			Logger.Debug(() => "Asking permissions for: " + target.ToString());
			HostAccessPermission perms = await
					engine.Security.RequestAccessPermission(target.Host, target.Port, HostAccessScope.HTTP, accessReason);
			return perms == HostAccessPermission.Allowed;
		}

		protected override IEnumerable<IFeaturePatch> GetFeaturePatches()
		{
			return Array.Empty<IFeaturePatch>();
		}
	}
}
