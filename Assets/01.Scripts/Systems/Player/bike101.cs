using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bike101 : MonoBehaviour
{
   
    public Rigidbody rb;
    public Transform LeftForceEmmitter, RightForceEmmitter;
    public WheelCollider frontWheelC, backWheelC;
    public Transform frontWheelT, backWheelT;

    public float motorTorque, brakeTorque;
    public Vector3 input;//X Horizontal Movement Y Vertical Movement Z Brake
    //puStart is called before the first frame update
    public float maxSteerAngle = 30;
    public Transform cameraController;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(!rb ||! frontWheelC || !backWheelC)
        {
            //we will Control manually this rotation
           
            Debug.LogError("No wheel Colliders or Rigidbody Found ");
            Destroy(this);
        }
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    //Run Logic On the Fixed Update cause it is a Physics Based Vehicle
    private void FixedUpdate()
    {
        //Get Input
        input = GetInput();
        //Update Engine
        UpdateEngine();

        //Update Steering
        UpdateSteering();

        //Update Z-Axis Rotation
        CorrectRotationZ();
        //push down The Bike

    }
    //TODO-> replace for unity Actions
    public Vector3 GetInput() => new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 
        Input.GetKeyDown(KeyCode.Space) ? 1 : 0);

    //Only Back tire 
    public void UpdateEngine()
    {
        backWheelC.motorTorque = input.y* motorTorque;
        frontWheelC.brakeTorque = input.z*brakeTorque;
        backWheelC.brakeTorque = input.z*brakeTorque;
    }

    public void UpdateSteering()    
    {
        frontWheelC.steerAngle = input.x * maxSteerAngle;
        
    }

    public void CorrectRotationZ()
    {
        var currentRotation=transform.rotation;
        currentRotation.z = 0;
        transform.rotation = currentRotation;
    }



}
