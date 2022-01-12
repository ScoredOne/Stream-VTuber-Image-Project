using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadMicOptions : MonoBehaviour
{
    public Dropdown dropdown;
    public MicrophoneManager manager;

    // Start is called before the first frame update
    void Start()
    {
        if (StaticSaveModule.CurrentSettings != null &&
                dropdown.options.Any(e => e.text.Equals(StaticSaveModule.CurrentSettings.ChosenMic))) {
            dropdown.value = dropdown.options.IndexOf(dropdown.options.Find(e => e.text.Equals(StaticSaveModule.CurrentSettings.ChosenMic)));
            dropdown.RefreshShownValue();
            if (manager != null) {
                manager.SetMic(dropdown.value);
            }
        }
    }

	private void Awake() {
        dropdown.options.Clear();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string mic in Microphone.devices) {
            string[] x = mic.Split('(');
            string y = x[1].Split(')')[0];
            
            options.Add(new Dropdown.OptionData(y));
        }
        dropdown.AddOptions(options);
        dropdown.RefreshShownValue();
    }
}
