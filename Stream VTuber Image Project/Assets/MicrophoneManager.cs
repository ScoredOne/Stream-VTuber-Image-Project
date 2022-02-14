using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class MicrophoneManager : MonoBehaviour {
	public Color InactiveColour;

	private Image image;
	private RectTransform ImageShape;

	private float MicSensitivity = -10;
	private float dbvalue;

	private string device;

	private readonly int sample_size = 250;
	private AudioClip RunningAudio;

	private IEnumerator ColourFader;
	private IEnumerator DelayIEnum;

	private bool MicOn;

	private bool _audiotriggered;
	public bool Audiotriggered {
		get => _audiotriggered;
		set {
			if (_audiotriggered != value) {
				AudioTriggerChanged.Invoke(value);
				_audiotriggered = value;
			}
		}
	}
	private Action<bool> AudioTriggerChanged => e => {
		if (e) {
			FadeIn();
		} else {
			if (DelayIEnum != null) {
				StopCoroutine(DelayIEnum);
				DelayIEnum = null;
			}
			StartCoroutine(DelayIEnum = TriggerDelay());
		}
	};

	public Vector3 OriginalScaleState { get; set; }
	public Vector3 OriginalPositionState { get; set; }

	private void Awake() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 25;
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
			CheckSpeaking();
		}

		//Debug.Log($"Audiotriggered:{Audiotriggered} | DBValue:{dbvalue} | MicSensitivity:{MicSensitivity}");
	}

	public void SetSensitivity(string x) {
		if (string.IsNullOrEmpty(x)) {
			MicSensitivity = -10;
		} else if (float.TryParse(x, out float value)) {
			MicSensitivity = Mathf.Clamp(value, -100, 100) - 10;
		}
	}

	public void SetMic(int value) {
		ToggleMic(false);
		device = Microphone.devices[value];
	}

	public void CheckSpeaking() {
		float[] spectrum = new float[sample_size];

		int mic_pos = Microphone.GetPosition(device) - (sample_size + 1);

		if (mic_pos < 0 || RunningAudio == null) {
			return;
		}

		RunningAudio.GetData(spectrum, mic_pos);

		float sum = 0;

		for (int i = 0; i < spectrum.Length; i++) {
			sum += spectrum[i] * spectrum[i];
		}

		dbvalue = 20 * Mathf.Log10(Mathf.Sqrt(sum / spectrum.Length) / 0.1f);
		if (dbvalue < -160) {
			dbvalue = -160;
		}

		Audiotriggered = dbvalue > MicSensitivity;
	}

	private IEnumerator TriggerDelay() {
		yield return new WaitForSeconds(0.5f);
		FadeOut();
		DelayIEnum = null;
	}

	public void ToggleMic(bool value) {
		MicOn = value;
		if (value) {
			RunningAudio = Microphone.Start(device, true, 1, 44100);
		} else {
			Microphone.End(device);
			RunningAudio = null;

			FadeOut();
		}
	}

	// Colour //

	public void FadeIn() {
		if (ColourFader != null) {
			StopCoroutine(ColourFader);
		}
		ColourFader = null;
		StartCoroutine(ColourFader = Fade(true));
	}

	public void FadeOut() {
		if (ColourFader != null) {
			StopCoroutine(ColourFader);
		}
		ColourFader = null;
		StartCoroutine(ColourFader = Fade(false));
	}

	private IEnumerator Fade(bool In) {
		float speed = 7.5f;
		if (In) {
			if (ImageLoaderScript.ONImageData != null) {
				ImageShape.sizeDelta = ImageLoaderScript.ONImageDimensions;
				image.sprite = ImageLoaderScript.ONImageData;
			}

			transform.localScale = OriginalScaleState + (new Vector3(0.02f, 0.02f, 0) * OriginalScaleState.x);
			transform.localPosition = OriginalPositionState + new Vector3(0, 0.1f, 0);

			// 0 - 1 for data, 255 for processing

			float x = (image.color.r + image.color.g + image.color.b) / 3;
			while (x < 1) {
				x += Time.deltaTime * speed;
				if (x > 1) {
					x = 1;
				}
				image.color = new Color(x, x, x, 1);
				yield return false;
			}
			image.color = new Color(1, 1, 1, 1);
		} else {
			float x = (image.color.r + image.color.g + image.color.b) / 3;
			float y = (InactiveColour.r + InactiveColour.g + InactiveColour.b) / 3;
			while (x > y) {
				x -= Time.deltaTime * speed;
				if (x < y) {
					x = y;
				}
				image.color = new Color(x, x, x, 1);
				yield return false;
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
