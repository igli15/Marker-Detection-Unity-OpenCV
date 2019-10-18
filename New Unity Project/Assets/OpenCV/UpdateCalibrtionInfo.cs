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
    }

    private void OnCalibrationFinished(CalibrationData data)
    {
        finishedCalibration = true;
        calibratedString = "Calibrated: True";
        projectionErrorString = "ProjectionError: " + data.projectionError;
    }
    
    private void OnCalibrationStarted()
    {
        startedCalibration = true;
    }

    private void OnCalibrationReset()
    {
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
