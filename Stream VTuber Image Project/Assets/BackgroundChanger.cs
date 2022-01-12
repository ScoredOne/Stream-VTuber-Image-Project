using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BackgroundChanger : MonoBehaviour
{
    private Camera Background;

    private byte ColourR = 0;
    private byte ColourG = 255;
    private byte ColourB = 0;

    void Awake()
    {
        Background = GetComponent<Camera>();
    }

    // string because input field
    public void UpdateRed(string value) {
        if (byte.TryParse(value, out ColourR)) {
            Background.backgroundColor = new Color32(ColourR, ColourG, ColourB, 255);
        }
	}

    public void UpdateGreen(string value) {
        if (byte.TryParse(value, out ColourG)) {
            Background.backgroundColor = new Color32(ColourR, ColourG, ColourB, 255);
        }
    }

    public void UpdateBlue(string value) {
        if (byte.TryParse(value, out ColourB)) {
            Background.backgroundColor = new Color32(ColourR, ColourG, ColourB, 255);
        }
    }
}
