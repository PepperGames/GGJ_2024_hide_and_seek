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
        Debug.Log("sum " + sum);

        int random = Random.Range(0, sum + 1);
        Debug.Log("random " + random);

        int prevSum = 0;
        sum = 0;

        for (int i = 0; i < _props.Count; i++)
        {
            prevSum = sum;
            sum += _props[i].Chance;

            Debug.Log("random+ " + random);
            Debug.Log("prevSum+ " + prevSum);
            Debug.Log("sum+ " + sum);

            if (random > prevSum && random <= sum)
            {
                Debug.Log("i+ " + i);
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
