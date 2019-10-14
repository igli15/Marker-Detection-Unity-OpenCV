using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableVariables/Double")]
public class DoubleVariable : ScriptableObject,ISerializationCallbackReceiver
{
    [SerializeField]
    private bool _isPersistent = false;
    
    public double value;

    [SerializeField]
    private double initValue;


    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if(!_isPersistent) value = initValue;
    }
}
