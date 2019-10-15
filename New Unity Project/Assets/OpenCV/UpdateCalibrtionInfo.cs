using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCalibrtionInfo : MonoBehaviour
{
    [SerializeField] private Button captureCalibButton;
    [SerializeField] private Button calibrateButton;
    [SerializeField] private Button finishCalibrationButton;

    private bool finishedCalibration = false;
    private bool startedCalibration = false;
    private void Start()
    {
        CalibrateCamera.OnCalibrationFinished += OnCalibrationFinished;
        CalibrateCamera.OnCalibrationStarted += OnCalibrationStarted;
        CalibrateCamera.OnCalibrationReset += OnCalibrationReset;
    }

    private void Update()
    {
        finishCalibrationButton.gameObject.SetActive(finishedCalibration);
        calibrateButton.interactable = !startedCalibration;
    }

    private void OnCalibrationFinished(CalibrationData data)
    {
        finishedCalibration = true;
    }
    
    private void OnCalibrationStarted()
    {
        startedCalibration = true;
    }

    private void OnCalibrationReset()
    {
        startedCalibration = false;
        finishedCalibration = false;
    }

    private void OnDisable()
    {
        CalibrateCamera.OnCalibrationFinished -= OnCalibrationFinished;
        CalibrateCamera.OnCalibrationStarted -= OnCalibrationStarted;
        CalibrateCamera.OnCalibrationReset -= OnCalibrationReset;
    }
}
