using UnityEngine;

[CreateAssetMenu(fileName = "PropsToTurning", menuName = "ScriptableObjects/PropsToTurning", order = 1)]
public class PropsToTurningScriptableObject : ScriptableObject
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private int _chance = 50;

    public Mesh Mesh => _mesh;
    public int Chance => _chance;
}
