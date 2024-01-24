using System.Collections.Generic;
using UnityEngine;

public class PropsToTurningManager : MonoBehaviour
{
    [SerializeField] private List<PropsToTurningScriptableObject> _props;
    public int PropsCount => _props.Count;

    public int GetRandomPropsId()
    {
        int sum = 0;
        foreach (PropsToTurningScriptableObject props in _props)
        {
            sum += props.Chance;
        }

        int random = Random.Range(0, sum + 1);

        int prevSum = 0;
        sum = 0;

        for (int i = 0; i < _props.Count; i++)
        {
            prevSum = sum;
            sum += _props[i].Chance;

            if (random > prevSum && random <= sum)
            {
                return i;
            }
        }

        Debug.LogError("Didn't give props id");
        return -1;
    }

    public PropsToTurningScriptableObject GetPropsToTurningSOById(int id)
    {
        return _props[id];
    }
}
