using UnityEngine;
using Photon.Pun;
using TMPro; // Подключаем для работы с TextMeshPro

public class PlayerNicknameDisplay : MonoBehaviourPun
{
    public TMP_Text nicknameText; // Ссылка на TextMeshPro объект
    private Transform cameraTransform; // Для хранения ссылки на трансформ камеры

    private void Start()
    {
        cameraTransform = Camera.main.transform;

        // Установка никнейма из Photon и проверка на видимость
        if (photonView.IsMine)
        {
            nicknameText.text = PhotonNetwork.NickName;
            SetNicknameVisibility(true); // Всегда видим свой никнейм
        }
        else
        {
            nicknameText.text = photonView.Owner.NickName;
            CheckAndSetNicknameVisibility();
        }
    }

    private void Update()
    {
        // Поворачиваем никнейм всегда к камере
        if (nicknameText.gameObject.activeSelf) // Обновляем только если никнейм активен
        {
            nicknameText.transform.rotation = Quaternion.LookRotation(nicknameText.transform.position - cameraTransform.position);
        }
    }

    private void CheckAndSetNicknameVisibility()
    {
        // Получение команды локального и отображаемого игрока
        string localPlayerTeam = PhotonNetwork.LocalPlayer.CustomProperties[ConstantsHolder.TEAM_PARAM_NAME]?.ToString();
        string displayedPlayerTeam = photonView.Owner.CustomProperties[ConstantsHolder.TEAM_PARAM_NAME]?.ToString();

        // Сравнение команд и установка видимости
        bool isSameTeam = localPlayerTeam == displayedPlayerTeam;
        SetNicknameVisibility(isSameTeam);
    }

    private void SetNicknameVisibility(bool isVisible)
    {
        nicknameText.gameObject.SetActive(isVisible);
    }

    [PunRPC]
    void SyncNickname(string nickname)
    {
        nicknameText.text = nickname;
        CheckAndSetNicknameVisibility(); // Повторная проверка видимости после синхронизации
    }
}

