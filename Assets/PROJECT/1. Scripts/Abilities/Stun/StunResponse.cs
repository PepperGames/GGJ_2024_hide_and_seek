using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StunResponse : MonoBehaviour
{
    [SerializeField] private ThirdPersonControllerNew _thirdPersonController;
    [SerializeField] private BaseAbility[] _baseAbilities;

    public UnityEvent OnStartStun;
    public UnityEvent OnEndStun;

    public void GetStan(float duration)
    {
        StartCoroutine(StandStill(duration));
    }

    private IEnumerator StandStill(float duration)
    {
        OnStartStun.Invoke();
        _thirdPersonController.enabled = false;
        foreach (var item in _baseAbilities)
        {
            item.enabled = false;
        }
        yield return new WaitForSeconds(duration);
        _thirdPersonController.enabled = true;
        foreach (var item in _baseAbilities)
        {
            item.enabled = true;
        }
        OnEndStun.Invoke();
    }
}
