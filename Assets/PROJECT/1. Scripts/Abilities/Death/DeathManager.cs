using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class DeathManager : MonoBehaviourPun
{
    public UnityEvent OnDeathBaseEvent;
    
    public void HandleDeath()
    {
        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable currentProps = PhotonNetwork.LocalPlayer.CustomProperties;
            
            currentProps[ConstantsHolder.LIVE_STATUS_PARAM_NAME] = ConstantsHolder.LIVE_STATUS_DEAD_PARAM_NAME;
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(currentProps);
        }
        
        photonView.RPC("OnDeathRPC", RpcTarget.All, photonView.Owner.ActorNumber);
    }
    
    [PunRPC]
    void OnDeathRPC(int actorNumber)
    {
        DeathManager playerThatDied = FindDeadPlayerByActorNumber(actorNumber);
        if (playerThatDied != null)
        {
            playerThatDied.VisualizeDeath();
        }
    }

    public void VisualizeDeath()
    {
        OnDeathBaseEvent.Invoke();
    }
    
    DeathManager FindDeadPlayerByActorNumber(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<DeathManager>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
}
