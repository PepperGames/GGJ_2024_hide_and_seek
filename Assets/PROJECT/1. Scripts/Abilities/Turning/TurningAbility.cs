using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class TurningAbility : BaseAbility
{
    [SerializeField] private KeyCode _keyCode = KeyCode.LeftShift;
    [SerializeField] private PropsToTurningManager _propsToTurningManager;

    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private AudioSource _turningAudioSource;

    [SerializeField] private ThirdPersonControllerNew _thirdPersonController;

    private int lastId = -1;

    public UnityEvent OnTurning;

    void Awake()
    {
        _propsToTurningManager = FindObjectOfType<PropsToTurningManager>();
    }

    public override void CheckAbilityUse()
    {
        if (Input.GetKeyDown(_keyCode) && canUse)
        {
            ActivateAbility();
        }
    }

    public override void LocalUseOfAbility()
    {
        int id = _propsToTurningManager.GetRandomPropsId();

        for (int i = 0; i < 3 || id != lastId; i++)
        {
            id = _propsToTurningManager.GetRandomPropsId();

        }
        lastId = id;

        Turning(id);
        OnTurning?.Invoke();

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
        PropsToTurningScriptableObject propsToTurningSO = _propsToTurningManager.GetPropsToTurningSOById(propsId);

        _meshCollider.enabled = true;
        _capsuleCollider.enabled = false;

        _meshCollider.sharedMesh = propsToTurningSO.Mesh;
        _skinnedMeshRenderer.sharedMesh = propsToTurningSO.Mesh;

        _thirdPersonController.Distance = propsToTurningSO.CameraDistance;

        _turningAudioSource.Play();
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
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }

}
