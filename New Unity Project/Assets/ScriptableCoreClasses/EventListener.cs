using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


[Serializable]
public class ArgEvent : UnityEvent<Object>
{

}

public class EventListener : MonoBehaviour
{
    [SerializeField]
    private Event _event;
    

    [SerializeField]
    private UnityEvent _response;
    
    [SerializeField]
    private ArgEvent _responseWithArg;
    
    public void OnEventRaised()
    {
        _response.Invoke();
    }
    
    public void OnEventRaised(Object arg)
    {
        _responseWithArg.Invoke(arg);
    }

    
    
    private void OnEnable()
    {
        _event.RegisterListener(this);
    }

    private void OnDisable()
    {
        _event.DeRegisterListener(this);
    }
}
