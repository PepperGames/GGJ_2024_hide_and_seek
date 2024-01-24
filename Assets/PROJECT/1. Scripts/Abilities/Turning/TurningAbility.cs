using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningAbility : BaseAbility
{
    [SerializeField] private KeyCode _keyCode = KeyCode.LeftShift;
    [SerializeField] private PropsToTurningManager _propsToTurningManager;

    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    [SerializeField] private CapsuleCollider _capsuleCollider;

    void Awake()
    {
        _propsToTurningManager = FindObjectOfType<PropsToTurningManager>();
    }

    public override void CheckAbilityUse()
    {
        if (Input.GetKeyDown(_keyCode))
        {
            ActivateAbility();
        }
    }

    public override void LocalUseOfAbility()
    {
        int id = _propsToTurningManager.GetRandomPropsId();

        Turning(id);
        Debug.Log("¬ыполнено локальное действие: " + abilityName);

        photonView.RPC("TurningOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber, id);

    }

    [PunRPC]
    void TurningOnOtherClients(int actorNumber, int propsId)
    {
        TurningAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.Turning(propsId);
        }
    }

    public void Turning(int propsId)
    {
        Debug.Log("Turning1  propsId=" + propsId);
        PropsToTurningScriptableObject propsToTurningSO = _propsToTurningManager.GetPropsToTurningSOById(propsId);
        Debug.Log("propsToTurningSO " + propsToTurningSO.name);

        _meshCollider.enabled = true;
        _capsuleCollider.enabled = false;

        _meshCollider.sharedMesh = propsToTurningSO.Mesh;
        _skinnedMeshRenderer.sharedMesh = propsToTurningSO.Mesh;
        Debug.Log("Turning1  propsId=" + propsId);
    }

    public override void OtherPlayersAbilityUse(string playerName, string usedAbility)
    {
        // Ћогика, видима€ другим игрокам дл€ способности "Dash"
        Debug.Log(playerName + " used spell = " + usedAbility + " (other player see this)");
    }

    private TurningAbility FindAbilityByActorNumber(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<TurningAbility>())
        {
            Debug.Log("FindAbilityByActorNumber " + player.gameObject.name);
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                Debug.Log("actorNumber " + actorNumber);
                return player;
            }
        }
        return null;
    }

}
