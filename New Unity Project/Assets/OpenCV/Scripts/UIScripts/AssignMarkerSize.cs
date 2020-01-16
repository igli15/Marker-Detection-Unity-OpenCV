using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignMarkerSize : MonoBehaviour
{
    [SerializeField] private MarkerBehaviour[] markers;

    [SerializeField] private InputField inputField;

    private void Awake()
    {
        inputField.onValueChanged.AddListener(OnInputChanged);
    }

    void OnInputChanged(string s)
    {
        float size = 0.1f;
        
        if (float.TryParse(s,out size))
        {
            foreach (MarkerBehaviour m in markers)
            {
                m.SetWorldSize(size);
            }
        }
    }
}
