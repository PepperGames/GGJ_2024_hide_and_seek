using UnityEngine;
using Photon.Pun;

public class DeathTrigger : MonoBehaviour
{
    [System.Flags]
    public enum Team
    {
        None = 0,
        Cops = 1 << 0,
        Bobs = 1 << 1
    }

    public Team deathTargets; // Выбор команд, которые будут реагировать на триггер

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(ConstantsHolder.PLAYERS_TAG)) // Проверяем, что вошел игрок
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                if (photonView.Owner.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue))
                {
                    Team playerTeam = (Team)System.Enum.Parse(typeof(Team), teamValue.ToString());
                    if ((deathTargets & playerTeam) != 0) // Проверяем, соответствует ли команда игрока одной из целевых команд
                    {
                        Debug.Log("Player from team " + playerTeam + " entered the death trigger.");
                        // Здесь можно добавить дополнительную логику обработки события
                        other.GetComponent<DeathManager>().HandleDeath();
                    }
                }
            }
        }
    }
}