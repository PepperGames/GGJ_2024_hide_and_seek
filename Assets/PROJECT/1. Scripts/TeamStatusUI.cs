using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamStatusUI : MonoBehaviourPunCallbacks
{
    public TMP_Text copsAliveCountText;
    public TMP_Text bobsAliveCountText;

    private void Start()
    {
        UpdateTeamCounts();
    }

    private void UpdateTeamCounts()
    {
        int copsAliveCount = 0;
        int bobsAliveCount = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue) &&
                player.CustomProperties.TryGetValue(ConstantsHolder.LIVE_STATUS_PARAM_NAME, out object lifeStatus))
            {
                if (ConstantsHolder.LIVE_STATUS_LIVE_PARAM_NAME.Equals(lifeStatus.ToString()))
                {
                    if (teamValue.ToString() == Team.Cops.ToString())
                    {
                        copsAliveCount++;
                    }
                    else if (teamValue.ToString() == Team.Bobs.ToString())
                    {
                        bobsAliveCount++;
                    }
                }
            }
        }

        copsAliveCountText.text = copsAliveCount.ToString();
        bobsAliveCountText.text = bobsAliveCount.ToString();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Обновляем UI, если были изменения в свойствах игроков
        UpdateTeamCounts();
    }
}