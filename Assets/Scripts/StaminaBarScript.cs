using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarScript : MonoBehaviour
{
    public Player playerScript;
    public Slider slider;
    // Start is called before the first frame update
    public void SetStamina(float stamina)
    {
        slider.value = stamina;
    }

    public void setMaxStamina(float stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }
}
