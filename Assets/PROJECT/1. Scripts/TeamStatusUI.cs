using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.Events;

public class TeamStatusUI : MonoBehaviourPunCallbacks
{
    public TMP_Text copsAliveCountText;
    public TMP_Text bobsAliveCountText;
    public UnityEvent OnTeamWin;
    public UnityEvent OnTeamLost;

    private Team myTeam;
    private bool initialize;
    private bool winnerDetermined;

    private void Start()
    {
        DetermineTeam();
        UpdateTeamCounts();
    }

    private void LateUpdate()
    {
        if (winnerDetermined)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void DetermineTeam()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue))
        {
            myTeam = (Team)System.Enum.Parse(typeof(Team), teamValue.ToString());
        }
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

        if (copsAliveCount > 0 && bobsAliveCount > 0 && !initialize)
        {
            initialize = true;
            return;
        }

        if (initialize)
        {
            CheckForTeamWinOrLoss(copsAliveCount, bobsAliveCount);
        }
    }

    private void CheckForTeamWinOrLoss(int copsAliveCount, int bobsAliveCount)
    {
        if (myTeam == Team.Cops && bobsAliveCount == 0)
        {
            winnerDetermined = true;
            OnTeamWin.Invoke();
        }
        else if (myTeam == Team.Cops && copsAliveCount == 0)
        {
            winnerDetermined = true;
            OnTeamLost.Invoke();
        }
        else if (myTeam == Team.Bobs && copsAliveCount == 0)
        {
            winnerDetermined = true;
            OnTeamWin.Invoke();
        }
        else if (myTeam == Team.Bobs && bobsAliveCount == 0)
        {
            winnerDetermined = true;
            OnTeamLost.Invoke();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateTeamCounts();
    }
    
    
}
