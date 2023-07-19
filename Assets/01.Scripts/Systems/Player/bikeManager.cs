using KVRL.KVRLENGINE.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bikeManager : MonoBehaviour
{
    //TODO
    /*
     * Polish Rubber band System
     * ADd game over
     * add game win
     * add tutorial
     * add Powerups
     */

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
    public BikeVehicle bikeVehicle;
    public float lastSteer = 0;
    [Range(0f, 1f)]
    public float steerSpeed = 0.05f;
    [Range(0f, 1f)]
    public float maxSteer = 0.5f;

    public float steeringRange = 26f;

    public bool isBikeOn;
   

    public BikeSoundManager bikeSoundMG;
    public Renderer fadeSphere;
    int fadeID = Shader.PropertyToID("_Alpha");
    public float fadeSpeed = 1f;
    bool isResseting;
    Quaternion defaultRotZRotation;
   
    void Start()
    {
        if (inputMap != null)
        {
            horizontalMovementAction = inputMap.FindAction("DebugMoveHorizontal");
            VertiaclMovementAction = inputMap.FindAction("DebugMoveVertical");
        }

        if (zRotationHandler)
        {
            zRotationHandler.automatic = false;
            defaultRotZRotation = zRotationHandler.transform.rotation;
        }
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

        if(!bikeVehicle.isAiControlled)
        bikeVehicle.remainingGas -= Time.deltaTime * gasDepletingSpeed;
        closestTarget = getClosestEnemy();
        
        UpdateEngine();

        
         if (uiController)
        {
            uiController.updateSpeed( bikeVehicle.getCurrentSpeed() * bikeVehicle.getMaxSpeed());
            uiController.UpdateGas(bikeVehicle.remainingGas);
            if (closestTarget)
            {
                uiController.UpdateEnemyData(closestTarget.getDistance(transform), npcBikes.Count);
            }
        }
         if(bikeSoundMG)
        {
            bikeSoundMG.updateMotorVolume(bikeVehicle.getCurrentSpeed());
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
            if (zRotationHandler && zRotationHandler.transform.rotation!= defaultRotZRotation)
            {
                zRotationHandler.transform.rotation = Quaternion.Lerp(zRotationHandler.transform.rotation, defaultRotZRotation, 0.05f);
            }
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("should Dead "+ collision.transform.name);
        if (collision.transform.CompareTag("DeadZone"))
        {
            Debug.Log("Dead");
            if(!isResseting && fadeSphere && fadeSphere.material.HasProperty(fadeID))
            {
                StartCoroutine(Fade());
            }
        }
        else
        {
            NPCBike npc;
            if (collision.transform.CompareTag("NPC"))
            {
                Debug.Log("Crashed with NPC");
                if (npc = collision.transform.getComponentInParent<NPCBike>())
                {
                    Debug.Log("Killing NPC");
                    if (npcBikes.Contains(npc))
                        npcBikes.Remove(npc);
                    npc.DestroyBike(false);
                }
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        NPCBike npc;
        if (other.transform.CompareTag("NPC"))
        {
            if(npc=other.GetComponentInParent<NPCBike>())
            {
                if(npcBikes.Contains(npc))
                npcBikes.Remove(npc);
                npc.DestroyBike(false);
            }
        }
    }
    IEnumerator Fade()
    {
        
        var delta = 1f;
        FadeStart();
        while (delta>0)
        {
            fadeSphere.material.SetFloat(fadeID, Mathf.Lerp(1, 0, delta));
            yield return new WaitForEndOfFrame();
            delta -= Time.deltaTime * fadeSpeed;
        }
        FadeMidPoint();
        delta = 1f;
        while (delta > 0)
        {
            fadeSphere.material.SetFloat(fadeID, Mathf.Lerp(0, 1, delta));
            yield return new WaitForEndOfFrame();
            delta -= Time.deltaTime * fadeSpeed;
        }
        FadeFinish();

    }
    public void FadeStart()
    {
        isResseting = true;
        bikeVehicle.DisableMotor();
    }
    public void FadeMidPoint()
    {
        bikeVehicle.ResetBike();
    }
    public void FadeFinish()
    {
        bikeVehicle.EnableMotor();
        isResseting = false;
    }





}
