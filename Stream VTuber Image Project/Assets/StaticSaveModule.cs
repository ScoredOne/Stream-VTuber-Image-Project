using System;
using System.IO;

using UnityEngine;

public static class StaticSaveModule
{
	public static SaveDataClass CurrentSettings { get; private set; } = null;

	static StaticSaveModule() {
		if (File.Exists(Directory.GetCurrentDirectory() + @"\Settings.txt")) {
			CurrentSettings = JsonUtility.FromJson<SaveDataClass>(File.ReadAllText(Directory.GetCurrentDirectory() + @"\Settings.txt"));
			if (CurrentSettings == null) {
				CurrentSettings = new SaveDataClass();
			}
			CurrentSettings.DataChanged = UpdateSettingsFile;
		} else {
			File.Create(Directory.GetCurrentDirectory() + @"\Settings.txt").Close();
			CurrentSettings = new SaveDataClass {
				DataChanged = UpdateSettingsFile
			};
		}
	}

	private static void UpdateSettingsFile() {
		if (CurrentSettings != null) {
			File.WriteAllText(Directory.GetCurrentDirectory() + @"\Settings.txt", JsonUtility.ToJson(CurrentSettings, true));
		}
	}
}

[Serializable]
public class SaveDataClass {
	public Action DataChanged;

	[SerializeField]
	private string _imageOneLocation = "";
	public string ImageOneLocation {
		get => _imageOneLocation;
		set {
			if (!_imageOneLocation.Equals(value)) {
				_imageOneLocation = value;
				DataChanged?.Invoke();
			}
		}
	}
	[SerializeField]
	private string _imageTwoLocation = "";
	public string ImageTwoLocation {
		get => _imageTwoLocation;
		set {
			if (!_imageTwoLocation.Equals(value)) {
				_imageTwoLocation = value;
				DataChanged?.Invoke();
			}
		}
	}

	[SerializeField]
	private string _posX = "";
	public string PosX {
		get => _posX;
		set {
			if (!_posX.Equals(value)) {
				_posX = value;
				DataChanged?.Invoke();
			}
		}
	}
	[SerializeField]
	private string _posY = "";
	public string PosY {
		get => _posY;
		set {
			if (!_posY.Equals(value)) {
				_posY = value;
				DataChanged?.Invoke();
			}
		}
	}
	[SerializeField]
	private string _scale = "";
	public string Scale {
		get => _scale;
		set {
			if (!_scale.Equals(value)) {
				_scale = value;
				DataChanged?.Invoke();
			}
		}
	}

	[SerializeField]
	private string _chosenMic = "";
	public string ChosenMic {
		get => _chosenMic;
		set {
			if (!_chosenMic.Equals(value)) {
				_chosenMic = value;
				DataChanged?.Invoke();
			}
		}
	}
	[SerializeField]
	private string _micSensitivity = "";
	public string MicSensitivity {
		get => _micSensitivity;
		set {
			if (!_micSensitivity.Equals(value)) {
				_micSensitivity = value;
				DataChanged?.Invoke();
			}
		}
	}

	[SerializeField]
	private string _colourR = "";
	public string ColourR {
		get => _colourR;
		set {
			if (!_colourR.Equals(value)) {
				_colourR = value;
				DataChanged?.Invoke();
			}
		}
	}
	[SerializeField]
	private string _colourG = "";
	public string ColourG {
		get => _colourG;
		set {
			if (!_colourG.Equals(value)) {
				_colourG = value;
				DataChanged?.Invoke();
			}
		}
	}
	[SerializeField]
	private string _colourB = "";
	public string ColourB {
		get => _colourB;
		set {
			if (!_colourB.Equals(value)) {
				_colourB = value;
				DataChanged?.Invoke();
			}
		}
	}
}