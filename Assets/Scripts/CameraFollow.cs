using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraFollow : MonoBehaviour
{
    private Camera myCamera;
    public float cameraMoveSpeed = 1.0f;
    private Func<float> GetCameraZoomFunc;
    private Func<Vector3> GetCameraFollowPositionFunc;

    //Optimization variables
    Vector3 cameraMoveDir = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        myCamera = transform.GetComponent<Camera>();
    }

    public void Setup(Func<Vector3> GetCameraFollowPositionFunc, Func<float> GetCameraZoomFunc)
    {
        this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
        this.GetCameraZoomFunc = GetCameraZoomFunc;
    }

    public void SetGetCameraFollowPositionFunc(Func<Vector3> GetCameraFollowPositionFunc)
    {
        this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
    }

    public void SetGetCameraZoomFunc(Func<float> GetCameraZoomFunc)
    {
        this.GetCameraZoomFunc = GetCameraZoomFunc;
    }
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
        cameraFollowPosition.z = transform.position.z;

        cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
        float distance = Vector3.Distance(cameraFollowPosition, transform.position);

        if (distance > 0)
        {
            cameraMoveDir = transform.position + cameraMoveDir * distance * cameraMoveSpeed * Time.deltaTime;
            float distanceAfterMoving = Vector3.Distance(cameraMoveDir, cameraFollowPosition);
            if (distanceAfterMoving > distance)
            {
                cameraMoveDir = cameraFollowPosition;
            }

            transform.position = cameraMoveDir;
        }
    }

    public void SetCameraFollowPosition(Vector3 cameraFollowPosition)
    {
        SetGetCameraFollowPositionFunc(() => cameraFollowPosition);
    }

    public void SetCameraZoom(float cameraZoom)
    {
        SetGetCameraZoomFunc(() => cameraZoom);
    }

    private void HandleZoom()
    {
        float cameraZoom = GetCameraZoomFunc();

        float cameraZoomDifference = cameraZoom - myCamera.orthographicSize;
        float cameraZoomSpeed = 3.0f;

        myCamera.orthographicSize += cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

        if (cameraZoomDifference > 0)
        {
            if (myCamera.orthographicSize > cameraZoom)
            {
                myCamera.orthographicSize = cameraZoom;
            }
        }
        else
        {
            if (myCamera.orthographicSize < cameraZoom)
            {
                myCamera.orthographicSize = cameraZoom;
            }
        }
        float opacity = 0.2f + (myCamera.orthographicSize / 10.0f) / 2.0f;
        Messenger.Broadcast<float,float>("kChangeAffinityBarScaleOpacity", myCamera.orthographicSize, opacity);
    }
}