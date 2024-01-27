using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokeAbility : BaseAbility
{
    [SerializeField] private KeyCode _keyCode = KeyCode.E;

    [SerializeField] private float _range = 15f;

    [SerializeField] private AudioSource _jokeAudioSource;

    [SerializeField] private LaughterAbility[] _laughterAbilitys;

    private void Awake()
    {
        _laughterAbilitys = FindObjectsByType<LaughterAbility>(FindObjectsSortMode.None);
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
        LocalMakeYouLaugh();

        Debug.Log("¬ыполнено локальное действие: " + abilityName);
    }

    public void LocalMakeYouLaugh()
    {
        MakeYouLaugh();

        photonView.RPC("MakeYouLaughOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber);
    }

    public void MakeYouLaugh()
    {
        _jokeAudioSource.Play();
        StartCoroutine(LookBobAndMakeHimLaugh());
    }

    private IEnumerator LookBobAndMakeHimLaugh()
    {
        float step = 0.1f;
        float duration = abilityDuration;
        while (duration > 0)
        {
            foreach (var item in _laughterAbilitys)
            {
                if (Vector3.Distance(transform.position, item.transform.position) <= _range)
                {
                    item.HandleUncontrollableLaughter();
                }
            }
            yield return new WaitForSeconds(step);
            duration -= step;
        }
        _jokeAudioSource.Stop();
    }

    [PunRPC]
    private void MakeYouLaughOnOtherClients(int actorNumber)
    {
        JokeAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.MakeYouLaugh();
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

    public override void OtherPlayersAbilityUse(string playerName, string usedAbility)
    {
        // Ћогика, видима€ другим игрокам дл€ способности "Dash"
        Debug.Log(playerName + " used spell = " + usedAbility + " (other player see this)");
    }
}
