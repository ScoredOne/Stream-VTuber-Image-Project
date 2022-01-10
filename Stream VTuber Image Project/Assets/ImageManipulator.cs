using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManipulator : MonoBehaviour
{
	public MicrophoneManager ImageObject;

	public InputField ScaleInput;

	private void Start() {
		ScaleInput.text = ImageObject.transform.localScale.x.ToString();
	}

	public void UpdateXPos(string x) {
		if (string.IsNullOrEmpty(x)) {
			ImageObject.transform.localPosition = new Vector3(0, ImageObject.transform.localPosition.y, ImageObject.transform.localPosition.z);
		}
		else if (float.TryParse(x, out float value)) {
			ImageObject.transform.localPosition = new Vector3(value, ImageObject.transform.localPosition.y, ImageObject.transform.localPosition.z);
		}
	}

	public void UpdateYPos(string x) {
		if (string.IsNullOrEmpty(x)) {
			ImageObject.transform.localPosition = new Vector3(ImageObject.transform.localPosition.x, 0, ImageObject.transform.localPosition.z);
		}
		else if (float.TryParse(x, out float value)) {
			ImageObject.transform.localPosition = new Vector3(ImageObject.transform.localPosition.x, value, ImageObject.transform.localPosition.z);
		}
	}

	public void UpdateScale(string x) {
		if (string.IsNullOrEmpty(x)) {
			ImageObject.transform.localScale = ImageObject.OriginalScaleState = new Vector3(1, 1, 1);
		}
		else if (float.TryParse(x, out float value)) {
			if (value < 0) {
				value = 0.00001f;
			}
			ImageObject.transform.localScale = ImageObject.OriginalScaleState = new Vector3(value, value, 1);
		}
	}
}
