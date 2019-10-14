using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateDebugText : MonoBehaviour
{
    public ArDebugValues arDebugValues;
    public  TextMeshProUGUI textMesh;

    private void Update()
    {
        //textMesh.text = "Debug: \n" + arDebugValues.ToString();
    }
}
