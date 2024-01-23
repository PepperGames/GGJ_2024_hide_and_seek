using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EventAggregator : MonoBehaviour {
    public EventInstance[] Events;

    public void Call(string Name)
    {
        for (int i = 0; i < Events.Length; i++)
        {
            if (Events[i].Name == Name)
            {
                Events[i].Event.Invoke();
                break;
            }
        }
    }

    [System.Serializable]
    public class EventInstance
    {
        public string Name;
        public UnityEvent Event;
    }
}
