using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoubleReference 
{
    public bool useConstant = true;
    
    [SerializeField]
    private double _constantValue;
    [SerializeField]
    private DoubleVariable _variable;

    public double value
    {
        get { return useConstant ? _constantValue : _variable.value; }
        set
        {
            if(useConstant) _constantValue = value; 
            else _variable.value = value; 
        }
    }
}
