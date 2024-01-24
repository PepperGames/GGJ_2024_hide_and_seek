using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BaseAbility : MonoBehaviourPun
{
    public string abilityName = "BaseSpell";
    public float cooldown = 5f; // Время перезарядки в секундах
    public float abilityDuration = 1f;
    public UnityEvent OnAbilityLocalStart;
    public UnityEvent OnAbilityLocalEnd;
    private bool isCooldown = false;

    void Update()
    {
        if (photonView.IsMine && !isCooldown)
        {
            CheckAbilityUse();
        }
    }

    public virtual void CheckAbilityUse()
    {
        //Проверка на использование способности, типо кнопки или ПКМ, ЛКМ
        //Затем вызов ActivateAbility();
    }

    public void ActivateAbility()
    {
        // Локальное действие способности
        LocalUseOfAbility();

        // Отправка уведомления и активация действия у других игроков
        photonView.RPC("OtherPlayersAbilityUseRPC", RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName, abilityName);

        // Начало перезарядки
        StartCoroutine(Cooldown());
        StartCoroutine(AbilityDuration());
    }

    IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }
    
    IEnumerator AbilityDuration()
    {
        OnAbilityLocalStart.Invoke();
        yield return new WaitForSeconds(abilityDuration);
        OnAbilityLocalEnd.Invoke();
    }

    public virtual void LocalUseOfAbility()
    {
        // Переопределите этот метод в производных классах для реализации локального действия способности
        Debug.Log("Local use of ability with name: " + abilityName);
    }

    [PunRPC]
    void OtherPlayersAbilityUseRPC(string playerName, string usedAbility)
    {
        OtherPlayersAbilityUse(playerName, usedAbility);
    }

    public virtual void OtherPlayersAbilityUse(string playerName, string usedAbility)
    {
        // Переопределите этот метод в производных классах для реализации действия способности, видимого другим игрокам
        Debug.Log("Player: " + playerName + " used spell: " + usedAbility + " on SERVER, (other players see this)");
    }
}