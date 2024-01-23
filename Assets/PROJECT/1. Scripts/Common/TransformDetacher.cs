using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformDetacher : MonoBehaviour
{
    public void SetNullParent()
    {
        transform.parent = null;
    }
}
