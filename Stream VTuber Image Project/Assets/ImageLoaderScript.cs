using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoaderScript : MonoBehaviour
{
	public Image ImageItem;

	public RectTransform ImageTransform;

	public Text debugtext;

	public Dropdown OutputBoxOFF;

	public Dropdown OutputBoxON;

	private string[] directorysave;

	private static string directory = "";

	public static Texture2D OFFImageData;

	public static Texture2D ONImageData;


	// Start is called before the first frame update
	void Start() {
		CreateDirectoryString();
		UpdateDebugOut();
		FindImageFiles(true);
		OutputBoxOFF.RefreshShownValue();
		OutputBoxON.RefreshShownValue();
	}

	private static void CreateDirectoryString() {
		string[] split = Application.dataPath.Split('/');
		for (int x = 0; x < split.Length - 1; x++) {
			directory += split[x] + "/";
		}
		directory += "Images";
		Directory.CreateDirectory(directory);
	}

	public void UpdateDebugOut() {
		if (debugtext != null) {
			debugtext.text = directory;
		}
	}

	public void FindImageFiles(bool OverrideStringCheck = false) {
		string[] files = Directory.GetFiles(directory);
		if (directorysave == null) {
			directorysave = new string[files.Length];
			files.CopyTo(directorysave, 0);
		}

		int count = 0;
		if (OverrideStringCheck || files.Length != directorysave.Length || !files.Any(e => e.Equals(directorysave[count++])) && OutputBoxOFF != null && OutputBoxON != null) {
			OutputBoxOFF.options.Clear(); 
			OutputBoxON.options.Clear();
			foreach (string x in files) {
				int index = x.IndexOf("\\");
				string y = x.Substring(index);
				string[] z = y.Split('.');
				if (z[z.Length - 1].Equals("png")) {
					OutputBoxOFF.options.Add(new Dropdown.OptionData(y));
					OutputBoxON.options.Add(new Dropdown.OptionData(y));
				}
			}
			if (StaticSaveModule.CurrentSettings != null &&
				OutputBoxOFF.options.Any(e => e.text.Equals(StaticSaveModule.CurrentSettings.ImageOneLocation)) &&
				OutputBoxON.options.Any(e => e.text.Equals(StaticSaveModule.CurrentSettings.ImageTwoLocation))) {
				OutputBoxOFF.value = OutputBoxOFF.options.IndexOf(OutputBoxOFF.options.Find(e => e.text.Equals(StaticSaveModule.CurrentSettings.ImageOneLocation)));
				OutputBoxOFF.RefreshShownValue();
				OutputBoxON.value = OutputBoxON.options.IndexOf(OutputBoxON.options.Find(e => e.text.Equals(StaticSaveModule.CurrentSettings.ImageTwoLocation)));
				OutputBoxON.RefreshShownValue();
				LoadImageFile();
			}
		}
	}

	public void LoadImageFile() {
		OFFImageData = LoadPNG(OutputBoxOFF.options[OutputBoxOFF.value].text);
		ONImageData = LoadPNG(OutputBoxON.options[OutputBoxON.value].text);
		ImageTransform.sizeDelta = new Vector2(OFFImageData.width, OFFImageData.height);
		ImageItem.sprite = Sprite.Create(OFFImageData, new Rect(0, 0, OFFImageData.width, OFFImageData.height), Vector2.zero);
	}

	public static Texture2D LoadPNG(string fileName) {
		fileName = directory + "/" + fileName;
		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists(fileName)) {
			fileData = File.ReadAllBytes(fileName);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		}
		else {
			Debug.Log(fileName + "\t :File Failed");
		}
		return tex;
	}
}
