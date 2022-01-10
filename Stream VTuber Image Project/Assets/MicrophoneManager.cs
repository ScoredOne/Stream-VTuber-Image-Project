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

	private void Start() {
		Application.targetFrameRate = 25;

		image = GetComponent<Image>();
		ImageShape = GetComponent<RectTransform>();
		image.color = new Color(InactiveColour.r, InactiveColour.g, InactiveColour.b, 255);
		device = Microphone.devices[0];
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
				ImageShape.sizeDelta = new Vector2(ImageLoaderScript.ONImageData.width, ImageLoaderScript.ONImageData.height);
				image.sprite = Sprite.Create(ImageLoaderScript.ONImageData, new Rect(0, 0, ImageLoaderScript.ONImageData.width, ImageLoaderScript.ONImageData.height), Vector2.zero);
			}

			transform.localScale = OriginalScaleState + new Vector3(0.02f, 0.02f, 0);
			transform.localPosition = OriginalPositionState + new Vector3(0, 0.1f, 0);

			float r = image.color.r;
			float g = image.color.g;
			float b = image.color.b;
			while (r < 1 || g < 1 || b < 1) {
				if (r < 1) {
					r += Time.deltaTime * speed;
				} else {
					r = 1;
				}
				if (g < 1) {
					g += Time.deltaTime * speed;
				} else {
					g = 1;
				}
				if (b < 1) {
					b += Time.deltaTime * speed;
				} else {
					b = 1;
				}
				image.color = new Color(r, g, b, 1);
				yield return false;
			}
			image.color = new Color(1, 1, 1, 1);
		} else {
			float r = image.color.r;
			float g = image.color.g;
			float b = image.color.b;
			while (r > InactiveColour.r || g > InactiveColour.g || b > InactiveColour.b) {
				if (r > InactiveColour.r) {
					r -= Time.deltaTime * speed;
				} else {
					r = InactiveColour.r;
				}
				if (g > InactiveColour.g) {
					g -= Time.deltaTime * speed;
				} else {
					g = InactiveColour.g;
				}
				if (b > InactiveColour.b) {
					b -= Time.deltaTime * speed;
				} else {
					b = InactiveColour.b;
				}
				image.color = new Color(r, g, b, 1);
				yield return false;
			}
			image.color = new Color(InactiveColour.r, InactiveColour.g, InactiveColour.b, 1);

			transform.localScale = OriginalScaleState;
			transform.localPosition = OriginalPositionState;

			if (ImageLoaderScript.OFFImageData != null) {
				ImageShape.sizeDelta = new Vector2(ImageLoaderScript.OFFImageData.width, ImageLoaderScript.OFFImageData.height);
				image.sprite = Sprite.Create(ImageLoaderScript.OFFImageData, new Rect(0, 0, ImageLoaderScript.OFFImageData.width, ImageLoaderScript.OFFImageData.height), Vector2.zero);
			}

		}
		ColourFader = null;
	}

}
