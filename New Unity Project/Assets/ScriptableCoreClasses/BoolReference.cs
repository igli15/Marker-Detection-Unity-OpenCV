using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoolReference
{
    public bool useConstant = true;
    
    [SerializeField]
    private bool _constantValue;
    [SerializeField]
    private BoolVariable _variable;

    public bool value
    {
        get { return useConstant ? _constantValue : _variable.value; }
        set
        {
            if(useConstant) _constantValue = value; 
            else _variable.value = value; 
        }
    }
}
