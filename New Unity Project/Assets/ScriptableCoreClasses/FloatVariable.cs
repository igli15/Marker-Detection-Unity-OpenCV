using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableVariables/Float")]
public class FloatVariable : ScriptableObject,ISerializationCallbackReceiver
{
    [SerializeField]
    private bool _isPersistent = false;
    
    public float value;

    [SerializeField]
    private float initValue;


    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if(!_isPersistent) value = initValue;
    }
}
