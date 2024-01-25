using UnityEngine;
using System.Collections;

public class CopDashAbility : BaseAbility
{
    public KeyCode dashKeyCode = KeyCode.LeftShift;
    public float dashSpeed = 10f; // Скорость дэша
    public float dashDuration = 0.5f; // Продолжительность дэша в секундах
    public float dashRaycastLength = 1f; // Длина рейкаста

    private Rigidbody playerRigidbody;
    private bool isDashing;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public override void CheckAbilityUse()
    {
        if (Input.GetKeyDown(dashKeyCode) && !isDashing)
        {
            ActivateAbility();
        }
    }

    public override void LocalUseOfAbility()
    {
        if (playerRigidbody != null)
        {
            StartCoroutine(Dash());
        }
        else
        {
            Debug.LogError("Rigidbody not found on the player for CopDashAbility");
        }

        Debug.Log("Выполнено локальное действие: " + abilityName);
    }

    IEnumerator Dash()
    {
        isDashing = true;

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            // Проверка на столкновение с помощью рейкаста
            if (Physics.Raycast(transform.position, transform.forward, dashRaycastLength))
            {
                Debug.Log("Dash interrupted due to collision");
                break; // Прерываем дэш при столкновении
            }

            playerRigidbody.MovePosition(playerRigidbody.position + transform.forward * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    public override void OtherPlayersAbilityUse(string playerName, string usedAbility)
    {
        Debug.Log(playerName + " used spell = " + usedAbility + " (other players see this)");
    }
}