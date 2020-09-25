﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    Always,
    Hold
}

[RequireComponent(typeof(CameraController))]
public class FreeForm : MonoBehaviour
{
    public bool cameraEnabled = true;
    public CameraMode cameraMode = CameraMode.Hold;

    public bool controllerEnabled = false;
    public bool controllerInvertY = true;
    public bool mouseInvertY = false;
    public bool lockMouseCursor = true;

    public Vector2 mouseSensitivity = new Vector2(1.5f, 1.0f);
    public Vector2 controllerSensitivity = new Vector2(1.0f, 0.7f);

    [HideInInspector]
    public float x;
    [HideInInspector]
    public float y;
    private float yAngle;
    private float angle;

    private string rightAxisXName = "";
    private string rightAxisYName = "";
    private bool mouse0;
    private bool mouse1;

    private Vector3 upVector;
    private Vector3 downVector;

    private bool smartPivotInit;

    private CameraController cameraController;

    public void Start()
    {
        cameraController = GetComponent<CameraController>();

        mouse0 = false;
        mouse1 = false;

        x = 0;
        y = 0;

        smartPivotInit = true;

        upVector = Vector3.up;
        downVector = Vector3.down;
    }

    public void Update()
    {
        if (cameraEnabled)
        {
            mouse0 = Input.GetMouseButton(0);
            mouse1 = Input.GetMouseButton(1);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                if ((cameraMode == CameraMode.Hold && mouse0 || mouse1) || cameraMode == CameraMode.Always)
                {
                    x = Input.GetAxis("Mouse X") * mouseSensitivity.x;
                    y = Input.GetAxis("Mouse Y") * mouseSensitivity.y;

                    if (mouseInvertY)
                        y *= -1.0f;

                    if (lockMouseCursor)
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
                else
                {
                    x = 0;
                    y = 0;

                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
            }
            

            if (controllerEnabled && x == 0 && y == 0)
            {
                //x = Input.GetAxis(rightAxisXName) * controllerSensitivity.x;
                //y = Input.GetAxis(rightAxisYName) * controllerSensitivity.y;

                //if (controllerInvertY)
                    //y *= -1.0f;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
            {
                cameraController.desiredDistance += cameraController.zoomOutStepValue;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            {
                cameraController.desiredDistance -= cameraController.zoomOutStepValue;
            }

            if (cameraController.desiredDistance < 0)
                cameraController.desiredDistance = 0;

            Vector3 offsetVectorTransformed = cameraController.target.transform.rotation * cameraController.offsetVector;

            transform.RotateAround(cameraController.target.position + offsetVectorTransformed, cameraController.target.up, x);

            yAngle = -y;
            // Prevent camera flipping
            angle = Vector3.Angle(transform.forward, upVector);
            if (yAngle > 0)
            {
                if (angle + yAngle > 180.0f)
                {
                    yAngle = Vector3.Angle(transform.forward, upVector) - 180;

                    if (yAngle < 0)
                        yAngle = 0;
                }
            }
            else
            {
                if (angle + yAngle < 0.0f)
                {
                    yAngle = Vector3.Angle(transform.forward, downVector) - 180;
                }
            }

            if (!cameraController.smartPivot || cameraController.cameraNormalMode
                && (!cameraController.bGroundHit || (cameraController.bGroundHit && y < 0) || transform.position.y > (cameraController.target.position.y + cameraController.offsetVector.y)))
            {
                // normal mode
                transform.RotateAround(cameraController.target.position + offsetVectorTransformed, transform.right, yAngle);
            }
            else
            {
                // smart pivot mode
                if (smartPivotInit)
                {
                    smartPivotInit = false;
                    cameraController.InitSmartPivot();
                }

                transform.RotateAround(transform.position, transform.right, yAngle);

                if (transform.rotation.eulerAngles.x > cameraController.startingY || (transform.rotation.eulerAngles.x >= 0 && transform.rotation.eulerAngles.x < 90))
                {
                    smartPivotInit = true;

                    cameraController.DisableSmartPivot();
                }
            }
        }
    }
}
