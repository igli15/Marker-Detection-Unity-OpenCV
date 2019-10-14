using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableVariables/String")]
public class StringVariable : ScriptableObject,ISerializationCallbackReceiver
{
    [SerializeField]
    private bool _isPersistent = false;
    
    public string value;

    [SerializeField]
    private string initValue;


    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if(!_isPersistent) value = initValue;
    }
}
