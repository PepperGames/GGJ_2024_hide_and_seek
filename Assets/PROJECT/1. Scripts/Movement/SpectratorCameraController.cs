using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpectratorCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera myCamera;
    public Transform targetToFollow;

    private int parentInstanceID;
    private Team myTeam;
    private List<Transform> potentialTargets;
    private int currentTargetIndex = 0; // Текущий индекс в списке целей
    
    private void Start()
    {
        DetermineTeam();
        StartSpectrate();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Проверка на нажатие ЛКМ
        {
            SwitchTarget();
        }
    }
    
    private void SwitchTarget()
    {
        if (potentialTargets == null || potentialTargets.Count == 0 || potentialTargets.Count == 1)
        {
            return;
        }

        // Увеличиваем индекс текущей цели
        currentTargetIndex++;
        if (currentTargetIndex >= potentialTargets.Count)
        {
            currentTargetIndex = 0;
        }

        // Обновляем цель камеры
        Transform newTarget = potentialTargets[currentTargetIndex];
        myCamera.Follow = newTarget;
        myCamera.LookAt = newTarget;
    }

    private void DetermineTeam()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue))
        {
            myTeam = (Team)System.Enum.Parse(typeof(Team), teamValue.ToString());
        }
    }

    public void Initialize(int _parentID)
    {
        parentInstanceID = _parentID;
    }

    public void StartSpectrate()
    {
        CinemachineVirtualCamera[] cameras = FindObjectsOfType<CinemachineVirtualCamera>();
        CinemachineVirtualCamera oldCamera = null;
        foreach (var cam in cameras)
        {
            if (cam.gameObject.GetInstanceID() != GetInstanceID() && cam.GetComponent<SpectratorCameraController>() == null)
            {
                oldCamera = cam;
                break;
            }
        }

        if (oldCamera != null)
        {
            oldCamera.gameObject.SetActive(false);
        }

        InitializeSpectrators();

        myCamera.enabled = true;
    }

    public void InitializeSpectrators()
    {
        potentialTargets = new List<Transform>();
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue) &&
                player.CustomProperties.TryGetValue(ConstantsHolder.LIVE_STATUS_PARAM_NAME, out object lifeStatus) &&
                lifeStatus.ToString().Equals(ConstantsHolder.LIVE_STATUS_LIVE_PARAM_NAME) &&
                teamValue.ToString() == myTeam.ToString())
            {
                Debug.Log("FIRST part is done!"); 

                if (myTeam.ToString().Equals("Bobs"))
                {
                    LaughterAbility ally = FindBob(player.ActorNumber);
                    
                    potentialTargets.Add(ally.transform);
                }
                else if(myTeam.ToString().Equals("Cops"))
                {
                    CopsLaughterResponse ally = FindCop(player.ActorNumber);
                    
                    potentialTargets.Add(ally.transform);
                }
            }
        }

        if (potentialTargets.Count > 0)
        {
            currentTargetIndex = Random.Range(0, potentialTargets.Count);
            Transform target = potentialTargets[currentTargetIndex];
            myCamera.Follow = target;
            myCamera.LookAt = target;
        }
        else
        {
            myCamera.Follow = null;
            myCamera.LookAt = null;
            Debug.Log("No alive teammates found");
        }
    }
    
    
    LaughterAbility FindBob(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<LaughterAbility>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
    
    CopsLaughterResponse FindCop(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<CopsLaughterResponse>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
}
