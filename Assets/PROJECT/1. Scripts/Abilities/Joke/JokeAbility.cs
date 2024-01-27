using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class JokeAbility : BaseAbility
{
    [SerializeField] private KeyCode _keyCode = KeyCode.E;

    [SerializeField] private float _range = 7f;

    [SerializeField] private AudioSource _jokeAudioSource;

    private JokeResponse[] _jokeResponse;

    public UnityEvent OnUseJoke;

    private void Awake()
    {
        _jokeResponse = FindObjectsByType<JokeResponse>(FindObjectsSortMode.None);
        _jokeAudioSource.maxDistance = _range;
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
        LocalJoke();

        Debug.Log("Выполнено локальное действие: " + abilityName);
    }

    public void LocalJoke()
    {
        TellAJoke();

        photonView.RPC("JokeOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber);
    }

    public void TellAJoke()
    {
        float d = _jokeAudioSource.clip.length;
        Debug.Log("_jokeAudioSource.clip.length " + _jokeAudioSource.clip.length);
        _jokeAudioSource.loop = false;
        _jokeAudioSource.Play();
        StartCoroutine(MakeYouLaugh(d));
        OnUseJoke?.Invoke();
    }

    private IEnumerator MakeYouLaugh(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var item in _jokeResponse)
        {
            if (Vector3.Distance(transform.position, item.transform.position) <= _range)
            {
                item.StartJoke(abilityDuration);
            }
        }
    }

    [PunRPC]
    private void JokeOnOtherClients(int actorNumber)
    {
        JokeAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.TellAJoke();
        }
    }

    private JokeAbility FindAbilityByActorNumber(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<JokeAbility>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
}
