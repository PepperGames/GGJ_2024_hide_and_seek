using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LaughterUIController : MonoBehaviourPun
{
    public Slider laughterSlider;
    public LaughterMeter laughterMeter;

    public UnityEvent onLaughterUse;
    public UnityEvent onUncontrollableLaughter;

    public UnityEvent onLaughterLevel0to25;
    public UnityEvent onLaughterLevel25to50;
    public UnityEvent onLaughterLevel50to80;
    public UnityEvent onLaughterLevel80to99;

    private bool[] laughterThresholdsReached = new bool[4];

    private void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
        UpdateLaughterSlider();
        CheckForLaughterEvents();
    }

    void UpdateLaughterSlider()
    {
        laughterSlider.value = laughterMeter.currentLaughter / laughterMeter.maxLaughter;
    }

    void CheckForLaughterEvents()
    {
        if (laughterMeter.isUsingLaughter)
        {
            onLaughterUse.Invoke();
        }

        if (laughterMeter.currentLaughter >= laughterMeter.maxLaughter && !laughterThresholdsReached[3])
        {
            onUncontrollableLaughter.Invoke();
        }

        // Проверка пороговых значений
        float laughterValue = laughterMeter.currentLaughter / laughterMeter.maxLaughter * 100;
        CheckThreshold(laughterValue, 0, 25, onLaughterLevel0to25, 0);
        CheckThreshold(laughterValue, 25, 50, onLaughterLevel25to50, 1);
        CheckThreshold(laughterValue, 50, 80, onLaughterLevel50to80, 2);
        CheckThreshold(laughterValue, 80, 99, onLaughterLevel80to99, 3);
    }

    void CheckThreshold(float value, float min, float max, UnityEvent eventToInvoke, int thresholdIndex)
    {
        if (value >= min && value < max && !laughterThresholdsReached[thresholdIndex])
        {
            eventToInvoke.Invoke();
            laughterThresholdsReached[thresholdIndex] = true;
        }
        else if ((value < min || value >= max) && laughterThresholdsReached[thresholdIndex])
        {
            laughterThresholdsReached[thresholdIndex] = false;
        }
    }
}