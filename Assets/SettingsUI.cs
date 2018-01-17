using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsUI : MonoBehaviour {

	public Button applyButton;
	public Button backButton;

	//visual controls
	public Toggle fullScreenToggle;
	public Dropdown resolutionDropdown;
	public Dropdown presetDropdown;
	public Dropdown textureQualityDropdown;
	public Dropdown antialiasingDropdown;
	public Dropdown vSyncDropdown;

	//audio controls
	public AudioSource musicSource;
	public Slider musicVolumeSlider;

	//game controls
	public Toggle showVRTutorialToggle;


	//---------------------------------------------------------

	public Resolution[] resolutions;

	public bool dirtySetting = false;
	public GameSettingsContainer gameSettings;

	void OnEnable() {
		gameSettings = new GameSettingsContainer ();

		applyButton.onClick.AddListener(delegate {	OnApplyButtonClick ();	});
		backButton.onClick.AddListener(delegate {	OnBackButtonClick ();	});

		fullScreenToggle.onValueChanged.AddListener (delegate {	OnFullScreenToggle ();	});
		resolutionDropdown.onValueChanged.AddListener (delegate {	OnResolutionChange ();	});
		presetDropdown.onValueChanged.AddListener (delegate {	OnPresetChange ();	});
		//textureQualityDropdown.onValueChanged.AddListener (delegate {	OnTextureQualityChange ();	});
		//antialiasingDropdown.onValueChanged.AddListener (delegate {	OnAntialiasingChange ();	});
		vSyncDropdown.onValueChanged.AddListener (delegate {	OnVSyncChange ();	});
		musicVolumeSlider.onValueChanged.AddListener (delegate {	OnMusicVolumeChange ();	});
		showVRTutorialToggle.onValueChanged.AddListener (delegate {	OnShowVRTutorialToggle ();	});

		resolutions = Screen.resolutions;
		resolutionDropdown.options.Clear ();
		foreach (Resolution res in resolutions) {
			resolutionDropdown.options.Add (new Dropdown.OptionData (res.ToString ()));
		}
			
		presetDropdown.options.Clear ();
		foreach (string presetName in QualitySettings.names) {
			presetDropdown.options.Add (new Dropdown.OptionData (presetName));
		}

		LoadSettings ();
	}



	public void OnFullScreenToggle() {
		gameSettings.fullscreen = Screen.fullScreen = fullScreenToggle.isOn;

		dirtySetting = true;
	}

	public void OnResolutionChange() {
		Screen.SetResolution (
						resolutions [resolutionDropdown.value].width, 
						resolutions [resolutionDropdown.value].height, 
						Screen.fullScreen, 
						resolutions [resolutionDropdown.value].refreshRate);
		gameSettings.resolutionIndex = resolutionDropdown.value;

		dirtySetting = true;
	}

	public void OnPresetChange() {
		gameSettings.presetIndex = presetDropdown.value;
		QualitySettings.SetQualityLevel (presetDropdown.value);
		OnVSyncChange ();

		dirtySetting = true;
	}

	public void OnTextureQualityChange() {
		QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.value;

		dirtySetting = true;
	}

	public void OnAntialiasingChange() {
		QualitySettings.antiAliasing = gameSettings.antialiasing = (int)Mathf.Pow (2, antialiasingDropdown.value);

		dirtySetting = true;
	}

	public void OnVSyncChange() {
		QualitySettings.vSyncCount = gameSettings.vSync = vSyncDropdown.value;

		dirtySetting = true;
	}

	public void OnMusicVolumeChange() {
		AudioListener.volume = gameSettings.musicVolume = musicVolumeSlider.value;

		dirtySetting = true;
	}

	public void OnShowVRTutorialToggle() {
		gameSettings.showVRTutorial = showVRTutorialToggle.isOn;

		dirtySetting = true;
	}

	public void OnApplyButtonClick() {
		SaveSettings ();
	}

	public void OnBackButtonClick() {
		if (dirtySetting) {
			Debug.Log ("unsaved setting upon BACK, loading last saved settings");
			LoadSettings ();
		}
	}

	public void SaveSettings() {
		MainMenu.s_Singleton.StartDisplayInfo ("Saving Settings");
		string jsonData = JsonUtility.ToJson (gameSettings, true);
		File.WriteAllText (Application.persistentDataPath + "/gamesettings.json", jsonData);
		dirtySetting = false;
		MainMenu.s_Singleton.StopDisplayInfo ();
	}

	public void LoadSettings() {
		if (File.Exists (Application.persistentDataPath + "/gamesettings.json")) {
			MainMenu.s_Singleton.StartDisplayInfo ("Loading Settings");
			gameSettings = JsonUtility.FromJson<GameSettingsContainer> (File.ReadAllText (Application.persistentDataPath + "/gamesettings.json"));

			//visual
			fullScreenToggle.isOn = gameSettings.fullscreen;
			resolutionDropdown.value = gameSettings.resolutionIndex;
			//textureQualityDropdown.value = gameSettings.textureQuality;
			//antialiasingDropdown.value = gameSettings.antialiasing;
			vSyncDropdown.value = gameSettings.vSync;
			presetDropdown.value = gameSettings.presetIndex;
			showVRTutorialToggle.isOn = gameSettings.showVRTutorial;

			//audio controls
			musicVolumeSlider.value = gameSettings.musicVolume;
			dirtySetting = false;
			MainMenu.s_Singleton.StopDisplayInfo ();
		}
	}
}
