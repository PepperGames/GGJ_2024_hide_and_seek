using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomNumberUnityEventGenerator : MonoBehaviour
{
    public int max;
    public int min;
    public bool generateOnStart;
    public IntEvent OnGenerateInt; 
    public StringEvent OnGenerateString; 

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateRandomNumber();
        }
    }
    
    public void GenerateRandomNumber()
    {
        int result = Random.Range(min, max);
        OnGenerateInt.Invoke(result);
        OnGenerateString.Invoke(result.ToString());
    }
}
