using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrooxEngine;
using HarmonyLib;
using LeapInternal;
using MonkeyLoader.Logging;
using MonkeyLoader.Patching;
using MonkeyLoader.Resonite;
using Newtonsoft.Json;
using Tmds.DBus.Protocol;
using UnityEngine;
using Connection = Tmds.DBus.Protocol.Connection;

namespace PenguNotifs
{

	[HarmonyPatch]
	class PenguNotifsMoonkey : ResoniteMonkey<PenguNotifsMoonkey>
	{
		const string path = "/org/freedesktop/Notifications";
		const string iface = "org.freedesktop.Notifications";
		const string member = "Notify";

		protected override bool OnEngineInit()
		{
			try
			{
				var rule = new MatchRule
				{
					Type = Tmds.DBus.Protocol.MessageType.MethodCall,
					Path = path,
					Member = member,
					Interface = iface
				};

				Task.Run(async () =>
				{
					var conn = new Connection(Address.Session);
					await conn.ConnectAsync();

					await conn.AddMatchAsync(rule, (msg, state) =>
					{
						return (Type: msg.MessageType, IFace: msg.InterfaceAsString, Member: msg.MemberAsString);
					}, (e, msg, _, _) =>
					{
						Notify(JsonConvert.SerializeObject(msg));
					});

					await BecomeMonitor(conn);
				}).Wait();

				Logger.Info(() => "Started successfully");
			}
			catch (Exception ex)
			{
				Logger.Error(() => ex.ToString());
				return false;
			}
			return true;
		}

		protected Task BecomeMonitor(Connection conn)
		{
			MessageBuffer msg;
			using (var writer = conn.GetMessageWriter())
			{
				writer.WriteMethodCallHeader("org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus.Monitoring", "BecomeMonitor", "asu");
				writer.WriteArray(new string[0]);
				writer.WriteUInt32(0);

				msg = writer.CreateMessage();
			}


			return conn.CallMethodAsync(msg);
		}

		private void Notify(string json)
		{
			var args = new object[] { "U-Resonite", json, null, null };
			Traverse.Create(NotificationPanel.Current)
				.Method("AddNotification", new Type[] { typeof(string), typeof(string), typeof(Uri), typeof(IAssetProvider<AudioClip>) })
				.GetValue(args);
		}

		protected override IEnumerable<IFeaturePatch> GetFeaturePatches()
		{
			return Array.Empty<IFeaturePatch>();
		}
	}
}
