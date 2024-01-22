using UnityEngine;

public class LaughterMeter : MonoBehaviour
{
    public float maxLaughter = 100f;
    public float currentLaughter;
    public float laughterDecreaseRate = 10f; // Уменьшение смеха в секунду
    public float laughterIncreaseRate = 5f;
    
    public bool isUsingLaughter = false; // Добавлено для отслеживания использования способности
    
    
    void Start()
    {
        currentLaughter = maxLaughter;
    }

    void Update()
    {
        if (!isUsingLaughter && currentLaughter < maxLaughter)
        {
            currentLaughter += Time.deltaTime * laughterIncreaseRate;
            currentLaughter = Mathf.Min(currentLaughter, maxLaughter);
        }

        // Округление до 0, если значение слишком мало
        if (currentLaughter < 0.01f)
        {
            currentLaughter = 0;
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
