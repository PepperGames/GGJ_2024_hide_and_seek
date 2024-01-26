using UnityEngine;

public class LaughterMeter : MonoBehaviour
{
    public float maxLaughter = 100f;
    public float currentLaughter;
    public float laughterDecreaseRate = 10f; // Уменьшение смеха в секунду
    public float laughterIncreaseRate = 5f;
    
    public bool isUsingLaughter = false; // Добавлено для отслеживания использования способности
    public bool isUncontrollableLaughter = false;
    
    
    void Start()
    {
        currentLaughter = 0;
    }

    void Update()
    {
        if (currentLaughter >= maxLaughter)
        {
            isUncontrollableLaughter = true;
            // Здесь можно вызвать метод для начала неосознанного смеха, если он еще не активен
        }

        if (isUncontrollableLaughter)
        {
            if (currentLaughter > 0)
            {
                currentLaughter -= Time.deltaTime * laughterDecreaseRate;
            }
            else
            {
                isUncontrollableLaughter = false;
                currentLaughter = 0;
                // Здесь можно вызвать метод для остановки неосознанного смеха
            }
        }
        else if (currentLaughter < maxLaughter)
        {
            currentLaughter += Time.deltaTime * laughterIncreaseRate;
            currentLaughter = Mathf.Min(currentLaughter, maxLaughter);
        }
    }

    // Метод для использования смеха
    public bool UseLaughter(float amount)
    {
        if (currentLaughter >= amount)
        {
            currentLaughter -= amount;
            return true; // Успешно использовали смех
        }
        return false; // Недостаточно смеха
    }
}
