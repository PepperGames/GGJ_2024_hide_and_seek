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

    [SerializeField] private List<GameObject> _whoTouch = new List<GameObject>();

    private bool _isPinned = false;

    [SerializeField] private AudioSource _pinAudioSource;
    [SerializeField] private LayerMask _layersToNotPin;

    [SerializeField] private BaseAbility[] _abilitiesThatWillBeBlocked;

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
        if (CanPin && !_isPinned)
        {
            LocalPin();
            _pinAudioSource.Play();
        }
        else if (_isPinned)
        {
            LocalChipOff();
        }

        Debug.Log("Выполнено локальное действие: " + abilityName);
    }

    public void LocalPin()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        Pin(position, rotation);
        _bobThirdPersonController.ChangeCameraMode(CameraMode.FreelyRotating);

        foreach (var item in _abilitiesThatWillBeBlocked)
        {
            item.BlockUse();
        }

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
        _bobThirdPersonController.ChangeCameraMode(CameraMode.Hard);
        ChipOff();

        foreach (var item in _abilitiesThatWillBeBlocked)
        {
            item.UnblockUse();
        }

        photonView.RPC("ChipOffOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber);
        _isPinned = false;
    }

    public void ChipOff()
    {
        _bobThirdPersonController.EnableMove();
        _bobThirdPersonController.EnableRotateCharacter();
        _rigidbody.isKinematic = false;

        _isPinned = false;
        OnChipOff?.Invoke();
    }

    [PunRPC]
    private void ChipOffOnOtherClients(int actorNumber)
    {
        PinAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.ChipOff();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject targetObject = collision.collider.gameObject;

        if (targetObject.layer == _layersToNotPin)
            return;

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
