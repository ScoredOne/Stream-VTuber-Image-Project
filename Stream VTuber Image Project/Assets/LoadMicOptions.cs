using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadMicOptions : MonoBehaviour
{
    public Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown.options.Clear();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string mic in Microphone.devices) {
            string[] x = mic.Split('(');
            string y = x[1].Split(')')[0];
            
            options.Add(new Dropdown.OptionData(y));
        }
        dropdown.AddOptions(options);
    }
}
