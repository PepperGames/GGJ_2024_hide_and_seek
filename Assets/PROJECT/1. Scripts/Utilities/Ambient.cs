using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambient : MonoBehaviour
{
    void Start()
    {
        Ambient[] ambients = FindObjectsByType<Ambient>(FindObjectsSortMode.None);
        if (ambients.Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
