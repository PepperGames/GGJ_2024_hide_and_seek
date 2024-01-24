using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class CopPunchAbility : BaseAbility
{
    public Animator anim;
    public UnityEvent OnPunch;
    public GameObject bobDeathCollider;
    public Transform deathColliderSpawnPoint;
    
    
    public override void CheckAbilityUse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ActivateAbility();
        }
    }
    
    public override void LocalUseOfAbility()
    {
        OnPunch.Invoke();
        photonView.RPC("AnimateCopPunch", RpcTarget.Others);

        //SpawnBobDeathProjectile();
    }

    public override void OtherPlayersAbilityUse(string playerName, string usedAbility)
    {
        //OnPunch.Invoke();
        //SpawnBobDeathProjectile();
    }

    public void SpawnBobDeathProjectile()
    {
        if (photonView.IsMine)
        {
            // Проверяем, существует ли префаб для спавна
            if (bobDeathCollider == null)
            {
                Debug.LogError("Bob Death Collider prefab is not assigned.");
                return;
            }

            // Рассчитываем позицию и направление спавна
            Vector3 spawnPosition = deathColliderSpawnPoint.position;
            Quaternion spawnRotation = deathColliderSpawnPoint.rotation;

            // Создаем объект на сервере
            PhotonNetwork.Instantiate(bobDeathCollider.name, spawnPosition, spawnRotation);
        }
    }

    [PunRPC]
    void AnimateCopPunch()
    {
        anim.SetTrigger(ConstantsHolder.COP_PUNCH_TRIGGER_NAME);
    }

}
