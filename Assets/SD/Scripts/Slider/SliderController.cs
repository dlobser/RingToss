using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public TMP_Text textValue;
    public Slider slider;

    public float SliderValue => slider.value;

    public void OnValueChange(float value)
    {
        textValue.text = value.ToString();
    }
}