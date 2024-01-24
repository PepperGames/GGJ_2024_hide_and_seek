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
    }

    public void Turning(int propsId)
    {
        PropsToTurningScriptableObject propsToTurningSO = _propsToTurningManager.GetPropsToTurningSOById(propsId);

        _capsuleCollider.enabled = false;
        _meshCollider.enabled = true;

        _meshCollider.sharedMesh = propsToTurningSO.Mesh;
        _skinnedMeshRenderer.sharedMesh = propsToTurningSO.Mesh;
    }

    public override void OtherPlayersAbilityUse(string playerName, string usedAbility)
    {
        // Ћогика, видима€ другим игрокам дл€ способности "Dash"
        Debug.Log(playerName + " used spell = " + usedAbility + " (other player see this)");
    }
}
