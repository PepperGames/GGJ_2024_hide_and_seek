using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int>
{
}

[System.Serializable]
public class StringEvent : UnityEvent<string>
{
}

[System.Serializable]
public class FloatEvent : UnityEvent<float>
{
}

public class SimpleCounter : MonoBehaviour
{
    public int startCountNumber;
    public IntEvent OnCountChangedIntager; // Используем наш новый IntEvent
    public StringEvent OnCountChangedString; // Используем наш новый IntEvent
    public FloatEvent OnCountChangedFloat;
    private int currentCount;

    private void Start()
    {
        ResetCount();
    }

    public int CurrentCount
    {
        get { return currentCount; }
        set
        {
            if (currentCount != value)
            {
                currentCount = value;
                if (gameObject.activeSelf)
                {
                    OnCountChangedIntager.Invoke(currentCount); // Вызываем ивент при изменении значения
                    OnCountChangedString.Invoke(currentCount.ToString());
                    OnCountChangedFloat.Invoke((float) currentCount);
                }
            }
        }
    }

    public void CountDown()
    {
        CurrentCount--;
    }
    
    public void CountDown(int _number)
    {
        CurrentCount -= _number;
    }

    public void CountUp()
    {
        CurrentCount++;
    }
    
    public void CountUp(int _number)
    {
        CurrentCount += _number;
    }

    public void ResetCount()
    {
        CurrentCount = startCountNumber;
    }

    public void SetCounterToNumber(int _number)
    {
        CurrentCount = _number;
    }
}

