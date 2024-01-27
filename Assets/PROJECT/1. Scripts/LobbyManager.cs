using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random; // Убедитесь, что используете UnityEngine.Random


public enum Team
{
    None,
    Cops,
    Bobs
}
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField nicknameInputField; // Поле ввода для ника
    public TMP_InputField roomCodeInputField;
    public TMP_Text nicknameDisplay; // TextMeshPro элемент для отображения никнейма
    public TMP_Text roomCodeDisplay;
    public TMP_Text timerText;
    public int maxNickLength;
    public int minNickLength;
    public GameObject playerListItemPrefab; // Префаб для элемента списка игроков
    public Transform copsHolder; // Холдер для команды Cops
    public Transform bobsHolder; // Холдер для команды Bobs
    public UnityEvent OnConnected;
    public UnityEvent OnDiconnect;
    public UnityEvent OnJoinedRoomEvent;
    public UnityEvent OnLeftRoomEvent;
    public UnityEvent OnIRoomOwner;

    private double startTime = 0;


    private void Start()
    {
        SetRandomNick();
        Time.timeScale = 1;
    }
    
    void Update()
    {
        if (startTime > 0)
        {
            double timer = startTime - PhotonNetwork.Time;
            if (timer > 0)
            {
                DisplayTimer(timer);
            }
            else
            {
                //Debug.Log("GAME START!");
                startTime = 0;
                
            }
        }
    }

    public void SetRandomNick()
    {
        nicknameInputField.text = "Narik#" + Random.Range(1000, 10000).ToString();
    }

    public void ConnectToPhoton()
    {
        // Проверка и сохранение никнейма
        string nickname = nicknameInputField.text;
        if (nickname.Length > maxNickLength) 
        {
            Debug.LogError("Nickname is too long.");
            nicknameInputField.text = "Nickname is too long.";
            return;
        }
        
        if (nickname.Length <= minNickLength) 
        {
            Debug.LogError("Nickname is too short.");
            nicknameInputField.text = "Nickname is too short.";
            return;
        }

        PhotonNetwork.NickName = nickname; // Сохраняем никнейм
        PhotonNetwork.ConnectUsingSettings(); // Подключаемся к Photon
    }
    
    public void LeaveLobby()
    {
        PhotonNetwork.Disconnect(); // Отключаемся от Photon
    }

    public void LeaveTheRoom()
    {
        OnLeftRoomEvent.Invoke();
        PhotonNetwork.LeaveRoom();
    }

    public void FindMatch()
    {
        AssignTeam();
        TryJoiningRoom();
    }

    public void StartGameManualy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        photonView.RPC("SyncLoadScene", RpcTarget.AllBuffered);
    }

    private void TryJoiningRoom()
    {
        PhotonNetwork.JoinRandomRoom(); // Попытка присоединиться к случайной комнате
    }
    
    public void CreateRoom()
    {
        int roomCode = Random.Range(1000, 10000);
        string roomName = ConstantsHolder.ROOM_NAME_PREFIX + roomCode;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 10 });
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    private void AssignTeam()
    {
        var team = ChooseTeam();
        Hashtable props = new Hashtable
        {
            {ConstantsHolder.TEAM_PARAM_NAME, team.ToString()}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void JoinRoomByCode()
    {
        string roomCode = roomCodeInputField.text; // Получаем код комнаты из поля ввода
        if (!string.IsNullOrEmpty(roomCode))
        {
            string roomName = ConstantsHolder.ROOM_NAME_PREFIX + roomCode;
            PhotonNetwork.JoinRoom(roomName); // Попытка присоединиться к комнате с указанным именем
            AssignTeam();
        }
        else
        {
            Debug.LogError("Room code is empty.");
        }
    }
    
    public void CreateAndJoinRoom()
    {
        // Создаем уникальное имя для комнаты
        string roomName = "Room_" + Random.Range(1000, 9999).ToString();

        // Создаем опции комнаты
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10; // Максимальное количество игроков
        roomOptions.IsVisible = true; // Комната видима для других игроков
        roomOptions.IsOpen = true; // Комната доступна для присоединения

        // Попытка создать комнату
        PhotonNetwork.CreateRoom(roomName, roomOptions);

        // Устанавливаем команду игрока
        AssignTeam();
    }

    
    private string ChooseTeam()
    {
        // Проверяем, существует ли текущая комната
        if (PhotonNetwork.CurrentRoom != null)
        {
            int playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
            return (playersCount % 2 == 0) ? Team.Cops.ToString() : Team.Bobs.ToString();
        }
        else
        {
            // Если комната не существует, выбираем команду случайным образом
            return (Random.Range(0, 2) == 0) ? Team.Bobs.ToString() : Team.Cops.ToString();
        }
    }

    public void SwitchTeams()
    {
        Team playerTeam = Team.None;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue))
        {
            if (Enum.TryParse(teamValue.ToString(), out Team parsedTeam))
            {
                playerTeam = parsedTeam;
            }
            else
            {
                Debug.LogError("Failed to parse team value: " + teamValue);
                return; // Или установите значение по умолчанию для playerTeam
            }
        }
        else
        {
            Debug.LogError("Team property not set for player: " + PhotonNetwork.LocalPlayer.NickName);
            return; // Или установите значение по умолчанию для playerTeam
        }

        if (playerTeam == Team.Bobs)
        {
            playerTeam = Team.Cops;
        }
        else if (playerTeam == Team.Cops)
        {
            playerTeam = Team.Bobs;
        }
        
        Hashtable props = new Hashtable
        {
            {ConstantsHolder.TEAM_PARAM_NAME, playerTeam.ToString()}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }



    private void UpdatePlayerList()
    {
        // Очистка предыдущих записей
        ClearPlayerList();

        // Добавление игроков в список
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            AddPlayerToList(player);
        }
    }

    private void ClearPlayerList()
    {
        // Удаляем старые элементы списка
        foreach (Transform child in copsHolder)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in bobsHolder)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddPlayerToList(Player player)
    {
        GameObject itemPrefab = playerListItemPrefab;
        Team playerTeam = Team.None;
        if (player.CustomProperties.TryGetValue(ConstantsHolder.TEAM_PARAM_NAME, out object teamValue))
        {
            if (Enum.TryParse(teamValue.ToString(), out Team parsedTeam))
            {
                playerTeam = parsedTeam;
            }
            else
            {
                Debug.LogError("Failed to parse team value: " + teamValue);
                return; // Или установите значение по умолчанию для playerTeam
            }
        }
        else
        {
            Debug.LogError("Team property not set for player: " + player.NickName);
            return; // Или установите значение по умолчанию для playerTeam
        }

        Transform parentHolder = playerTeam == Team.Cops ? copsHolder : bobsHolder;
        GameObject item = Instantiate(itemPrefab, parentHolder);
        var roomPlayerListItem = item.GetComponent<RoomPlayerListItem>();
        if (roomPlayerListItem != null)
        {
            roomPlayerListItem.Initialize(player.NickName, playerTeam);
        }
        else
        {
            Debug.LogError("RoomPlayerListItem component not found on the item prefab.");
        }
    }
    
    [PunRPC]
    void SyncStartTime(double time)
    {
        startTime = time;
    }

    [PunRPC]
    void SyncLoadScene()
    {
        
        PhotonNetwork.LoadLevel(ConstantsHolder.LEVEL_SCENENAME_NAME);
    }

    void DisplayTimer(double time)
    {
        // Отобразите таймер на экране
        // Например, если у вас есть текстовый элемент для таймера, вы можете сделать так:
        timerText.text = "Game start in: " + time.ToString("F2"); // Округление до двух знаков после запятой
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available. Creating a new one.");
        CreateRoom(); // Если нет доступных комнат, создаем новую
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon with nickname: " + PhotonNetwork.NickName);
        nicknameDisplay.text = "Hi " + PhotonNetwork.NickName + "!"; // Обновляем отображаемый никнейм
        OnConnected.Invoke();
        //AssignTeam();
        // Здесь можно перейти к следующей сцене или отобразить дополнительные опции
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon: " + cause.ToString());
        SetRandomNick();
        OnDiconnect.Invoke();
        // Обработка отключения от Photon
    }
    
    public override void OnJoinedRoom()
    {
        roomCodeDisplay.text = PhotonNetwork.CurrentRoom.Name.Split(ConstantsHolder.ROOM_NAME_PREFIX)[1];
        
        OnJoinedRoomEvent.Invoke();
        //AssignTeam();
        UpdatePlayerList();
        
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            OnIRoomOwner.Invoke();
            startTime = PhotonNetwork.Time + ConstantsHolder.GAME_START_COUNTDOWN_TIME;
            photonView.RPC("SyncStartTime", RpcTarget.AllBuffered, startTime);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //AssignTeam();
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        UpdatePlayerList();
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        Debug.Log("New master client is now: " + newMasterClient.NickName);

        // Вызовите вашу проверку здесь
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            OnIRoomOwner.Invoke();
        }
    }

}
