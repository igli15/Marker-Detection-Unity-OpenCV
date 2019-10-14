using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArg
{
    private object _value;
    
    public static EventArg CreateArg<T>(T pValue) where T:Object
    {
        EventArg arg = new EventArg {_value = pValue};
        return arg;
    }

    public U Value<U>()where U: Object
    {
        return (U) _value;
    }

    public void SetValue(Object value)
    {
        _value = value;
    }
}
