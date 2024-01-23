using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject bobsPrefab; // Префаб персонажа для команды Bobs
    public GameObject copsPrefab; // Префаб персонажа для команды Cops
    public Transform[] bobsSpawnPoints; // Точки спавна для команды Bobs
    public Transform[] copsSpawnPoints; // Точки спавна для команды Cops

    private void Awake()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue) 
            && Enum.TryParse(teamValue.ToString(), out Team playerTeam))
        {
            Transform[] spawnPoints = (playerTeam == Team.Bobs) ? bobsSpawnPoints : copsSpawnPoints;
            GameObject prefab = (playerTeam == Team.Bobs) ? bobsPrefab : copsPrefab;
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Получаем текущие пользовательские свойства
            ExitGames.Client.Photon.Hashtable currentProps = PhotonNetwork.LocalPlayer.CustomProperties;

            // Добавляем или обновляем свойство LifeStatus
            currentProps[ConstantsHolder.LIVE_STATUS_PARAM_NAME] = "Alive";

            // Устанавливаем обновленные пользовательские свойства
            PhotonNetwork.LocalPlayer.SetCustomProperties(currentProps);
            
            PhotonNetwork.Instantiate(prefab.name, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Team not set for player: " + PhotonNetwork.LocalPlayer.NickName);
        }
    }
}
