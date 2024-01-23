using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseUnityEventComponent : MonoBehaviour
{
    public UnityEvent OnAwake;
    public UnityEvent OnStart;
    public UnityEvent OnOnDisable;

    private void Awake()
    {
        OnAwake.Invoke();
    }

    void Start()
    {
        OnStart.Invoke();
    }

    private void OnDisable()
    {
        OnOnDisable.Invoke();
    }
}
