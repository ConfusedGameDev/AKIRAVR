using KVRL.KVRLENGINE.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bike101 : MonoBehaviour
{
   
    public Rigidbody rb;
    public Transform LeftForceEmmitter, RightForceEmmitter;
    public WheelCollider frontWheelC, backWheelC;
    public Transform frontWheelT, backWheelT;

    public float motorTorque, brakeTorque;
    public Vector3 input;//X Horizontal Movement Y Vertical Movement Z Brake
    public InputActionAsset inputMap;
    private InputAction horizontalMovementAction;
    private InputAction VertiaclMovementAction;
    //puStart is called before the first frame update
    public float maxSteerAngle = 30;
    public Transform cameraController;
    public Transform rotZController;
    public Transform leftHandT, rightHandT, bikeroationController;
    public float handAngle;
    public UIController uiController;
    public float maxSpeed=40f;

    public Interactablee leftHandle, rightHandle;
    public FollowObject zRotationHandler;
    void Start()
    {
        if (inputMap != null)
        {
            horizontalMovementAction = inputMap.FindAction("DebugMoveHorizontal");
            VertiaclMovementAction = inputMap.FindAction("DebugMoveVertical");


        }
        rb = GetComponent<Rigidbody>();

        if(!rb ||! frontWheelC || !backWheelC)
        {
            //we will Control manually this rotation
           
            Debug.LogError("No wheel Colliders or Rigidbody Found ");
            Destroy(this);
        }
        frontWheelC.ConfigureVehicleSubsteps(5, 12, 15);
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        if(zRotationHandler)
        zRotationHandler.automatic = false;
    }

    //Run Logic On the Fixed Update cause it is a Physics Based Vehicle
    private void FixedUpdate()
    {
        //Get Input
        input = GetInput();
        //Update Engine
        UpdateEngine();

        //Update Steering
         if (zRotationHandler)
                zRotationHandler.automatic = (rightHandle && leftHandle && rightHandle.isGrabbed && leftHandle.isGrabbed);
            UpdateSteering();
        

        
        //push down The Bike
        if (uiController)
        {
            uiController.updateLineSpeed(rb.velocity.magnitude,maxSpeed);
        }
        

    }
    //TODO-> replace for unity Actions
    public Vector3 GetInput()
    {
        return new Vector3(horizontalMovementAction!=null? horizontalMovementAction.ReadValue<float>(): Input.GetAxis("Horizontal"),
                           VertiaclMovementAction != null ? VertiaclMovementAction.ReadValue<float>(): Input.GetAxis("Vertical"),
                            Input.GetKeyDown(KeyCode.Space) ? 1 : 0);
    }

    //Only Back tire 
    public void UpdateEngine()
    {
        backWheelC.motorTorque = input.y* motorTorque;
        frontWheelC.brakeTorque = input.z*brakeTorque;
        backWheelC.brakeTorque = input.z*brakeTorque;
    }
    public float XRSteering;
    public void UpdateSteering()    
    {
        XRSteering = (bikeroationController ? ((bikeroationController.rotation.eulerAngles.z > 180.0f )? 
                                                    bikeroationController.rotation.eulerAngles.z - 360 : bikeroationController.rotation.eulerAngles.z) / 50f :0);
        frontWheelC.steerAngle = -XRSteering * maxSteerAngle;
        CorrectRotationZ();
        
    }
    public float rotz;
    public void CorrectRotationZ()
    {
        var currentRotation=transform.rotation;
        currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x,
            currentRotation.eulerAngles.y, 0);
        //rotZController ? rotz*rotZController.rotation.eulerAngles.z : 0);

        
         transform.rotation = currentRotation;
        if (leftHandT && rightHandT)
        {
            var target = rightHandT.position - leftHandT.position;
            handAngle = Vector3.Angle(target, leftHandT.forward);
        }
 
    }
    



}
