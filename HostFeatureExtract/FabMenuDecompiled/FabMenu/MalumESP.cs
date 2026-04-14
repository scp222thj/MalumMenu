using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Sentry.Internal.Extensions;
using TMPro;
using UnityEngine;

namespace FabMenu;

public static class MalumESP
{
	public static bool freecamActive;

	public static bool resolutionchangeNeeded;

	public static void sporeCloudVision(Mushroom mushroom)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.fullBright)
		{
			mushroom.sporeMask.transform.position = new Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, -1f);
		}
		else
		{
			mushroom.sporeMask.transform.position = new Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, 5f);
		}
	}

	public static bool fullBrightActive()
	{
		if (!CheatToggles.fullBright && !(Camera.main.orthographicSize > 3f))
		{
			return (Object)(object)((Component)Camera.main).gameObject.GetComponent<FollowerCamera>().Target != (Object)(object)PlayerControl.LocalPlayer;
		}
		return true;
	}

	public static void zoomOut(HudManager hudManager)
	{
		if (CheatToggles.zoomOut)
		{
			if (!hudManager.Chat.IsOpenOrOpening && !Object.op_Implicit((Object)(object)PlayerCustomizationMenu.Instance) && (!Utils.isLobby || (!FriendsListUI.Instance.IsOpen && !((Component)DestroyableSingleton<GameStartManager>.Instance.LobbyInfoPane.LobbyViewSettingsPane).gameObject.active && !Object.op_Implicit((Object)(object)DestroyableSingleton<GameStartManager>.Instance.RulesEditPanel))))
			{
				resolutionchangeNeeded = true;
				if (Input.GetAxis("Mouse ScrollWheel") < 0f)
				{
					Camera main = Camera.main;
					float orthographicSize = main.orthographicSize;
					main.orthographicSize = orthographicSize + 1f;
					Camera uICamera = hudManager.UICamera;
					orthographicSize = uICamera.orthographicSize;
					uICamera.orthographicSize = orthographicSize + 1f;
					Utils.adjustResolution();
				}
				else if (Input.GetAxis("Mouse ScrollWheel") > 0f && Camera.main.orthographicSize > 3f)
				{
					Camera main2 = Camera.main;
					float orthographicSize = main2.orthographicSize;
					main2.orthographicSize = orthographicSize - 1f;
					Camera uICamera2 = hudManager.UICamera;
					orthographicSize = uICamera2.orthographicSize;
					uICamera2.orthographicSize = orthographicSize - 1f;
					Utils.adjustResolution();
				}
			}
		}
		else
		{
			Camera.main.orthographicSize = 3f;
			hudManager.UICamera.orthographicSize = 3f;
			if (resolutionchangeNeeded)
			{
				Utils.adjustResolution();
				resolutionchangeNeeded = false;
			}
		}
	}

	public static void MeetingNametags(MeetingHud meetingHud)
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (PlayerVoteArea item in (Il2CppArrayBase<PlayerVoteArea>)(object)meetingHud.playerStates)
			{
				NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(item.TargetPlayerId);
				if (!MiscExtensions.IsNull((Object)(object)playerById) && !playerById.Disconnected && !MiscExtensions.IsNull((Object)(object)playerById.Outfits[(PlayerOutfitType)0]))
				{
					((TMP_Text)item.NameText).text = Utils.GetNameTag(playerById, playerById.DefaultOutfit.PlayerName);
					if (CheatToggles.seeRoles && CheatToggles.showPlayerInfo)
					{
						item.NameText.transform.localPosition = new Vector3(0.33f, 0.08f, 0f);
						item.NameText.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
					}
					else if (CheatToggles.seeRoles || CheatToggles.showPlayerInfo)
					{
						item.NameText.transform.localPosition = new Vector3(0.3384f, 0.1125f, -0.1f);
						item.NameText.transform.localScale = new Vector3(0.9f, 1f, 1f);
					}
					else
					{
						item.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
						item.NameText.transform.localScale = new Vector3(0.9f, 1f, 1f);
					}
				}
			}
		}
		catch
		{
		}
	}

	public static void PlayerNametags(PlayerPhysics playerPhysics)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			playerPhysics.myPlayer.cosmetics.SetName(Utils.GetNameTag(playerPhysics.myPlayer.Data, playerPhysics.myPlayer.CurrentOutfit.PlayerName));
			if (CheatToggles.seeRoles && CheatToggles.showPlayerInfo)
			{
				playerPhysics.myPlayer.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0.186f, 0f);
			}
			else if (CheatToggles.seeRoles || CheatToggles.showPlayerInfo)
			{
				playerPhysics.myPlayer.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0.093f, 0f);
			}
			else
			{
				playerPhysics.myPlayer.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
		}
		catch
		{
		}
	}

	public static void ChatNametags(ChatBubble chatBubble)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			((TMP_Text)chatBubble.NameText).text = Utils.GetNameTag(chatBubble.playerInfo, ((TMP_Text)chatBubble.NameText).text, isChat: true);
			((TMP_Text)chatBubble.NameText).ForceMeshUpdate(true, true);
			chatBubble.Background.size = new Vector2(5.52f, 0.2f + TextMeshProExtensions.GetNotDumbRenderedHeight(chatBubble.NameText) + TextMeshProExtensions.GetNotDumbRenderedHeight(chatBubble.TextArea));
			chatBubble.MaskArea.size = chatBubble.Background.size - new Vector2(0f, 0.03f);
		}
		catch
		{
		}
	}

	public static void seeGhostsCheat(PlayerPhysics playerPhysics)
	{
		try
		{
			if (playerPhysics.myPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsDead)
			{
				playerPhysics.myPlayer.Visible = CheatToggles.seeGhosts;
			}
		}
		catch
		{
		}
	}

	public static void freecamCheat()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.freecam)
		{
			if (!freecamActive)
			{
				((Behaviour)((Component)Camera.main).gameObject.GetComponent<FollowerCamera>()).enabled = false;
				((Component)Camera.main).gameObject.GetComponent<FollowerCamera>().Target = null;
				freecamActive = true;
			}
			PlayerControl.LocalPlayer.moveable = false;
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
			((Component)Camera.main).transform.position = ((Component)Camera.main).transform.position + val * 10f * Time.deltaTime;
		}
		else if (freecamActive)
		{
			PlayerControl.LocalPlayer.moveable = true;
			((Behaviour)((Component)Camera.main).gameObject.GetComponent<FollowerCamera>()).enabled = true;
			((Component)Camera.main).gameObject.GetComponent<FollowerCamera>().SetTarget((MonoBehaviour)(object)PlayerControl.LocalPlayer);
			freecamActive = false;
		}
	}
}
