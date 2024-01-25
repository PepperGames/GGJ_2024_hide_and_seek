using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class StunAbility : BaseAbility
{
    [SerializeField] private KeyCode _keyCode = KeyCode.E;

    [SerializeField] private float _range = 5f;

    [SerializeField] private AudioSource _stunAudioSource;
    private StunResponse[] _stunResponses;

    public UnityEvent OnPin;

    private void Awake()
    {
        _stunResponses = FindObjectsByType<StunResponse>(FindObjectsSortMode.None);
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
        LocalStun();

        Debug.Log("Выполнено локальное действие: " + abilityName);
    }

    public void LocalStun()
    {
        Stun();

        photonView.RPC("StunOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber);
    }

    public void Stun()
    {
        foreach (var item in _stunResponses)
        {
            if (Vector3.Distance(transform.position, item.transform.position) <= _range)
            {
                item.GetStan(abilityDuration);
            }
        }
        OnPin?.Invoke();
    }

    [PunRPC]
    private void StunOnOtherClients(int actorNumber)
    {
        StunAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.Stun();
        }
    }

    private StunAbility FindAbilityByActorNumber(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<StunAbility>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
}
