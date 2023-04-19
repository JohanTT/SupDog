using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public Image speedAbilityCD;
    public Image speedAbilityCA;
    public float cooldown = 5;
    bool isCooldown = false;
    bool isCasting = false;

    // Start is called before the first frame update
    void Start()
    {
        speedAbilityCD.fillAmount = 0;
        speedAbilityCA.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        coolDownAbility();   
    }

    void coolDownAbility() {
        if (Input.GetKey(KeyCode.K) && isCooldown == false && isCasting == false) {
            isCasting = true;
            speedAbilityCA.fillAmount = 1;
            speedAbilityCD.fillAmount = 1;
        }

        if (isCasting) {
            speedAbilityCA.fillAmount -= 1 / cooldown * Time.deltaTime;

            if(speedAbilityCA.fillAmount <= 0) {
                isCasting = false;
                speedAbilityCA.fillAmount = 0;
                isCooldown = true;
                speedAbilityCD.fillAmount = 1;
            }
        }

        if (isCooldown) {
            speedAbilityCD.fillAmount -= 1 / cooldown * Time.deltaTime; 

            if (speedAbilityCD.fillAmount <= 0) {
                speedAbilityCD.fillAmount = 0;
                isCooldown = false;
            }
        }
    }
}
