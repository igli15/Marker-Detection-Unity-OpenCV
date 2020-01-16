using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StringReference
{
    public bool useConstant = true;
    
    [SerializeField]
    private string _constantValue;
    [SerializeField]
    private StringVariable _variable;

    public string value
    {
        get { return useConstant ? _constantValue : _variable.value; }
        set
        {
            if(useConstant) _constantValue = value; 
            else _variable.value = value; 
        }
    }
}
