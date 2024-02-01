using UnityEngine;
using Photon.Pun;
using TMPro; // Подключаем для работы с TextMeshPro

public class PlayerNicknameDisplay : MonoBehaviourPun
{
    public TMP_Text nicknameText; // Ссылка на TextMeshPro объект
    private Transform cameraTransform; // Для хранения ссылки на трансформ камеры

    [SerializeField] private float minSize = 1f; // Минимальный размер объекта
    [SerializeField] private float maxSize = 5f; // Максимальный размер объекта
    [SerializeField] private float distanceScaleFactor = 0.12f; // Коэффициент масштабирования

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
        RotateNicknameText();
        ResizeNicknameText();
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

    private void RotateNicknameText()
    {
        // Поворачиваем никнейм всегда к камере
        if (nicknameText.gameObject.activeSelf) // Обновляем только если никнейм активен
        {
            nicknameText.transform.rotation = Quaternion.LookRotation(nicknameText.transform.position - cameraTransform.position);
        }
    }

    void ResizeNicknameText()
    {
        // Вычисляем расстояние от объекта до камеры
        float distanceToPlayer = Vector3.Distance(transform.position, cameraTransform.position);

        // Масштабируем объект в зависимости от расстояния
        float newSize = Mathf.Clamp((distanceToPlayer +d) * distanceScaleFactor, minSize, maxSize);
        transform.localScale = new Vector3(newSize, newSize, newSize);
    }
}

