using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPUNInfos : MonoBehaviourPun
{
    public UnityEvent OnNotMyView;
    public UnityEvent OnPlayerIsBOB;
    public UnityEvent OnPlayerIsCOP;
    public UnityEvent OnMyPhotonView;
    public Camera mainCamera;
    public CameraUIDeterminer UIDeterminer;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            OnNotMyView.Invoke();
            return;
        }

        if (IsPlayerACop())
        {
            OnPlayerIsCOP.Invoke();
        }
        else
        {
            OnPlayerIsBOB.Invoke();
        }
        
        OnMyPhotonView.Invoke();
        mainCamera = Camera.main;
        UIDeterminer = mainCamera.GetComponent<CameraUIDeterminer>();

        if (UIDeterminer != null)
        {
            if (IsPlayerACop())
            {
                UIDeterminer.OnPlayerIsCOP.Invoke();
            }
            else
            {
                UIDeterminer.OnPlayerIsBOB.Invoke();
            }
        }
    }

    private bool IsPlayerACop()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue))
        {
            return teamValue.ToString() == Team.Cops.ToString();
        }
        return false;
    }
}

