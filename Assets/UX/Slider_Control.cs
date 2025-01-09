using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slider_Control : MonoBehaviour
{

    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderValue;


    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(displayText);
    }

    void displayText(float arg) {
        if(slider.value % 1 == 0) {
            sliderValue.text = slider.value.ToString();
       
        } else {

            try {
                
                var currentString = slider.value.ToString();
                var trimmedString = "";

                trimmedString = currentString.Substring(0, 4);

                sliderValue.text = trimmedString;
            
            } catch(System.Exception e) {
                sliderValue.text = slider.value.ToString();
            }
        }
    }
}
