using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class CrawlerLobbyHook : LobbyHook 
{
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
	{
        // is called by lobby once scene is loaded on clients, 
        // used to hook lobby player to client player(prefabs) and transfer information

		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
		Crawler localPlayer = gamePlayer.GetComponent<Crawler>();

		Debug.Log ("CLH: Copying name/color to player " + localPlayer.pName);
		localPlayer.pName = lobby.playerName;
		localPlayer.playerColor = lobby.playerColor;
		localPlayer.isVRMasterPlayer = lobby.isVRMasterPlayer;
		Debug.Log ("CLH: Name/Color copied to player " + localPlayer.pName);
	}
}