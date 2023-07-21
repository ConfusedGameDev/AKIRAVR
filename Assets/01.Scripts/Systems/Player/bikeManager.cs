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

    internal void setMaxSpeed(float v)
    {
        if(bikeVehicle && v>0)
        {
            bikeVehicle.setSpeed(v);
        }
    }

    internal void setGasSpeed(float v)
    {
        if (v != 0)
            gasDepletingSpeed = v;
        
    }

    internal void setSteeringAngle(float v)
    {
        if(v!=0)
        steeringRange = v;
    }

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

    public GameObject winScreen,looseScreen;
    bool gameOver;
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
        if (npcBikes.Count == 0)
        {
            gameOver = true;
            StartCoroutine(Fade(true, true));

            return null;
        }
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
        if (!isBikeOn || bikeVehicle.remainingGas<=0 || gameOver)
            return;

        if (!bikeVehicle.isAiControlled)
        {
            bikeVehicle.remainingGas -= Time.deltaTime * gasDepletingSpeed;
            if (bikeVehicle.remainingGas <= 0)
            {
                StartCoroutine(Fade(true, false));
                return;
            }
        }

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
                StartCoroutine(Fade(false,false));
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
                    if (npcBikes.Count<=0)
                    {
                        StartCoroutine(Fade(true,true));

                    }
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
    IEnumerator Fade(bool GameOver=false,bool win =false)
    {
        if(GameOver)
        {
            if(win)
            {
                winScreen.SetActive(true);
            }
            else
            {
                looseScreen.SetActive(true);
            }
            fadeSpeed *= 0.15f;
        }
        var delta = 1f;
        FadeStart();
        while (delta>0)
        {
            fadeSphere.material.SetFloat(fadeID, Mathf.Lerp(1, 0, delta));
            yield return new WaitForEndOfFrame();
            delta -= Time.deltaTime * fadeSpeed;
        }
        if(GameOver)
        yield return new WaitForSeconds(1f);
        FadeMidPoint(GameOver);
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
    public void FadeMidPoint(bool resetGame=false)
    {
        if(resetGame)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        bikeVehicle.ResetBike();
    }
    public void FadeFinish()
    {
        bikeVehicle.EnableMotor();
        isResseting = false;
    }





}
