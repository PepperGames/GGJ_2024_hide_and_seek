using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PinAbility : BaseAbility
{
    [SerializeField] private KeyCode _keyCode = KeyCode.Mouse0;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private BobThirdPersonController _bobThirdPersonController;

    [SerializeField] private bool _isPinned = false;

    //private bool _collisionEnter = false;
    //private bool _collisionStay = false;
    [SerializeField] private List<GameObject> _whoTouch = new List<GameObject>();

    private bool CanPin
    {
        get
        {
            if (_whoTouch.Count > 0)
                return true;
            return false;
        }
    }

    public UnityEvent OnPin;
    public UnityEvent OnChipOff;


    //void Awake()
    //{
    //    _propsToTurningManager = FindObjectOfType<PropsToTurningManager>();
    //}

    public override void CheckAbilityUse()
    {
        if (Input.GetKeyDown(_keyCode))
        {
            ActivateAbility();
        }
    }

    public override void LocalUseOfAbility()
    {
        //int id = _propsToTurningManager.GetRandomPropsId();

        //Turning(id);
        //OnTurning?.Invoke();

        if (CanPin && !_isPinned)
        {
            Pin();
        }
        else if (_isPinned)
        {
            ChipOff();
        }

        Debug.Log("¬ыполнено локальное действие: " + abilityName);

        //photonView.RPC("TurningOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber, id);
    }

    public void Pin()
    {
        _bobThirdPersonController.DisableMove();
        _rigidbody.isKinematic = true;
         _isPinned = true;
    }

    public void ChipOff()
    {
        _bobThirdPersonController.EnableMove();
        _rigidbody.isKinematic = false;
        _isPinned = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject targetObject = collision.collider.gameObject;
        if (!_whoTouch.Contains(targetObject))
        {
            _whoTouch.Add(targetObject);
            Debug.Log("OnCollisionEnter: " + targetObject.name);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject targetObject = collision.collider.gameObject;
        if (_whoTouch.Contains(targetObject))
        {
            _whoTouch.Remove(targetObject);
            Debug.Log("OnCollisionEnter: " + targetObject.name);
        }
    }

    //[PunRPC]
    //void TurningOnOtherClients(int actorNumber, int propsId)
    //{
    //    TurningAbility ability = FindAbilityByActorNumber(actorNumber);
    //    if (ability != null)
    //    {
    //        ability.Turning(propsId);
    //    }
    //}

    //public void Turning(int propsId)
    //{
    //    PropsToTurningScriptableObject propsToTurningSO = _propsToTurningManager.GetPropsToTurningSOById(propsId);

    //    _meshCollider.enabled = true;
    //    _capsuleCollider.enabled = false;

    //    _meshCollider.sharedMesh = propsToTurningSO.Mesh;
    //    _skinnedMeshRenderer.sharedMesh = propsToTurningSO.Mesh;
    //}

    //public override void OtherPlayersAbilityUse(string playerName, string usedAbility)
    //{
    //    // Ћогика, видима€ другим игрокам дл€ способности "Dash"
    //    Debug.Log(playerName + " used spell = " + usedAbility + " (other player see this)");
    //}

    //private TurningAbility FindAbilityByActorNumber(int actorNumber)
    //{
    //    foreach (var player in FindObjectsOfType<TurningAbility>())
    //    {
    //        if (player.photonView.Owner.ActorNumber == actorNumber)
    //        {
    //            return player;
    //        }
    //    }
    //    return null;
    //}

}
