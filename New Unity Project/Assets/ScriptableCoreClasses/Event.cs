using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Event")]
public class Event : ScriptableObject
{
    List<EventListener> _listeners = new List<EventListener>();

    //private object _thrower;

    //public object eventThrower => _thrower;
    
    public void Raise()
    {
        foreach (EventListener listener in _listeners)
        {
           listener.OnEventRaised();
        }
    }
    
    
    public void Raise(Object arg)
    {
        //_thrower = mono;
        foreach (EventListener listener in _listeners)
        {
            listener.OnEventRaised(arg);
        }
    }

    public void RegisterListener(EventListener eventListener)
    {
        _listeners.Add(eventListener);
    }
    
    public void DeRegisterListener(EventListener eventListener)
    {
        _listeners.Remove(eventListener);
    }
}
