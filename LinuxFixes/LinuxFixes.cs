using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using ResoniteModLoader;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LinuxFixes
{
	[HarmonyPatch]
	class LinuxFixesMod : ResoniteMod	{
		public override string Name => BuildInfo.Name;
		public override string Author => BuildInfo.Author;
		public override string Version => BuildInfo.Version;
		public override string Link => BuildInfo.Link;

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> ReverseScroll = new("ReverseScroll", "If to reverse the scroll direction (requires game restart)", () => false);
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<CursorLockMode?> SetCursorLockMode = new("SetCursorLockMode", "Which cursor lock mode to enforce", () => null);
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> WarpMouseToCenter = new("WarpMouseToCenter", "If to warp mouse to center on context menu open", () => true);
		private static ModConfiguration Config;

		private static CursorLockMode? SetCursorLockTo;

		public override void OnEngineInit()
		{
			try
			{
				Config = GetConfiguration();
				Config.Save(true);
				SetCursorLockTo = Config.GetValue(SetCursorLockMode);
				Config.OnThisConfigurationChanged += delegate { SetCursorLockTo = Config.GetValue(SetCursorLockMode); };
				//DisplayPointer = XOpenDisplay(null);
				Harmony harmony = new(BuildInfo.GUID);
				harmony.PatchAll();
				Msg("Patched successfully");
			}
			catch (Exception ex)
			{
				Error(ex);
			}
		}

		[HarmonyPatch(typeof(MouseDriver), nameof(MouseDriver.UpdateMouse))]
		[HarmonyPostfix]
		private static void OverwriteLockState()
		{
			var lockMode = SetCursorLockTo;
			if (lockMode.HasValue)
			{
				Cursor.lockState = lockMode.Value;
			}
		}

		[HarmonyPatch(typeof(FrooxEngine.ContextMenu), nameof(FrooxEngine.ContextMenu.OpenMenu))]
		[HarmonyPostfix]
		private static void CenterMouseOnContextMenu()
		{
			if (!Config.GetValue(WarpMouseToCenter)) return;
			Debug($"Warping mouse to the center");
			Cursor.lockState = CursorLockMode.None;
			Mouse.current.WarpCursorPosition(
				new Vector2(Screen.width / 2, Screen.height / 2)
			);
		}

		[HarmonyPatch(typeof(MouseDriver), nameof(MouseDriver.UpdateMouse))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> FixReverseScrollwheel(IEnumerable<CodeInstruction> instructions)
		{
			var found = false;
			foreach (var instruction in instructions)
			{
				if (instruction.Is(OpCodes.Ldc_I4_S, (sbyte)100))
				{
					instruction.operand = (sbyte)instruction.operand * (Config.GetValue(ReverseScroll) ? -1 : 1);
					found = true;
				}
				yield return instruction;
			}
			if (found is false)
				throw new Exception("Cannot find scrollwheel multiplier in MouseDriver");
		}
	}
}
