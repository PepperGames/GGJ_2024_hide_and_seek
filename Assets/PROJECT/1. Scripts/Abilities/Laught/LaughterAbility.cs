using System.Collections;
using Photon.Pun;
using UnityEngine;

public class LaughterAbility : BaseAbility
{
    public AudioClip laughterSound;
    public AudioSource audioSource;
    private LaughterMeter laughterMeter;
    private Coroutine laughterBroadcastCoroutine;


    void Start()
    {
        laughterMeter = GetComponent<LaughterMeter>();
        //audioSource = GetComponent<AudioSource>();
    }

    public override void CheckAbilityUse()
    {
        if (laughterMeter.isUncontrollableLaughter)
        {
            // Логика для неосознанного смеха
            HandleUncontrollableLaughter();
        }
        else
        {
            // Обычная логика смеха
            HandleControlledLaughter();
        }
    }

    private void HandleControlledLaughter()
    {
        if (Input.GetMouseButtonDown(1) && laughterMeter.currentLaughter > 0)
        {
            StartLaughter();
        }
        else if (Input.GetMouseButton(1))
        {
            if (laughterMeter.currentLaughter <= 0) //laughterMeter.laughterIncreaseRate
            {
                StopLaughter();
                return; // Выходим из метода, если шкала смеха опустела
            }

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            laughterMeter.UseLaughter(Time.deltaTime * laughterMeter.laughterDecreaseRate);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopLaughter();
        }
    }

    public void HandleUncontrollableLaughter()
    {
        // Проверяем, достигла ли шкала смеха нуля
        if (laughterMeter.currentLaughter <= 0)
        {
            StopLaughter();
            return; // Выходим из метода, так как шкала смеха пуста
        }

        if (!audioSource.isPlaying)
        {
            StartLaughter();
        }
    }


    private void StartLaughter()
    {
        laughterMeter.isUsingLaughter = true;
        audioSource.loop = true;
        audioSource.PlayOneShot(laughterSound);
        photonView.RPC("PlayLaughterSoundOnOtherClients", RpcTarget.Others, photonView.Owner.ActorNumber);
        if (laughterBroadcastCoroutine != null)
        {
            StopCoroutine(laughterBroadcastCoroutine);
            laughterBroadcastCoroutine = null;
        }

        foreach (var cop in FindObjectsOfType<CopsLaughterResponse>())
        {
            cop.photonView.RPC("TryFindLaughterSource", RpcTarget.All, photonView.Owner.ActorNumber);
        }
    }

    private IEnumerator LaughterBroadcastLoop()
    {
        while (laughterMeter.isUsingLaughter)
        {
            foreach (var cop in FindObjectsOfType<CopsLaughterResponse>())
            {
                cop.photonView.RPC("TryFindLaughterSource", RpcTarget.All, photonView.Owner.ActorNumber);
            }
            yield return new WaitForSeconds(ConstantsHolder.LAUGHTER_BROADCAST_TIME); // Задержка между отправками сообщений
        }
    }

    public void StopLaughter()
    {
        laughterMeter.isUsingLaughter = false;
        audioSource.loop = false;
        audioSource.Stop();

        if (laughterBroadcastCoroutine != null)
        {
            StopCoroutine(laughterBroadcastCoroutine);
            laughterBroadcastCoroutine = null;
        }

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
