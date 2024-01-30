using UnityEngine;
using Photon.Pun;
using TMPro; // Подключаем для работы с TextMeshPro

public class PlayerNicknameDisplay : MonoBehaviourPun
{
    public TMP_Text nicknameText; // Ссылка на TextMeshPro объект, который будет отображать никнейм
    //public Vector3 offset = new Vector3(0, 2, 0); // Смещение текста относительно позиции игрока

    private Transform cameraTransform; // Для хранения ссылки на трансформ камеры

    private void Start()
    {
        cameraTransform = Camera.main.transform;

        // Установка никнейма из Photon
        if (photonView.IsMine)
        {
            nicknameText.text = PhotonNetwork.NickName;
        }
        else
        {
            nicknameText.text = photonView.Owner.NickName;
        }

        // Синхронизация никнейма с другими клиентами
        photonView.RPC("SyncNickname", RpcTarget.OthersBuffered, nicknameText.text);
    }

    private void Update()
    {
        // Поворачиваем никнейм всегда к камере
        nicknameText.transform.rotation = Quaternion.LookRotation(nicknameText.transform.position - cameraTransform.position);
    }

    [PunRPC]
    void SyncNickname(string nickname)
    {
        nicknameText.text = nickname;
    }
}
