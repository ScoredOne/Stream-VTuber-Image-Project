using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class ValueLimiter : MonoBehaviour
{
    public float Upper;
    public float Lower;

    private InputField Field;

    void Awake()
    {
        Field = GetComponent<InputField>();
        Field.onValueChanged.AddListener(valueChanged);
    }

    private void valueChanged(string value) {
        if (float.TryParse(value, out float result)) {
            if (result < Lower) {
                Field.textComponent.text = Lower.ToString();
            } else if (result > Upper) {
                Field.textComponent.text = Upper.ToString();
            }
		} else {
            Field.text = "0";
		}
	}
}
