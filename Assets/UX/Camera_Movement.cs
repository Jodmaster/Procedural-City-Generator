using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Camera_Movement : MonoBehaviour {

    Camera cam;
    public Transform target;
    public GameObject targetUI;
    Generator_UI_Controller controller;

    public float minimumDistance;
    public float maximumDistance;

    public float rotationSpeed;
    public float zoomSpeed;

    Vector3 rotation;

    // Start is called before the first frame update
    void Start() {
        cam = GetComponentInChildren<Camera>();  
        controller = GameObject.FindObjectOfType<Generator_UI_Controller>();
        targetUI = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    // Update is called once per frame
    void Update() {

        CameraZoom();

        if(Input.GetKey(KeyCode.Mouse0)) {
            cameraRotate();
        }

        moveCameraTarget();
        drawTargetPoint();
    }

    private void CameraZoom() {

        float step = zoomSpeed * Time.deltaTime;
        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if(Input.mouseScrollDelta.y > 0) {
            if(distanceToTarget > minimumDistance) {
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            }          
        } else if(Input.mouseScrollDelta.y < 0) {
                transform.position = Vector3.MoveTowards(transform.position, target.position, -1f * step);
        }

        distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        if(distanceToTarget < minimumDistance) {
            transform.position = target.position - directionToTarget.normalized * minimumDistance;
        }
        
        if(distanceToTarget > maximumDistance) {
            transform.position = target.position - directionToTarget.normalized * maximumDistance;
        }
        
    }

    private void cameraRotate() {
        
        rotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
        rotation.y -= Input.GetAxis("Mouse Y") * rotationSpeed;
        
        rotation.y = Mathf.Clamp(rotation.y, 0, 80f);

        Quaternion QT = Quaternion.Euler(rotation.y, rotation.x, 0f);
        transform.rotation = QT;
    }

    //method for use in UI controller for when the map limits are changed
    public void setCameraOnTerrainChange() {
        Vector3 targetPosition = new Vector3(controller.mapSizeXSlider.value / 2, 0, controller.mapSizeYSlider.value / 2);

        Vector3 currentOffset = new Vector3(targetPosition.x - transform.position.x, 0f, targetPosition.z - transform.position.z);

        target.position = targetPosition;
        transform.position += currentOffset;

    }

    //moves the camera target relative to current camera direction
    //mapped to WASD 
    private void moveCameraTarget() {

        Vector3 targetPosition = target.transform.position;

        //left right
        if(Input.GetKey(KeyCode.A)) {
            targetPosition = targetPosition + (-1f * transform.right);
        } else if(Input.GetKey(KeyCode.D)) {
            targetPosition = targetPosition + transform.right;
        }

        //up down
        if(Input.GetKey(KeyCode.S)) {
            targetPosition = targetPosition + (-1f * transform.forward);
        } else if(Input.GetKey(KeyCode.W)) {
            targetPosition = targetPosition + transform.forward;
        }

        //clamp coordinates within the bounds of the terrain so user can't fly off
        targetPosition.x = Mathf.Clamp(targetPosition.x, 0, controller.mapSizeXSlider.value);
        targetPosition.z = Mathf.Clamp(targetPosition.z, 0, controller.mapSizeYSlider.value);

        //get offset from target to camera holder
        Vector3 currentOffset = new Vector3(targetPosition.x - transform.position.x, 0f, targetPosition.z - transform.position.z);

        //apply new position to target and update camera holder posiiton
        target.position = new Vector3(targetPosition.x, 0f, targetPosition.z);
        transform.position += currentOffset;
    }

    private void drawTargetPoint() {
       targetUI.transform.position = target.position;
    }

}
