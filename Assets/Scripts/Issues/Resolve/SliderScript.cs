using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderScript : MonoBehaviour
{
    [SerializeField]
    private Slider slider; // reference to the UI Slider

    [SerializeField]
    private TextMeshProUGUI sliderValue;   // Reference to the TextMesh Pro UGUI component to display the slider's value

    void Start()
    {
        // ensure that the slider and sliderValue are set before adding listeners
        if (slider == null)
        {
            Debug.LogError("Slider not assigned in SliderScript!");
            return;
        }

        if (sliderValue == null)
        {
            Debug.LogError("Slider value TextMeshProUGUI not assigned in SliderScript!");
            return;
        }

        // add listener to the slider to detect when its value changes
        slider.onValueChanged.AddListener((v) =>
        {
            // set the text of the sliderValue to the current value of the slider
            sliderValue.text = v.ToString("0"); //no decimal places
        });
    }
}
