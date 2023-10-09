using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    // references to the toggles
    public Toggle yesToggle;
    public Toggle noToggle;

    // flag to prevent recursive changes when updating toggle values
    private bool isToggling = false;

    void Start()
    {
        // ensuringg toggles are set before adding listeners
        if (yesToggle == null || noToggle == null)
        {
            Debug.LogError("Toggles not assigned in ToggleController!");
            return;
        }

        // adding listener to the toggles to detect when their values change
        yesToggle.onValueChanged.AddListener(OnYesToggleValueChanged);
        noToggle.onValueChanged.AddListener(OnNoToggleValueChanged);
    }

    void OnYesToggleValueChanged(bool isOn)
    {
        // if currently updating a toggle, skip to prevent recursion
        if (isToggling) return;

        // lock to update the toggle
        isToggling = true;

        // if yes toggle was turned on then turn off no toggle
        if (isOn && noToggle != null)
        {
            noToggle.isOn = false;
        }

        // Release lock
        isToggling = false;
    }

    void OnNoToggleValueChanged(bool isOn)
    {
        // if currently updating a toggle, skip to prevent recursion
        if (isToggling) return;

        // lock to update the toggle
        isToggling = true;

        // if No toggle was turned on, turn off yes toggle
        if (isOn && yesToggle != null)
        {
            yesToggle.isOn = false;
        }

        // release lock
        isToggling = false;
    }
}
