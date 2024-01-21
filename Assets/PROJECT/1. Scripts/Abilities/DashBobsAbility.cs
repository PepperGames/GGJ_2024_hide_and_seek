using UnityEngine;

public class DashBobsAbility : BaseAbility
{
    public float dashForce = 500f; // Сила толчка

    private Rigidbody playerRigidbody;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public override void LocalUseOfAbility()
    {
        // Локальное действие для способности "Dash"
        if (playerRigidbody != null)
        {
            Vector3 dashDirection = transform.forward; // Вперед относительно направления игрока
            playerRigidbody.AddForce(dashDirection * dashForce); // Применяем силу для толчка
        }
        else
        {
            Debug.LogError("Rigidbody not found on the player for DashBobsAbility");
        }

        Debug.Log("Выполнено локальное действие: " + abilityName);
    }

    public override void OtherPlayersAbilityUse(string playerName, string usedAbility)
    {
        // Логика, видимая другим игрокам для способности "Dash"
        Debug.Log(playerName + " used spell = " + usedAbility +" (other player see this)");
    }
}