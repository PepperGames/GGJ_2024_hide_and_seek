using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RaycastAbility : BaseAbility
{
    public GameObject markerPrefab; // Префаб маркера
    public float rayLength = 100f; // Длина луча
    private MarkerBehaviour myMarker; // Ссылка на маркер
    private Team playerTeam;

    void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue))
        {
            playerTeam = (Team)Enum.Parse(typeof(Team), teamValue.ToString());
            GameObject markerInstance = PhotonNetwork.Instantiate(markerPrefab.name, Vector3.zero, Quaternion.identity);
            myMarker = markerInstance.GetComponent<MarkerBehaviour>();
        }
    }

    public override void CheckAbilityUse()
    {
        if (Input.GetKey(KeyCode.Mouse2))
        {
            ActivateAbility();
        }
    }

    public override void LocalUseOfAbility()
    {
        if (myMarker != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength))
            {
                myMarker.ActivateMarker(hit.point, playerTeam);
            }
        }

        base.LocalUseOfAbility();
    }
}


