using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class FPSMouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseX, MouseY }
    public RotationAxes axes = RotationAxes.MouseY;

    private float currentSensivity_X = 1.5f;
    private float currentSensivity_Y = 1.5f;

    private float sensivity_X = 1.5f;
    private float sensivity_Y = 1.5f;

    private float rotation_X , rotation_Y ;

    private float minimum_X = -360f;
    private float maximum_X = 360f;

    private float minimum_Y = -60f;
    private float maximum_Y = 60f;

    private Quaternion originalRotation;

    private float mouseSensivity = 1.7f;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        HandleRotation();
    }

    float ClampAngle(float angle, float min, float max)
    {
        if(angle < -360f)
        {
            angle += 360f;
        }

        if (angle > 360)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    void HandleRotation()
    {
        if(currentSensivity_X != mouseSensivity || currentSensivity_Y != mouseSensivity)
        {
            currentSensivity_X = currentSensivity_Y = mouseSensivity;
        }

        sensivity_X = currentSensivity_X;
        sensivity_Y = currentSensivity_Y;

        if (axes == RotationAxes.MouseX)
        {
            rotation_X += Input.GetAxis("Mouse X") * sensivity_X;

            rotation_X = ClampAngle(rotation_X, minimum_X, maximum_X);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotation_X, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        if (axes == RotationAxes.MouseY)
        {
            rotation_Y += Input.GetAxis("Mouse Y") * sensivity_Y;

            rotation_Y = ClampAngle(rotation_Y, minimum_Y, maximum_Y);
            Quaternion yQuaternion = Quaternion.AngleAxis(-rotation_Y, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }
    }


}
