using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class CrawlerLobbyHook : LobbyHook 
{
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
	{
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
		CrawlerController localPlayer = gamePlayer.GetComponent<CrawlerController>();

		Debug.Log ("CLH: Copying name/color to player " + localPlayer.pName);
		localPlayer.pName = lobby.playerName;
		localPlayer.playerColor = lobby.playerColor;
		localPlayer.isVRMasterPlayer = lobby.isVRMasterPlayer;
		Debug.Log ("CLH: Name/Color copied to player " + localPlayer.pName);
	}
}