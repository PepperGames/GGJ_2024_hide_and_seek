using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class JokeResponse : MonoBehaviour
{
    [SerializeField] private LaughterAbility _laughterAbility;

    public UnityEvent OnStartStun;
    public UnityEvent OnEndStun;

    public void StartJoke(float duration)
    {
        StartCoroutine(UncontrollableLaughter(duration));
    }

    private IEnumerator UncontrollableLaughter(float duration)
    {
        OnStartStun.Invoke();
        _laughterAbility.HandleUncontrollableLaughter();

        yield return new WaitForSeconds(duration);

        _laughterAbility.StopLaughter();
        OnEndStun.Invoke();
    }
}
