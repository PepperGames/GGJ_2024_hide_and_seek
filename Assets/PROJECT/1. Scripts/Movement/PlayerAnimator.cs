using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerAnimator : MonoBehaviourPun
{
    public Animator anim;
    public string runKey;
    public string jumpKey;
    public string landAnimationName;
    public string copPunchTriggerKey;
    public string copPunchDashAnimClipName;
    public string copPunchDashKey;
    public string copStunAnimClipKey;
    public string copStunBoolKey;
    public string bobStunKey;
    
    
    public string bobTurnKey;
    public ParticleSystem bobStunEffect;
    public ParticleSystem bobTurnEffect;
    public GameObject[] objectsToDisable;
    public GameObject[] objectsToEnable;

    
    
    public void AnimateIdle()
    {
        AnimateBol(runKey, false);
    }
    
    public void AnimateJump()
    {
        PlayAnimationClip(jumpKey);
    }

    public void AnimateRun()
    {
        AnimateBol(runKey, true);
    }
    
    public void AnimateLand()
    {
        PlayAnimationClip(landAnimationName);
    }
    
    public void AnimateCopPunch()
    {
        AnimateTrigger(copPunchTriggerKey);
    }
    
    public void AnimateCopDash()
    {
        PlayAnimationClip(copPunchDashAnimClipName);
        AnimateBol(copPunchDashKey, true);
    }
    
    public void AnimateBobStun()
    {
        bobStunEffect.gameObject.SetActive(true);
        bobStunEffect.Play(true);
        photonView.RPC("ServerAnimateBobStun", RpcTarget.Others);
    }
    
    public void AnimateCopStun()
    {
        PlayAnimationClip(copStunAnimClipKey);
        AnimateBol(copStunBoolKey, true);
    }
    
    public void StopAnimateCopStun()
    {
        PlayAnimationClip(copStunAnimClipKey);
        AnimateBol(copStunBoolKey, false);
    }
    
    public void StopAnimateCopDash()
    {
        AnimateBol(copPunchDashKey, false);
    }
    
    public void AnimateBobTurnAbility()
    {
        bobTurnEffect.Play(true);
        photonView.RPC("ServerAnimateBobTurn", RpcTarget.Others);
    }

    private void PlayAnimationClip(string clipName)
    {
        anim.Play(clipName);
        photonView.RPC("ServerPlayAnimation", RpcTarget.Others, clipName);
    }

    private void AnimateTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
        photonView.RPC("ServerTriggerAnimate", RpcTarget.Others, triggerName);
    }
    
    private void AnimateBol(string name, bool value)
    {
        anim.SetBool(name, value);
        photonView.RPC("ServerAnimateBool", RpcTarget.Others, name, value);
    }

    [PunRPC]
    void ServerTriggerAnimate(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
    
    [PunRPC]
    void ServerAnimateBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }
    
    
    [PunRPC]
    void ServerPlayAnimation(string name)
    {
        anim.Play(name);
    }
    
    [PunRPC]
    void ServerAnimateBobStun()
    {
        bobStunEffect.gameObject.SetActive(true);
        bobStunEffect.Play(true);
    }
    
    [PunRPC]
    void ServerAnimateBobTurn()
    {
        //anim.Play(bobTurnKey);
        bobTurnEffect.Play(true);
        foreach (var ob in objectsToDisable)
        {
            ob.SetActive(false);
        }
        
        foreach (var ob in objectsToEnable)
        {
            ob.SetActive(true);
        }
    }
    
}
