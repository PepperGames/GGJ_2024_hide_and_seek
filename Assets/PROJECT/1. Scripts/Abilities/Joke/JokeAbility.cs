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
        Joke();

        photonView.RPC("JokeOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber);
    }

    public void Joke()
    {
        _jokeAudioSource.Play();
        StartCoroutine(JokeLoop());
        OnUseJoke?.Invoke();
    }

    private IEnumerator JokeLoop()
    {
        float step = 0.1f;
        float duration = abilityDuration;

        while (duration > 0)
        {
            foreach (var item in _jokeResponse)
            {
                if (Vector3.Distance(transform.position, item.transform.position) <= _range)
                {
                    item.StartJoke();
                }
            }
            duration -= step;
            yield return new WaitForSeconds(step);
        }
        foreach (var item in _jokeResponse)
        {
            item.StopJoke();
        }
    }


    [PunRPC]
    private void JokeOnOtherClients(int actorNumber)
    {
        JokeAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.Joke();
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
