using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RoomExitHandler : MonoBehaviourPunCallbacks
{
    public void LeaveRoomAndLoadScene()
    {
        Time.timeScale = 1;
        PhotonNetwork.LeaveRoom(); // Покидаем комнату
        PhotonNetwork.LeaveLobby(); // Покидаем лобби (опционально)
        SceneManager.LoadScene(ConstantsHolder.MAINSCENE_SCENENAME_NAME); // Загружаем указанную сцену
    }

    public override void OnLeftRoom()
    {
        Time.timeScale = 1;
        Debug.Log("Left the room successfully.");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Time.timeScale = 1;
        Debug.Log("Disconnected from Photon: " + cause.ToString());
    }
}