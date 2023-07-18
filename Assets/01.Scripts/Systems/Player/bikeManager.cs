using KVRL.KVRLENGINE.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bikeManager : MonoBehaviour
{

    public List<NPCBike> npcBikes=new List<NPCBike>();
    public NPCBike closestTarget;
    public Vector3 input;//X Horizontal Movement Y Vertical Movement Z Brake
    public InputActionAsset inputMap;
    private InputAction horizontalMovementAction;
    private InputAction VertiaclMovementAction;
     public float maxSteerAngle = 30;
    public Transform cameraController;
    public Transform rotZController;
    public Transform leftHandT, rightHandT, bikeroationController;
    public float handAngle;
    public UIController uiController;
    public float maxSpeed=40f;

    public Interactablee leftHandle, rightHandle;
    public FollowObject zRotationHandler;
    

    public float steer = 0;
    public float startRotation = 90;
    public float maxAngle = 30;
    public float maxTorque = 500;
    public float maxHoverForce = 1f;
    public BicycleVehicle bikeVehicle;
    public float lastSteer = 0;
    [Range(0f, 1f)]
    public float steerSpeed = 0.05f;
    [Range(0f, 1f)]
    public float maxSteer = 0.5f;

    public float steeringRange = 26f;

    public bool isBikeOn;
    public float remainingGas = 100f;

    public BikeSoundManager bikeSoundMG;
    
    void Start()
    {
        if (inputMap != null)
        {
            horizontalMovementAction = inputMap.FindAction("DebugMoveHorizontal");
            VertiaclMovementAction = inputMap.FindAction("DebugMoveVertical");


        }
        
       
        if(zRotationHandler)
        zRotationHandler.automatic = false;
        rightHandle.ToggleGrabability(false);
        leftHandle.ToggleGrabability(false);

        npcBikes.AddRange(FindObjectsByType<NPCBike>(FindObjectsSortMode.InstanceID));

    }

    [Button]
    public void StartBike()
    {

        bikeSoundMG.StartBike();
        isBikeOn = true;
        rightHandle.ToggleGrabability(true);
        leftHandle.ToggleGrabability(true);
    }
    public NPCBike getClosestEnemy()
    {
        if (npcBikes.Count == 0) return null;
        if (npcBikes.Count == 1) return npcBikes[0];
        var minDist = Vector3.Distance(transform.position, npcBikes[0].transform.position);
        int closestTargetIndex = 0;
        for (int i = 1; i < npcBikes.Count; i++)
        {
            var currentDist = npcBikes[i].getDistance(transform);
            if (currentDist < minDist)
            {
                closestTargetIndex = i;
                minDist = currentDist;
            }
        }
        return npcBikes[closestTargetIndex];
    }
    [ShowInInspector]
    bool isBikeGrabbed { get => rightHandle && leftHandle && rightHandle.isGrabbed && leftHandle.isGrabbed; }
    public float debugGas = 1f;
    //Run Logic On the Fixed Update cause it is a Physics Based Vehicle
    private void FixedUpdate()
    {
        if (!isBikeOn)
            return;

        remainingGas -= Time.deltaTime * gasDepletingSpeed;
        closestTarget = getClosestEnemy();
        
        UpdateEngine();

        
         if (uiController)
        {
            uiController.updateSpeed((debugGas + input.y) * bikeVehicle.getMaxSpeed());
            uiController.UpdateGas(remainingGas);
            if (closestTarget)
            {
                uiController.UpdateEnemyData(closestTarget.getDistance(transform), npcBikes.Count);
            }
        }
         if(bikeSoundMG)
        {
            bikeSoundMG.updateMotorVolume(Input.GetAxis("Vertical") + input.y);
        }
        

    }

    public float getTfromLerp(float v0, float v1, float v2)
    {
        float v1MinusV0 = v1 - v0;
        float t = (v2 - v0) / v1MinusV0;
        return t;
    }
    //TODO-> replace for unity Actions
    public Vector3 GetInput()
    {
        return new Vector3(horizontalMovementAction!=null? horizontalMovementAction.ReadValue<float>(): Input.GetAxis("Horizontal"),
                           VertiaclMovementAction != null ? VertiaclMovementAction.ReadValue<float>(): Input.GetAxis("Vertical"),
                            Input.GetKeyDown(KeyCode.Space) ? 1 : 0);
    }

    public void updateGas(float x)
    {
        input.y = x;
    }
    

    

   
    public void UpdateEngine()
    {
        if (zRotationHandler)
            zRotationHandler.automatic = isBikeGrabbed;
        if (isBikeGrabbed)
        {
            steer = getTfromLerp(zRotationHandler.rotationLimitZ.x, zRotationHandler.rotationLimitZ.y, getSignedAngle(zRotationHandler.transform.rotation.eulerAngles.z));
            
            steer = -Mathf.Lerp(-1, 1, steer);
            float fakeSteer = steeringRange * steer;
            lastSteer =  Mathf.Lerp(lastSteer, steer, steerSpeed);
            //bikeVehicle.UpdateInput(input.y,Mathf.Min(Mathf.Max( lastSteer,-maxSteer),maxSteer));
            bikeVehicle.UpdateInput(input.y, fakeSteer);

        }
        else
        {
            bikeVehicle.UpdateInput(0,0);
        }
    }
    float getSignedAngle(float angle)
    {
        return angle > 180 ? angle - 360 : angle;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

    }
    public float XRSteering;
    public void UpdateSteering()    
    {
        XRSteering = (bikeroationController ? ((bikeroationController.rotation.eulerAngles.z > 180.0f )? 
                                                    bikeroationController.rotation.eulerAngles.z - 360 : bikeroationController.rotation.eulerAngles.z) / 50f :0);
       // frontWheelC.steerAngle = -XRSteering * maxSteerAngle;
        CorrectRotationZ();
        
    }
    public float rotz;
    [SerializeField] float gasDepletingSpeed=1f;

    public void  UpdateGas(float addition)
    {
        remainingGas += addition;
    }
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
