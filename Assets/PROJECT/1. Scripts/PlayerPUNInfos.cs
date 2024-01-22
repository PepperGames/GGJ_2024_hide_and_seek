using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPUNInfos : MonoBehaviourPun
{
    public UnityEvent OnNotMyView;
    
    private void Start()
    {
        if (!photonView.IsMine)
        {
            OnNotMyView.Invoke();
        }
    }

    private void EnablePlayerControl()
    {
        // Код для активации управления игроком
    }

    private void DisablePlayerControl()
    {
        // Код для деактивации управления игроком
    }
}

