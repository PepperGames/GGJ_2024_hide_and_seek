using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPUNInfos : MonoBehaviourPun
{
    public UnityEvent OnNotMyView;
    public UnityEvent OnPlayerIsBOB;
    public UnityEvent OnPlayerIsCOP;
    public UnityEvent OnMyPhotonView;
    public UnityEvent OnPlayerIsTeammate;
    public UnityEvent OnPlayerIsEnemy;
    public Camera mainCamera;
    public CameraUIDeterminer UIDeterminer;
    public GameObject playerMarker; // Маркер игрока

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            OnNotMyView.Invoke();
            SetMarkerVisibility(false); // Скрываем маркер для других игроков
            return;
        }

        DetermineAndSyncTeam();
        
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

        SetMarkerVisibility(false); // Скрываем маркер для самого игрока
        UpdateMarkerVisibilityForAll();
    }

    private void DetermineAndSyncTeam()
    {
        bool isCop = IsPlayerACop();
        if (isCop)
        {
            OnPlayerIsCOP.Invoke();
        }
        else
        {
            OnPlayerIsBOB.Invoke();
        }

        // Синхронизация команды с другими клиентами
        photonView.RPC("SyncTeam", RpcTarget.Others, isCop);
    }

    [PunRPC]
    private void SyncTeam(bool isCop)
    {
        if (isCop)
        {
            OnPlayerIsCOP.Invoke();
        }
        else
        {
            OnPlayerIsBOB.Invoke();
        }
        UpdateMarkerVisibilityForAll();
    }

    private void SetMarkerVisibility(bool isVisible)
    {
        if (playerMarker != null)
        {
            playerMarker.SetActive(isVisible);
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
    
    private void UpdateMarkerVisibilityForAll()
    {
        foreach (var playerInfo in FindObjectsOfType<PlayerPUNInfos>())
        {
            // Проверяем, находятся ли игроки в одной команде
            if (IsPlayerInMyTeam(playerInfo))
            {
                // Включаем маркер для союзников
                OnPlayerIsTeammate.Invoke();
                playerInfo.SetMarkerVisibility(true);
            }
            else
            {
                OnPlayerIsEnemy.Invoke();
                // Выключаем маркер для игроков другой команды
                playerInfo.SetMarkerVisibility(false);
            }
        }
    }

    private bool IsPlayerInMyTeam(PlayerPUNInfos playerInfo)
    {
        return playerInfo.photonView.Owner.CustomProperties[ConstantsHolder.TEAM_PARAM_NAME].ToString() == PhotonNetwork.LocalPlayer.CustomProperties[ConstantsHolder.TEAM_PARAM_NAME].ToString();
    }
}
