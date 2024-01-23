using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CopsLaughterResponse : MonoBehaviourPun
{
    public float minDistanceToReact = 5f;
    public float indicatorDisableDelay;

    private Dictionary<int, Coroutine> activeIndicatorCoroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, Target> activeIndicators = new Dictionary<int, Target>();

    private void Start()
    {
        if (!IsPlayerACop())
        {
            this.enabled = false; // Отключаем скрипт, если локальный игрок не коп
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
                Debug.Log("Cop hear the sound from ActorNumber: " + actorNumber);
                Target indicatorTarget = ability.gameObject.GetComponent<Target>();

                if (indicatorTarget != null)
                {
                    if (activeIndicatorCoroutines.ContainsKey(actorNumber))
                    {
                        // Обновляем время отключения индикатора
                        StopCoroutine(activeIndicatorCoroutines[actorNumber]);
                        activeIndicatorCoroutines[actorNumber] = StartCoroutine(DisableIndicator(actorNumber));
                    }
                    else
                    {
                        // Включаем индикатор и запускаем таймер его отключения
                        indicatorTarget.enabled = true;
                        activeIndicators.Add(actorNumber, indicatorTarget);
                        activeIndicatorCoroutines.Add(actorNumber, StartCoroutine(DisableIndicator(actorNumber)));
                    }
                }
            }
        }
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

    private IEnumerator DisableIndicator(int actorNumber)
    {
        yield return new WaitForSeconds(indicatorDisableDelay);
        if (activeIndicators.TryGetValue(actorNumber, out Target indicatorTarget))
        {
            indicatorTarget.enabled = false;
        }
        activeIndicatorCoroutines.Remove(actorNumber);
        activeIndicators.Remove(actorNumber);
    }
}
