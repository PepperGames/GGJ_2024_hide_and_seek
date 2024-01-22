using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CopsLaughterResponse : MonoBehaviourPun
{
    public float minDistanceToReact = 5f;
    public float indicatorDisableDelay;

    private Target indicatorTarget;

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
                indicatorTarget = ability.gameObject.GetComponent<Target>();
                indicatorTarget.enabled = true;
                StartCoroutine(DisableIndicator());
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

    private IEnumerator DisableIndicator()
    {
        yield return new WaitForSeconds(indicatorDisableDelay);
        indicatorTarget.enabled = false;
        indicatorTarget = null;
    }
}
