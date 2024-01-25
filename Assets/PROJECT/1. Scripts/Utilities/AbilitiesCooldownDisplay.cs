using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilitiesCooldownDisplay : MonoBehaviour
{
    public string myAbilityName;
    public IAbility myAbility;
    public Slider cooldownSlider;
    public UnityEvent OnAbilityCooldown;
    public UnityEvent OnAbilityReady;

    private void Update()
    {
        UpdateAbilityDisplay();
    }

    private void UpdateAbilityDisplay()
    {
        if (myAbility != null)
        {
            float cooldown = myAbility.GetCurrentCooldown();
            bool isCooldown = myAbility.GetCooldownBool();
            cooldownSlider.minValue = 0;
            cooldownSlider.maxValue = myAbility.GetMaxCooldown();
            cooldownSlider.value = cooldown;
            
            if (isCooldown)
            {
                OnAbilityCooldown.Invoke();
            }
            else
            {
                OnAbilityReady.Invoke();
            }

        }
        else
        {
            Debug.Log("No IAbility found");
        }
    }
}
