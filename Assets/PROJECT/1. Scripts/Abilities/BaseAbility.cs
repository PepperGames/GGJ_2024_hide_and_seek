using System;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public interface IAbility
{
    public float GetCurrentCooldown();
    public bool GetCooldownBool();
    public float GetMaxCooldown();
}

public class BaseAbility : MonoBehaviourPun, IAbility
{
    public string abilityName = "BaseSpell";
    public float cooldown = 5f; // Время перезарядки в секундах
    public float abilityDuration = 1f;

    private bool isCooldown = false;
    protected bool canUse = true; //флажек который блочит использование по нажатию клавиши

    private float currentCooldown = 0;

    public UnityEvent OnAbilityLocalStart;
    public UnityEvent OnAbilityLocalEnd;

    public UnityEvent OnBlockUse;
    public UnityEvent OnUnblockUse;

    private void Start()
    {
        InitializeDisplay();
    }

    void Update()
    {
        if (photonView.IsMine && !isCooldown)
        {
            CheckAbilityUse();
        }

        if (isCooldown)
        {
            currentCooldown += Time.deltaTime;
            if (currentCooldown >= cooldown)
            {
                currentCooldown = 0;
                isCooldown = false;
            }
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

        isCooldown = true;
        // Начало перезарядки
        StartCoroutine(AbilityDuration());
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

    private void InitializeDisplay()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        AbilitiesCooldownDisplay[] cooldownDisplays = FindObjectsOfType<AbilitiesCooldownDisplay>();
        foreach (var display in cooldownDisplays)
        {
            if (abilityName.Equals(display.myAbilityName))
            {
                display.myAbility = this;
                AbilityBlockDisplay abilityBlockDisplay = display.gameObject.GetComponent<AbilityBlockDisplay>();
                if (abilityBlockDisplay != null)
                {
                    abilityBlockDisplay.baseAbility = this;
                }
                break;
            }
        }
    }

    public float GetCurrentCooldown()
    {
        return currentCooldown;
    }

    public bool GetCooldownBool()
    {
        return isCooldown;
    }

    public float GetMaxCooldown()
    {
        return cooldown;
    }

    public void BlockUse()
    {
        canUse = false;
        OnBlockUse?.Invoke();
    }

    public void UnblockUse()
    {
        canUse = true;
        OnUnblockUse?.Invoke();
    }
}