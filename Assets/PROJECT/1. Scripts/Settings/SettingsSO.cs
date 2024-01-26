using UnityEngine;

[CreateAssetMenu(fileName = "SettingsSO", menuName = "ScriptableObjects/SettingsSO", order = 1)]
public class SettingsSO : ScriptableObject
{
    [SerializeField] private float _remainingTime;

    public float RemainingTime => _remainingTime;
}

