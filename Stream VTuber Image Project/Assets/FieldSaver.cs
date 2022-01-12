using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class FieldSaver : MonoBehaviour {
	[Header("INPUTS")]
	public Dropdown ImageOFFDrop;
	public Dropdown ImageONDrop;

	public InputField PosXIn;
	public InputField PosYIn;
	public InputField ScaleIn;

	public Dropdown MicOptionDrop;

	public InputField MicSensIn;

	public InputField RedIn;
	public InputField GreenIn;
	public InputField BlueIn;

	private void Awake() {
		if (StaticSaveModule.CurrentSettings != null) {
			if (PosXIn != null && !string.IsNullOrEmpty(StaticSaveModule.CurrentSettings.PosX)) {
				PosXIn.text = StaticSaveModule.CurrentSettings.PosX;
			}
			if (PosYIn != null && !string.IsNullOrEmpty(StaticSaveModule.CurrentSettings.PosY)) {
				PosYIn.text = StaticSaveModule.CurrentSettings.PosY;
			}
			if (ScaleIn != null && !string.IsNullOrEmpty(StaticSaveModule.CurrentSettings.Scale)) {
				ScaleIn.text = StaticSaveModule.CurrentSettings.Scale;
			}
			if (MicSensIn != null && !string.IsNullOrEmpty(StaticSaveModule.CurrentSettings.MicSensitivity)) {
				MicSensIn.text = StaticSaveModule.CurrentSettings.MicSensitivity;
			}
			if (RedIn != null && !string.IsNullOrEmpty(StaticSaveModule.CurrentSettings.ColourR)) {
				RedIn.text = StaticSaveModule.CurrentSettings.ColourR;
			}
			if (GreenIn != null && !string.IsNullOrEmpty(StaticSaveModule.CurrentSettings.ColourG)) {
				GreenIn.text = StaticSaveModule.CurrentSettings.ColourG;
			}
			if (BlueIn != null && !string.IsNullOrEmpty(StaticSaveModule.CurrentSettings.ColourB)) {
				BlueIn.text = StaticSaveModule.CurrentSettings.ColourB;
			}
		}
	}

	public void SaveSettings() {
		if (StaticSaveModule.CurrentSettings != null) {
			if (ImageOFFDrop != null) {
				StaticSaveModule.CurrentSettings.ImageOneLocation = ImageOFFDrop.options[ImageOFFDrop.value].text;
			}
			if (ImageONDrop != null) {
				StaticSaveModule.CurrentSettings.ImageTwoLocation = ImageONDrop.options[ImageONDrop.value].text;
			}
			if (PosXIn != null) {
				StaticSaveModule.CurrentSettings.PosX = PosXIn.text;
			}
			if (PosYIn != null) {
				StaticSaveModule.CurrentSettings.PosY = PosYIn.text;
			}
			if (ScaleIn != null) {
				StaticSaveModule.CurrentSettings.Scale = ScaleIn.text;
			}
			if (MicOptionDrop != null) {
				StaticSaveModule.CurrentSettings.ChosenMic = MicOptionDrop.options[MicOptionDrop.value].text;
			}
			if (MicSensIn != null) {
				StaticSaveModule.CurrentSettings.MicSensitivity = MicSensIn.text;
			}
			if (RedIn != null) {
				StaticSaveModule.CurrentSettings.ColourR = RedIn.text;
			}
			if (GreenIn != null) {
				StaticSaveModule.CurrentSettings.ColourG = GreenIn.text;
			}
			if (BlueIn != null) {
				StaticSaveModule.CurrentSettings.ColourB = BlueIn.text;
			}
		}
	}
}
