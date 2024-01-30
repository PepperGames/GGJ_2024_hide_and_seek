using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class MarkerBehaviour : MonoBehaviourPun
{
    public float activeDuration = 5f; // Время активности маркера
    public UnityEvent OnEnemyMarkerActivated;
    public UnityEvent OnNormalMarkerActivated;
    public UnityEvent OnMarkerDeactivated;

    private Coroutine activeCoroutine;

    public void ActivateMarker(Vector3 position, Team team)
    {
        transform.position = position;

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(DeactivateMarker());

        if (team == Team.Bobs)
        {
            // Проверяем, выделен ли вражеский игрок
            if (IsEnemyPlayerMarked(position, team))
            {
                photonView.RPC("ActivateEnemyMarkerOnAllClients", RpcTarget.AllBuffered, position, team.ToString());
            }
            else
            {
                photonView.RPC("ActivateNormalMarkerOnAllClients", RpcTarget.AllBuffered, position, team.ToString());
            }
        }
        else
        {
            photonView.RPC("ActivateNormalMarkerOnAllClients", RpcTarget.AllBuffered, position, team.ToString());
        }
    }

    private bool IsEnemyPlayerMarked(Vector3 position, Team team)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (team == Team.Bobs && hitCollider.gameObject.GetComponent<CopPunchAbility>() != null)
            {
                return true;
            }
            else if (team == Team.Cops && hitCollider.gameObject.GetComponent<StunAbility>() != null)
            {
                return true;
            }
        }
        return false;
    }

    [PunRPC]
    void ActivateEnemyMarkerOnAllClients(Vector3 position, string team)
    {
        gameObject.SetActive(ShouldBeVisible(team));
        transform.position = position;
        OnEnemyMarkerActivated?.Invoke();
    }

    [PunRPC]
    void ActivateNormalMarkerOnAllClients(Vector3 position, string team)
    {
        gameObject.SetActive(ShouldBeVisible(team));
        transform.position = position;
        OnNormalMarkerActivated?.Invoke();
    }

    private bool ShouldBeVisible(string team)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object localTeam))
        {
            return localTeam.ToString() == team;
        }

        return false;
    }

    IEnumerator DeactivateMarker()
    {
        yield return new WaitForSeconds(activeDuration);
        photonView.RPC("DeactivateMarkerOnAllClients", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DeactivateMarkerOnAllClients()
    {
        gameObject.SetActive(false);
        OnMarkerDeactivated?.Invoke();
    }
}





