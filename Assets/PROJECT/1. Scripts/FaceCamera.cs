using Photon.Pun;
using UnityEngine;

public class FaceCamera : MonoBehaviourPun
{
    private Camera mainCamera;

    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
        else
        {
            mainCamera = Camera.main; // Находим главную камеру
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // Поворачиваем Canvas к камере
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }
}