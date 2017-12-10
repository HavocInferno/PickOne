using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.VR;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();

        public Button colorButton;
        public InputField nameInput;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;

		public Toggle vrMasterToggle;
		public GameObject vrMasterIcon;
		public GameObject vrInfoIcon;
		public Toggle class1Button;
		public Toggle class2Button;
		public Toggle class3Button;
		public Toggle class4Button;

        public GameObject localIcone;
        public GameObject remoteIcone;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;
		[SyncVar(hook = "OnVRMaster")]
		public bool isVRMasterPlayer = false;
		//[SyncVar(hook = "OnHasHMD")]
		//public bool isVRcapable = false;
		[SyncVar(hook = "OnMyVRModel")]
		public int vrDeviceModel = -1; //-1 = NONE, 1 = VIVE, 2 = RIFT
		[SyncVar(hook = "OnMyClassIndex")]
		public int classIndex = -1; //-1 = NONE, x = Class x

        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f/255.0f, 0.0f, 101.0f/255.0f,1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);


        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            if (isLocalPlayer)
            {
				//check if a VR HMD is connected, if so set syncvar
				QueryVRDeviceModel();
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
            OnMyColor(playerColor);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

			QueryVRDeviceModel ();

           SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
			Debug.Log ("Setting up other lobby player; ---- Name: " + playerName + "; Color: " + playerColor.ToString() + "; VR model: " + vrDeviceModel + "; isMaster: " + isVRMasterPlayer.ToString());

            nameInput.interactable = false;
            removePlayerButton.interactable = NetworkServer.active;

            ChangeReadyButtonColor(NotReadyColor);

			//VR Master and HMD capability checks
			CheckMasterToggle ();
			CheckHMDToggle ();
			if(!isVRMasterPlayer)
				OnMyClassIndex (classIndex);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
			Debug.Log ("Setting up local lobby player");

            nameInput.interactable = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);

			//host kick-ability, VR Master and HMD capability checks
            CheckRemoveButton();
			CheckMasterToggle ();
			CheckHMDToggle ();
			EnableClassButtons ();

            if (playerColor == Color.white)
                CmdColorChange();

            ChangeReadyButtonColor(JoinColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            readyButton.interactable = true;

            //have to use child count of player prefab already setup as "this.slot" is not set yet
            if (playerName == "")
                CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount-1));

            //we switch from simple name display to name input
            colorButton.interactable = true;
            nameInput.interactable = true;

            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener(OnNameChanged);

            colorButton.onClick.RemoveAllListeners();
            colorButton.onClick.AddListener(OnColorClicked);

            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);

            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.interactable = localPlayerCount > 1;
        }

		//This enable/disable the vr master selection buttons for the host
		public void CheckMasterToggle()
		{
			if (isServer) {
				vrMasterToggle.gameObject.SetActive (true);
			}

			if (isVRMasterPlayer) {
				vrMasterIcon.SetActive (true);
			}
		}

		/* check whether the client in question is vr capable, 
		 * if so check the model and display whether it is a Rift or a Vive
		 * otherwise display "no HMD"
		*/
		public void CheckHMDToggle()
		{
			Debug.Log ("Checking HMD Toggle");
			OnMyVRModel (vrDeviceModel);
		}

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
            }
        }

        public void OnPlayerListChanged(int idx)
        { 
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

		public void QueryVRDeviceModel() {
			Debug.Log ("Querying VR Device Model");
			if (UnityEngine.XR.XRDevice.isPresent) {
				//isVRcapable = true;
			
				string model = UnityEngine.XR.XRDevice.model != null ?
				UnityEngine.XR.XRDevice.model : "";

				if (model.IndexOf ("Rift") >= 0) {
					vrDeviceModel = 2;
				} else {
					vrDeviceModel = 1;
				}
				Debug.Log ("VR headset found, model " + vrDeviceModel + "; " + model);
			} else {
				vrDeviceModel = -1;
			}

			if (isServer)
				RpcVRDetected(vrDeviceModel);
			else
				CmdVRDetected (vrDeviceModel);
		}

		[ClientRpc]
		public void RpcVRDetected(int vrmodel) {
			CmdVRDetected (vrmodel);
		}

		[Command]
		public void CmdVRDetected(int vrmodel) {
			vrDeviceModel = vrmodel;
		}

		public void EnableClassButtons() {
			if (!hasAuthority)
				return;
			
			class1Button.interactable = true;
			class2Button.interactable = true;
			class3Button.interactable = true;
			class4Button.interactable = true;

			class1Button.gameObject.SetActive (true);
			class2Button.gameObject.SetActive (true);
			class3Button.gameObject.SetActive (true);
			class4Button.gameObject.SetActive (true);

			class1Button.onValueChanged.RemoveAllListeners ();
			class1Button.onValueChanged.AddListener (delegate {ClassPicker (class1Button.name, class1Button.isOn);});
			class2Button.onValueChanged.RemoveAllListeners ();
			class2Button.onValueChanged.AddListener (delegate {ClassPicker (class2Button.name, class2Button.isOn);});
			class3Button.onValueChanged.RemoveAllListeners ();
			class3Button.onValueChanged.AddListener (delegate {ClassPicker (class3Button.name, class3Button.isOn);});
			class4Button.onValueChanged.RemoveAllListeners ();
			class4Button.onValueChanged.AddListener (delegate {ClassPicker (class4Button.name, class4Button.isOn);});
		}

		public void DisableClassButtons() {
			class1Button.interactable = false;
			class2Button.interactable = false;
			class3Button.interactable = false;
			class4Button.interactable = false;

			class1Button.gameObject.SetActive (false);
			class2Button.gameObject.SetActive (false);
			class3Button.gameObject.SetActive (false);
			class4Button.gameObject.SetActive (false);
		}

		void ClassPicker(string buttonName, bool isOn) {
			if (!isOn)
				return;
			
			switch (buttonName) {
			case "Class1Button":
				classIndex = 0;
				break;
			case "Class2Button":
				classIndex = 1;
				break;
			case "Class3Button":
				classIndex = 2;
				break;
			case "Class4Button":
				classIndex = 3;
				break;
			default:
				break;
			}

			Debug.Log ("picked new class: " + buttonName + ", #" + classIndex);

			if (isServer)
				RpcClassPicked (classIndex);
			else
				CmdClassPicked (classIndex);
		}

		//ISSUE: apparently changing class on the server has "No authority"? 
		// thus not actually changing the value on the clients permanently? 
		// however the info is still transmitted and the correct class selection is displayed...
		[ClientRpc]
		public void RpcClassPicked(int cIndex) {
			CmdClassPicked (cIndex);
		}

		[Command]
		public void CmdClassPicked(int cIndex) {
			LobbyManager.s_Singleton.SetPlayerTypeLobby (GetComponent<NetworkIdentity> ().connectionToClient, cIndex);
			classIndex = cIndex;
		}

        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            //playerName = newName;
            nameInput.text = newName;
        }

        public void OnMyColor(Color newColor)
        {
            //playerColor = newColor;
            colorButton.GetComponent<Image>().color = newColor;
        }

		//if the isVRMasterPlayer syncvar is changed, enable the respective icon on that lobbyplayer's UI piece
		public void OnVRMaster(bool newState)
		{
			//isVRMasterPlayer = newState;
			//vrMasterToggle.isOn = newState;
			vrMasterIcon.SetActive (newState);

			if (newState == true) {
				class1Button.gameObject.SetActive (false);
				class2Button.gameObject.SetActive (false);
				class3Button.gameObject.SetActive (false);
				class4Button.gameObject.SetActive (false);
			} else {
				if (isLocalPlayer) {
					class1Button.gameObject.SetActive (true);
					class2Button.gameObject.SetActive (true);
					class3Button.gameObject.SetActive (true);
					class4Button.gameObject.SetActive (true);
				} else {
					Debug.Log ("class index of otherplayer is " + classIndex);
					OnMyClassIndex (classIndex);
				}
			}
		}

		/* display whether the client in question is vr capable, 
		 * and whether it is a Rift or a Vive or "no HMD"
		*/
		public void OnMyVRModel(int model) {
			Debug.Log ("OnMyVRModel called with " + model);
			vrInfoIcon.SetActive (true);
			switch (model) {
			case -1:
				vrInfoIcon.GetComponentInChildren<Text> ().text = "no HMD";
				break;
			case 1: 
				vrInfoIcon.GetComponentInChildren<Text> ().text = "Vive";
				break;
			case 2:
				vrInfoIcon.GetComponentInChildren<Text> ().text = "Rift";
				break;
			default:
				break;
			}
		}

		public void OnMyClassIndex(int cIndex) {
			if (!isLocalPlayer) {
				class1Button.gameObject.SetActive (false);
				class2Button.gameObject.SetActive (false);
				class3Button.gameObject.SetActive (false);
				class4Button.gameObject.SetActive (false);
			Toggle selectedButton = null;
				switch (cIndex) {
			case 0:
				selectedButton = class1Button;
					break;
			case 1:
				selectedButton = class2Button;
					break;
			case 2:
				selectedButton = class3Button;
					break;
			case 3:
				selectedButton = class4Button;
					break;
				default:
					break;
				}

				if(selectedButton != null)
					selectedButton.gameObject.SetActive (true);
			}
		}

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnColorClicked()
        {
            CmdColorChange();
        }

		/* if one of the vr master toggles is clicked, 
		 * set all toggles to not-checked, 
		 * then set the clicked toggle to checked 
		 * and update the isVRMasterPlayer property of each player accordingly */
		public void OnVRMasterClick(LobbyPlayer target)
		{
			if (isServer) {
				foreach (LobbyPlayer lp in FindObjectsOfType(typeof(LobbyPlayer))) {
					lp.isVRMasterPlayer = false;
				}

				target.isVRMasterPlayer = true;
				Debug.Log ("new Master is " + target.playerName + "; c " + target.playerColor + "; id " + target.playerControllerId);
			}
		}

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }

        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);
                
        }

        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        [Command]
        public void CmdColorChange()
        {
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % Colors.Length;

            bool alreadyInUse = false;

            do
            {
                alreadyInUse = false;
                for (int i = 0; i < _colorInUse.Count; ++i)
                {
                    if (_colorInUse[i] == idx)
                    {//that color is already in use
                        alreadyInUse = true;
                        idx = (idx + 1) % Colors.Length;
                    }
                }
            }
            while (alreadyInUse);

            if (inUseIdx >= 0)
            {//if we already add an entry in the colorTabs, we change it
                _colorInUse[inUseIdx] = idx;
            }
            else
            {//else we add it
                _colorInUse.Add(idx);
            }

            playerColor = Colors[idx];
        }

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx < 0)
                return;

            for (int i = 0; i < _colorInUse.Count; ++i)
            {
                if (_colorInUse[i] == idx)
                {//that color is already in use
                    _colorInUse.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
