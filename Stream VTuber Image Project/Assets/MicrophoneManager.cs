using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class MicrophoneManager : MonoBehaviour {
	public Color InactiveColour;
	public Text DBValueTextObject;

	private Image image;
	private RectTransform ImageShape;

	private float MicSensitivity = 0;
	private float dbvalue;

	private string device;

	private readonly int sample_size = 250;
	private AudioClip RunningAudio;

	private IEnumerator ColourFader;
	private IEnumerator MicChecker;

	private bool MicOn;

	private bool _audiotriggered;
	public bool Audiotriggered {
		get => _audiotriggered;
		set {
			_audiotriggered = value;
			StartFade();
		}
	}
	public Vector3 OriginalScaleState { get; set; }
	public Vector3 OriginalPositionState { get; set; }

	private void Awake() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
		Application.runInBackground = true;

		image = GetComponent<Image>();
		image.color = new Color(InactiveColour.r, InactiveColour.g, InactiveColour.b, 255);
		ImageShape = GetComponent<RectTransform>();
	}

	private void Start() {
		if (string.IsNullOrEmpty(device)) {
			device = Microphone.devices[0];
		}
		OriginalScaleState = transform.localScale;
		OriginalPositionState = transform.localPosition;
	}

	private void Update() {
		if (MicOn) {
			if (!Microphone.IsRecording(device)) {
				ToggleMic(false);
				ToggleMic(true);
			}
			if (MicChecker == null) {
				StartCoroutine(MicChecker = CheckSpeaking());
			}
		} else {
			Audiotriggered = false;
			if (MicChecker != null) {
				StopCoroutine(MicChecker);
				MicChecker = null;
			}
		}
	}

	public void SetSensitivity(string x) {
		if (string.IsNullOrEmpty(x)) {
			MicSensitivity = 0;
		} else if (float.TryParse(x, out float value)) {
			MicSensitivity = Mathf.Clamp(value, -100, 100);
		}
	}

	public void SetMic(int value) {
		ToggleMic(false);
		device = Microphone.devices[value];
	}

	public IEnumerator CheckSpeaking() {
		Start:
		yield return null;

		float[] spectrum = new float[sample_size];

		int mic_pos = Microphone.GetPosition(device) - (sample_size + 1);

		if (mic_pos > 0 && RunningAudio != null) {

			RunningAudio.GetData(spectrum, mic_pos);

			float sum = 0;

			for (int i = 0; i < spectrum.Length; i++) {
				sum += spectrum[i] * spectrum[i];
			}

			dbvalue = Mathf.Round(20 * Mathf.Log10(Mathf.Sqrt(sum / spectrum.Length) / 0.1f));

			if (dbvalue < -101) {
				dbvalue = -101;
			}

			//if (Audiotriggered && ++count >= 3) {
			//	count = 0;
			//	Audiotriggered = dbvalue >= MicSensitivity;
			//} else if (!Audiotriggered) {
			//	Audiotriggered = dbvalue >= MicSensitivity;
			//}

			Audiotriggered = dbvalue >= MicSensitivity;

			if (DBValueTextObject != null) {
				DBValueTextObject.text = dbvalue.ToString();
			}

			if (Audiotriggered) {
				yield return new WaitForSecondsRealtime(0.5f);
			}

			#if UNITY_EDITOR
			Debug.Log($"Device:{device} | Audiotriggered:{Audiotriggered} | DBValue:{dbvalue} | MicSensitivity:{MicSensitivity}");
			#endif
		}

		if (MicOn) {
			goto Start;
		}

		yield return null;
		MicChecker = null;
	}

	public void ToggleMic(bool value) {
		MicOn = value;
		if (value) {
			RunningAudio = Microphone.Start(device, true, 1, 44100);
		} else {
			Microphone.End(device);
			RunningAudio = null;
			StartFade();
		}
	}

	// Colour //

	public void StartFade() {
		if (ColourFader == null) {
			StartCoroutine(ColourFader = Fade());
		}
	}

	private IEnumerator Fade() {
		float speed = 2f;
		Restart:
		bool trigValue = Audiotriggered;
		if (trigValue) {
			if (ImageLoaderScript.ONImageData != null) {
				ImageShape.sizeDelta = ImageLoaderScript.ONImageDimensions;
				image.sprite = ImageLoaderScript.ONImageData;
			}

			transform.localScale = OriginalScaleState + (new Vector3(0.02f, 0.02f, 0) * OriginalScaleState.x);
			transform.localPosition = OriginalPositionState + new Vector3(0, 0.1f, 0);

			// 0 - 1 for data, 255 for processing

			float x = (image.color.r + image.color.g + image.color.b) / 3;
			while (x < 1) {
				if (trigValue != Audiotriggered) {
					yield return new WaitForSecondsRealtime(0.5f);
					goto Restart;
				}

				x += Time.deltaTime * speed;
				if (x > 1) {
					x = 1;
				}
				image.color = new Color(x, x, x, 1);

				yield return null;
			}
			image.color = new Color(1, 1, 1, 1);
		} else {
			float x = (image.color.r + image.color.g + image.color.b) / 3;
			float y = (InactiveColour.r + InactiveColour.g + InactiveColour.b) / 3;
			while (x > y) {
				if (trigValue != Audiotriggered) {
					goto Restart;
				}

				x -= Time.deltaTime * speed;
				if (x < y) {
					x = y;
				}
				image.color = new Color(x, x, x, 1);

				yield return null;
			}
			image.color = new Color(InactiveColour.r, InactiveColour.g, InactiveColour.b, 1);

			transform.localScale = OriginalScaleState;
			transform.localPosition = OriginalPositionState;

			if (ImageLoaderScript.OFFImageData != null) {
				ImageShape.sizeDelta = ImageLoaderScript.OFFImageDimensions;
				image.sprite = ImageLoaderScript.OFFImageData;
			}

		}
		ColourFader = null;
	}
}
