using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CopsLaughterResponse : MonoBehaviourPun
{
    public float minDistanceToReact = 5f;
    public float indicatorDisableDelay;

    private List<MyOffScreenIndicator> availableIndicators;
    private Dictionary<int, MyOffScreenIndicator> activeIndicators = new Dictionary<int, MyOffScreenIndicator>();

    private void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false; 
        }
        if (!IsPlayerACop())
        {
            enabled = false;
        }
        if(IsPlayerACop() && photonView.IsMine)
        {
            // Находим и запоминаем все доступные индикаторы
            availableIndicators = new List<MyOffScreenIndicator>(FindObjectsOfType<MyOffScreenIndicator>());
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

    [PunRPC]
    void TryFindLaughterSource(int actorNumber)
    {
        LaughterAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            float distanceToSource = Vector3.Distance(transform.position, ability.transform.position);
            if (distanceToSource <= minDistanceToReact)
            {
                Debug.Log("Cop hear sound from actor: " + actorNumber);
                // Проверяем, есть ли уже активный индикатор для этого источника смеха
                if (!activeIndicators.ContainsKey(actorNumber))
                {
                    // Находим свободный индикатор и активируем его
                    MyOffScreenIndicator indicator = GetAvailableIndicator();
                    if (indicator != null)
                    {
                        indicator.target = ability.transform; // Назначаем цель индикатора
                        Debug.Log("Indicator set for actor: " + actorNumber);
                        activeIndicators.Add(actorNumber, indicator); // Добавляем в активные
                        StartCoroutine(DisableIndicator(actorNumber, indicator)); // Запускаем таймер отключения
                    }
                }
            }
        }
    }
    
    private MyOffScreenIndicator GetAvailableIndicator()
    {
        if (availableIndicators != null && availableIndicators.Count > 0)
        {
            // Ищем индикатор, у которого target не установлен (null)
            return availableIndicators.Find(indicator => indicator.target == null);
        }

        return null;
    }

    LaughterAbility FindAbilityByActorNumber(int actorNumber)
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

    private IEnumerator DisableIndicator(int actorNumber, MyOffScreenIndicator indicator)
    {
        yield return new WaitForSeconds(indicatorDisableDelay);
        indicator.target = null; // Сбрасываем цель индикатора
        activeIndicators.Remove(actorNumber); // Удаляем из активных
    }
}
