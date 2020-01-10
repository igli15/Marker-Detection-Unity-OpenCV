using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCalibrtionInfo : MonoBehaviour
{
    [SerializeField] private Button captureCalibButton;
    [SerializeField] private Button calibrateButton;
    [SerializeField] private Button finishCalibrationButton;
    [SerializeField] private CalibrationData calibrationData;
    
    [SerializeField] private TextMeshProUGUI projectionErrorText;
    [SerializeField] private TextMeshProUGUI calibratedText;
    
    private bool finishedCalibration = false;
    private bool startedCalibration = false;
    
    private string calibratedString = "Calibrated: false";
    private string projectionErrorString = "ProjectionError: -1";
    
    private void Start()
    {
        CalibrateCamera.OnCalibrationFinished += OnCalibrationFinished;
        CalibrateCamera.OnCalibrationStarted += OnCalibrationStarted;
        CalibrateCamera.OnCalibrationReset += OnCalibrationReset;
    }

    private void Update()
    {
        if (calibrationData.hasCalibratedBefore)
        {
            finishCalibrationButton.interactable = true;
        }
        else
        {
            finishCalibrationButton.interactable = finishedCalibration;
        }
        
        calibrateButton.interactable = !startedCalibration;

        calibratedText.text = calibratedString;
        projectionErrorText.text = projectionErrorString;

        if (finishedCalibration)
        {
            calibratedText.color = Color.green;
            projectionErrorText.color = Color.green;
        }
    }

    private void OnCalibrationFinished(CalibrationData data)
    {
        finishedCalibration = true;
        //calibratedText.color = Color.green;
        //projectionErrorText.color = Color.green;
        
        calibratedString = "Calibrated: True";
        projectionErrorString = "ProjectionError: " + data.projectionError;
    }
    
    private void OnCalibrationStarted()
    {
        startedCalibration = true;
    }

    private void OnCalibrationReset()
    {
        calibratedText.color = Color.red;
        projectionErrorText.color = Color.red;
        
        startedCalibration = false;
        finishedCalibration = false;
        calibratedString = "Calibrated: false";
        projectionErrorString = "ProjectionError: -1";
    }

    private void OnDisable()
    {
        CalibrateCamera.OnCalibrationFinished -= OnCalibrationFinished;
        CalibrateCamera.OnCalibrationStarted -= OnCalibrationStarted;
        CalibrateCamera.OnCalibrationReset -= OnCalibrationReset;
    }
}
