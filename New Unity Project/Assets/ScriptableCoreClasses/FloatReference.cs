using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatReference
{
    public bool useConstant = true;
    
    [SerializeField]
    private float _constantValue;
    [SerializeField]
    private FloatVariable _variable;

    public float value
    {
        get { return useConstant ? _constantValue : _variable.value; }
        set
        {
            if(useConstant) _constantValue = value; 
            else _variable.value = value; 
        }
    }
    
}
