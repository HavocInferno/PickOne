using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour {

	//visual controls
	public Toggle fullScreenToggle;
	public Dropdown resolutionDropdown;
	public Dropdown textureQualityDropdown;
	public Dropdown antialiasingDropdown;
	public Dropdown vSyncDropdown;

	//audio controls
	public AudioSource musicSource;
	public Slider musicVolumeSlider;


	//---------------------------------------------------------

	public Resolution[] resolutions;
	public GameSettingsContainer gameSettings;

	void OnEnable() {
		gameSettings = new GameSettingsContainer ();

		//fullScreenToggle.onValueChanged.AddListener (delegate {	OnFullScreenToggle ();	});
		resolutionDropdown.onValueChanged.AddListener (delegate {	OnResolutionChange ();	});
		//textureQualityDropdown.onValueChanged.AddListener (delegate {	OnTextureQualityChange ();	});
		//antialiasingDropdown.onValueChanged.AddListener (delegate {	OnAntialiasingChange ();	});
		//vSyncDropdown.onValueChanged.AddListener (delegate {	OnVSyncChange ();	});
		//musicVolumeSlider.onValueChanged.AddListener (delegate {	OnMusicVolumeChange ();	});

		resolutions = Screen.resolutions;
		resolutionDropdown.options.Clear ();
		foreach (Resolution res in resolutions) {
			resolutionDropdown.options.Add (new Dropdown.OptionData (res.ToString ()));
		}
	}



	public void OnFullScreenToggle() {
		gameSettings.fullscreen = Screen.fullScreen = fullScreenToggle.isOn;
	}

	public void OnResolutionChange() {
		Screen.SetResolution (
						resolutions [resolutionDropdown.value].width, 
						resolutions [resolutionDropdown.value].height, 
						Screen.fullScreen, 
						resolutions [resolutionDropdown.value].refreshRate);
	}

	public void OnTextureQualityChange() {
		QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.value;
	}

	public void OnAntialiasingChange() {
		QualitySettings.antiAliasing = gameSettings.antialiasing = (int)Mathf.Pow (2, antialiasingDropdown.value);
	}

	public void OnVSyncChange() {
		QualitySettings.vSyncCount = gameSettings.vSync = vSyncDropdown.value;
	}

	public void OnMusicVolumeChange() {
		musicSource.volume = gameSettings.musicVolume = musicVolumeSlider.value;
	}

	public void SaveSettings() {

	}

	public void LoadSettings() {

	}
}
