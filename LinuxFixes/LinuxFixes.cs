using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using NeosModLoader;
using UnityEngine;

namespace LinuxFixes
{
	[HarmonyPatch]
	class LinuxFixesMod : NeosMod
	{
		public override string Name => BuildInfo.Name;
		public override string Author => BuildInfo.Author;
		public override string Version => BuildInfo.Version;
		public override string Link => BuildInfo.Link;

		public override void OnEngineInit()
		{
			try
			{
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
			Cursor.lockState = CursorLockMode.Locked;
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
					instruction.operand = (sbyte)instruction.operand * -1;
					found = true;
				}
				yield return instruction;
			}
			if (found is false)
				throw new Exception("Cannot find scrollwheel multiplier in MouseDriver");
		}
	}
}
