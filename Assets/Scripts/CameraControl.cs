using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Control Settings")]
    public float cameraMoveTime;
    public float cameraZoomSpeed;
    public float cameraZoomMax;
    public float cameraZoomMin;
    public float cameraZoomDefault;
    [Header("References")]
    public Transform target;
    Camera camera;

    // Runtime variables
    Vector3 nextCameraPosition;
    Vector2 currentCamVelocity;
    float mouseWheelInput;
    float currentZoomVelocity;
    float currentOrthographicSize;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        nextCameraPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        currentCamVelocity = Vector2.zero;
        camera.orthographicSize = cameraZoomDefault;
        currentOrthographicSize = cameraZoomDefault;
        currentZoomVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        nextCameraPosition = new Vector3(target.position.x, target.position.y, nextCameraPosition.z);
        if(transform.position != nextCameraPosition){
            Vector2 camSmoothDampPosition = Vector2.SmoothDamp(transform.position, nextCameraPosition, ref currentCamVelocity, cameraMoveTime);
            transform.position = new Vector3(camSmoothDampPosition.x, camSmoothDampPosition.y, nextCameraPosition.z);
        }

        mouseWheelInput = Input.GetAxis("Mouse ScrollWheel");


        currentOrthographicSize -= mouseWheelInput * cameraZoomSpeed * Time.deltaTime;

        if(currentOrthographicSize > cameraZoomMax){
            currentOrthographicSize = cameraZoomMax;
        }
        else if(currentOrthographicSize < cameraZoomMin){
            currentOrthographicSize = cameraZoomMin;
        }
        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, currentOrthographicSize, ref currentZoomVelocity, cameraMoveTime);
    }
}
