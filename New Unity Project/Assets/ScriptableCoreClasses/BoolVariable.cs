using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableVariables/Bool")]
public class BoolVariable : ScriptableObject,ISerializationCallbackReceiver
{
    [SerializeField]
    private bool _isPersistent = false;
    
    public bool value;

    [SerializeField]
    private bool initValue;


    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if(!_isPersistent) value = initValue;
    }
}
