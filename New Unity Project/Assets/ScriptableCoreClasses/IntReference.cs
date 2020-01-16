using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IntReference 
{
    public bool useConstant = true;
    
    [SerializeField]
    private int _constantValue;
    [SerializeField]
    private IntVariable _variable;

    public int value
    {
        get { return useConstant ? _constantValue : _variable.value; }
        set
        {
            if(useConstant) _constantValue = value; 
            else _variable.value = value; 
        }
    }
}
