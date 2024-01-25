using Photon.Pun;
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
            LocalPin();
        }
        else if (_isPinned)
        {
            LocalChipOff();
        }

        Debug.Log("¬ыполнено локальное действие: " + abilityName);

        //photonView.RPC("TurningOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber, id);
    }

    public void LocalPin()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        Pin(position, rotation);
        _bobThirdPersonController.ChangeCameraMode(CameraMode.FreelyRotating);

        photonView.RPC("PinOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber, position, rotation);
        _isPinned = true;
    }

    public void Pin(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        _bobThirdPersonController.DisableMove();
        _bobThirdPersonController.DisableRotateCharacter();
        _rigidbody.isKinematic = true;

        _isPinned = true;
        OnPin?.Invoke();
    }

    [PunRPC]
    private void PinOnOtherClients(int actorNumber, Vector3 position, Quaternion rotation)
    {
        PinAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.Pin(position, rotation);
        }
    }

    public void LocalChipOff()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        _bobThirdPersonController.ChangeCameraMode(CameraMode.Hard);
        ChipOff(position, rotation);

        photonView.RPC("ChipOffOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber, position, rotation);
        _isPinned = false;
    }

    public void ChipOff(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        _bobThirdPersonController.EnableMove();
        _bobThirdPersonController.EnableRotateCharacter();
        _rigidbody.isKinematic = false;

        _isPinned = false;
        OnChipOff?.Invoke();
    }

    [PunRPC]
    private void ChipOffOnOtherClients(int actorNumber, Vector3 position, Quaternion rotation)
    {
        PinAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.ChipOff(position, rotation);
        }
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

    private PinAbility FindAbilityByActorNumber(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<PinAbility>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
}
