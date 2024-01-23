using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyOffScreenIndicator : MonoBehaviour
{
    // The target to follow
    public Transform target;
    // The camera that renders the scene
    public Camera cam;
    // The image that shows the indicator
    
    public Image image;
    public Image imageToDisable;

    // The position of the indicator on the screen
    
    private Vector2 position;
    // The width and height of the screen
    private float screenWidth;
    private float screenHeight;
    // The rect transform of the image
    private RectTransform rectTransform;
    private Color baseImageColor;

    void Start()
    {
        // Get the rect transform of the image
        rectTransform = image.GetComponent<RectTransform>();

        // Get the width and height of the screen
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        baseImageColor = imageToDisable.color;
    }

    void Update()
    {
        if (target == null)
        {
            baseImageColor.a = 0;
            imageToDisable.color = baseImageColor;
            return;
        }

        // Конвертация мировой позиции цели в позицию на экране
        Vector3 screenPosition = cam.WorldToViewportPoint(target.position);
    
        // Определение, находится ли цель перед камерой
        bool inFront = screenPosition.z > 0;
    
        // Определение, находится ли цель в пределах границ экрана
        bool onScreen = inFront &&
                        screenPosition.x >= 0 && screenPosition.x <= 1 &&
                        screenPosition.y >= 0 && screenPosition.y <= 1;

        // Если цель на экране и перед камерой, скрываем индикатор и возвращаемся
        if (onScreen)
        {
            //baseImageColor.a = 0;
            //imageToDisable.color = baseImageColor;
            //return;
        }

        // Если цель за пределами экрана, показываем индикатор
        baseImageColor.a = 1;
        imageToDisable.color = baseImageColor;

        // Нормализация позиции экрана в диапазоне [-1, 1]
        Vector2 direction = new Vector2(screenPosition.x - 0.5f, screenPosition.y - 0.5f) * 2;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Вычисление позиции индикатора на краю экрана
        Vector2 edgePosition = direction.normalized * Mathf.Min(screenWidth, screenHeight) / 2;
        position = new Vector2(screenWidth / 2, screenHeight / 2) + edgePosition;

        // Установка позиции и вращения индикатора
        rectTransform.position = position;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
