using UnityEngine;

public class HeightController : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    public void SetHeight(float height)
    {
        Vector2 anchoredPosition = rectTransform.anchoredPosition;
        anchoredPosition.y = height;
        rectTransform.anchoredPosition = anchoredPosition;

        //Vector3 vector3 = new Vector3(transform.position.x, height, transform.position.z);
        //transform.position = vector3;
    }
}
