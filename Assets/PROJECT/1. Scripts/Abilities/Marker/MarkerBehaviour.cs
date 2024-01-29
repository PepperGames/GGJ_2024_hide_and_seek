using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class MarkerBehaviour : MonoBehaviourPun
{
    public float activeDuration = 5f; // Время активности маркера
    public UnityEvent OnMarkerActivated;
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
        photonView.RPC("ActivateMarkerOnAllClients", RpcTarget.AllBuffered, position, team.ToString());
    }

    [PunRPC]
    void ActivateMarkerOnAllClients(Vector3 position, string team)
    {
        gameObject.SetActive(ShouldBeVisible(team));
        transform.position = position;
        OnMarkerActivated?.Invoke();
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





