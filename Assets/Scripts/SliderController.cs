using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerController;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    
    void Start() {
        slider = GetComponent<Slider>();
        slider.gameObject.SetActive(false);
    }

    public void TriggerSlider(bool check) 
    {
        slider.gameObject.SetActive(check);
    }

    public void DiggingItem(float value)
    {
        if (slider.gameObject.activeInHierarchy == true) StartCoroutine(SliderValueChange(value));
    }

    public IEnumerator SliderValueChange(float value) {
        slider.value = value;
        yield return new WaitForSeconds(0.00f);
    }
}
