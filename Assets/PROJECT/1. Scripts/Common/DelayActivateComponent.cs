using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DelayActivateComponent : MonoBehaviour
{
	[SerializeField] private UnityEvent OnActivate;

	public void Activate (float delay) 
	{
		//delay.Delay (OnActivate.Invoke);
		Invoke ("DelayInvoke",delay);
	}

	private void DelayInvoke ()
	{
		OnActivate.Invoke ();
	}

	public void CancelDelay () 
	{
		CancelInvoke ();
	}

	private void OnDestroy () 
	{
		//MurkaCore.Instance.DelayedCallService.RemoveDelayedCallsTo (OnActivate.Invoke);
		CancelInvoke ();	
	}

}
