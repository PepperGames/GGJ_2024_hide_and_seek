using UnityEngine;

public class HeightController : MonoBehaviour
{
    public void SetHeight(float height)
    {
        Vector3 vector3 = new Vector3(transform.position.x, height, transform.position.z);
        transform.position = vector3;
    }
}
