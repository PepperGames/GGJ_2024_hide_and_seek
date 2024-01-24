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
    public string copPunchDashKey;
    public string bobTurnKey;
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
        AnimateTrigger(copPunchDashKey);
    }
    
    public void AnimateBobTurnAbility()
    {
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
    void ServerAnimateBobTurn()
    {
        anim.Play(bobTurnKey);
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
