using UnityEngine;

public class JokeResponse : MonoBehaviour
{
    [SerializeField] private LaughterAbility _laughterAbility;

    public void StartJoke()
    {
        _laughterAbility.HandleUncontrollableLaughter();
    }
    public void StopJoke()
    {
        _laughterAbility.StopLaughter();
    }
}
