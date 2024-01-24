using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpectratorNameDisplay : MonoBehaviour
{
    public TMP_Text nickDisplayText;
    [HideInInspector] public SpectratorCameraController spectratorCameraController;

    private void Update()
    {
        if (spectratorCameraController != null)
        {
            nickDisplayText.text = spectratorCameraController.GetCurrentSpectrator();
        }
        else
        {
            nickDisplayText.text = "";
        }
    }
}
