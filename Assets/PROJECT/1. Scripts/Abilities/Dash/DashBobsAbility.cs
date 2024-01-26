using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DashBobsAbility : BaseAbility
{
    public KeyCode dashKeyCode = KeyCode.LeftShift;

    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;

    [SerializeField] private Transform _raycastTransform;
    public float dashRaycastLength = 1f;
    public LayerMask forbiddenlayers;

    private Rigidbody playerRigidbody;

    public UnityEvent OnStartDash;
    public UnityEvent OnEndDash;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public override void CheckAbilityUse()
    {
        if (Input.GetKeyDown(dashKeyCode) && canUse)
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


    private IEnumerator Dash()
    {
        OnStartDash.Invoke();

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            // Проверка на столкновение с помощью рейкаста
            if (Physics.Raycast(_raycastTransform.position, transform.forward, dashRaycastLength, forbiddenlayers))
            {
                Debug.Log("Dash interrupted due to collision");
                break; // Прерываем дэш при столкновении
            }

            playerRigidbody.MovePosition(playerRigidbody.position + transform.forward * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        OnEndDash.Invoke();
    }

    public override void OtherPlayersAbilityUse(string playerName, string usedAbility)
    {
        // Логика, видимая другим игрокам для способности "Dash"
        Debug.Log(playerName + " used spell = " + usedAbility + " (other player see this)");
    }
}