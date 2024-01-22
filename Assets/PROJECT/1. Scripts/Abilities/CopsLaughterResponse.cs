using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CopsLaughterResponse : MonoBehaviourPun
{
    public float minDistanceToReact = 5f;
    
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
}
