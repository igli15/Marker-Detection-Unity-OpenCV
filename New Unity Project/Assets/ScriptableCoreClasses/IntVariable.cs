using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableVariables/Int")]
public class IntVariable : ScriptableObject,ISerializationCallbackReceiver
{
    [SerializeField]
    private bool _isPersistent = false;
    
    public int value;

    [SerializeField]
    private int initValue;


    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if(!_isPersistent) value = initValue;
    }
}
