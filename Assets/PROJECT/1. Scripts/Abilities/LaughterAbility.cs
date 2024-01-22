using Photon.Pun;
using UnityEngine;

public class LaughterAbility : BaseAbility
{
    public AudioClip laughterSound;
    public AudioSource audioSource;
    private LaughterMeter laughterMeter;

    void Start()
    {
        laughterMeter = GetComponent<LaughterMeter>();
        //audioSource = GetComponent<AudioSource>();
    }

    public override void CheckAbilityUse()
    {
        if (Input.GetMouseButtonDown(1) && laughterMeter.currentLaughter > 0)
        {
            StartLaughter();
        }
        else if (Input.GetMouseButton(1) && laughterMeter.currentLaughter > 0)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            laughterMeter.UseLaughter(Time.deltaTime * laughterMeter.laughterDecreaseRate);
        }
        else if (Input.GetMouseButtonUp(1) || laughterMeter.currentLaughter <= 0)
        {
            StopLaughter();
        }
    }

    private void StartLaughter()
    {
        laughterMeter.isUsingLaughter = true;
        audioSource.loop = true;
        audioSource.PlayOneShot(laughterSound);
        photonView.RPC("PlayLaughterSoundOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber);
    }

    private void StopLaughter()
    {
        laughterMeter.isUsingLaughter = false;
        audioSource.loop = false;
        audioSource.Stop();
        photonView.RPC("StopLaughterSoundOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber);
    }

    [PunRPC]
    void PlayLaughterSoundOnOtherClients(int actorNumber)
    {
        LaughterAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.audioSource.loop = true; // Включаем зацикливание
            if (!ability.audioSource.isPlaying)
            {
                ability.audioSource.clip = ability.laughterSound;
                ability.audioSource.Play(); // Используем Play вместо PlayOneShot
            }
        }
    }

    [PunRPC]
    void StopLaughterSoundOnOtherClients(int actorNumber)
    {
        LaughterAbility ability = FindAbilityByActorNumber(actorNumber);
        if (ability != null)
        {
            ability.audioSource.loop = false; // Отключаем зацикливание
            ability.audioSource.Stop();
        }
    }



    LaughterAbility FindAbilityByActorNumber(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<LaughterAbility>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
}
