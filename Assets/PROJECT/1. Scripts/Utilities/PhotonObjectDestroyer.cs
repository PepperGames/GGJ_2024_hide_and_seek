using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PhotonObjectDestroyer : MonoBehaviourPun
{
    public void DestroyObjectPhoton()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
