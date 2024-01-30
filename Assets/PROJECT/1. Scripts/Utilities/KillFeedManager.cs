using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedManager : MonoBehaviour
{
    public GameObject killFeedItemPrefab; // Префаб элемента kill feed
    public Transform killFeedList; // Родительский объект, куда будут помещаться сообщения
    public float messageLifetime = 3f; // Время жизни сообщения

    // Метод для добавления сообщения
    public void AddMessage(string playerThatDied, string couseOfDeath)
    {
        string deathMessage = $"Player {playerThatDied} died due to {couseOfDeath}.";
        GameObject item = Instantiate(killFeedItemPrefab, killFeedList);
        item.GetComponent<TMP_Text>().text = deathMessage;
        StartCoroutine(RemoveMessageAfterTime(item, messageLifetime));
    }

    // Корутина для удаления сообщения
    private IEnumerator RemoveMessageAfterTime(GameObject message, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(message);
    }
}