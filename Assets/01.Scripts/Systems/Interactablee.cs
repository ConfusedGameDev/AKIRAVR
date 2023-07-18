using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactablee : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> fakeLeftHands, fakeRightHands;
    GameObject currentFakeHand;
    public bool isGrabbed;
    public bool requireGrip;
    public bool isDistancedBased;
    [ShowIf("isDistancedBased")]
    public float maxDistance = 0.5f;

    protected HandController currentHand;
    public HandType compatibleHand;

    public UnityEvent onGrab, onRelease;
    public AudioSource audioPlayer;
    public AudioClip onGrabClip, onReleaseClip;
    public GameObject callToActionFX, onGrabFX, onReleaseFX;
    public  bool canBeGrabbed;
    public bool rumbleOnTouch, rumbleOnGrab;

    
    public void ToggleGrabability(bool toggle)
    {
        canBeGrabbed = toggle;
        if (callToActionFX)
        {
            callToActionFX.SetActive(canBeGrabbed);
        }

    }
    public void ResetHands()
    {
        foreach (var hand in fakeLeftHands)
        {
            hand.SetActive(false);
        }
        foreach (var hand in fakeRightHands)
        {
            hand.SetActive(false);
        }
    }
    protected void Awake()
    {
        ResetHands();
        if(canBeGrabbed && callToActionFX)
        {
            callToActionFX.SetActive(true);
        }
    }
    GameObject GetFakeHand()
    {

        if(currentHand)
        {
            if ( currentHand.handT == HandType.Left)
            {
                if (fakeLeftHands.Count == 0) return null;
                if (fakeLeftHands.Count == 1)
                {

                    currentFakeHand= fakeLeftHands[0];
                    return currentFakeHand;
                }

                GameObject ClosestHand = fakeLeftHands[0];
                var closestDistance = Vector3.Distance(currentHand.transform.position, ClosestHand.transform.position);
                foreach (var hand in fakeLeftHands)
                {
                    var currentHandDistance = Vector3.Distance(currentHand.transform.position, hand.transform.position);
                    if (currentHandDistance < closestDistance)
                    {
                        closestDistance = currentHandDistance;
                        ClosestHand = hand;
                    }

                }
                currentFakeHand = ClosestHand;
                return ClosestHand;
            }
            else
            {
                if (fakeRightHands.Count == 0) return null;
                if (fakeRightHands.Count == 1)
                {

                    currentFakeHand = fakeRightHands[0];
                    return currentFakeHand;
                }
                 
                GameObject ClosestHand = fakeRightHands[0];
                var closestDistance = Vector3.Distance(currentHand.transform.position, ClosestHand.transform.position);
                foreach (var hand in fakeRightHands)
                {
                    var currentHandDistance = Vector3.Distance(currentHand.transform.position, hand.transform.position);
                    if (currentHandDistance < closestDistance)
                    {
                        closestDistance = currentHandDistance;
                        ClosestHand = hand;
                    }

                }
                currentFakeHand = ClosestHand;
                return ClosestHand;
                
            }
        }
        else
        {
            currentFakeHand = null;
            ResetHands();
            
        }
        return null;
    }
    bool fakeHandsValid()
    {
        return fakeLeftHands.Count > 0 || fakeRightHands.Count > 0;
    }
    //Implement Parent To Object
    public void ToggleFakeHand(bool toggle, HandController hand=null)
    {
        
        if (!canBeGrabbed ||  !fakeHandsValid()) return;
            
        if (toggle )
        {
            Grab(hand);
          
        }
        else if(!toggle)
        {
            Release();
        }

    }
    protected IEnumerator coolOffGrabability(float coolOffTime)
    {
        canBeGrabbed = false;
        yield return new WaitForSeconds(coolOffTime);
        canBeGrabbed = true;
    }
    public virtual void Grab(HandController newHand)
    {
        if (compatibleHand != HandType.Both && newHand && newHand.handT != compatibleHand || !fakeHandsValid()) return;
        currentHand = newHand;
        isGrabbed = true;
        
        GetFakeHand().SetActive(true);
        if (callToActionFX)
        {
            callToActionFX.SetActive(false);
        }
        if (audioPlayer && onGrabClip)
        {
            audioPlayer.PlayOneShot(onGrabClip);
        }
        onGrab?.Invoke();
        if(rumbleOnGrab)
            PlayerInputHandler.Instance.RumbleHand(currentHand);

    }
    public virtual void Release()
    {        
        currentHand = null;
        isGrabbed = false;
        if(currentFakeHand)
        currentFakeHand.SetActive(false);
        if (audioPlayer && onReleaseClip)
        {
            audioPlayer.PlayOneShot(onReleaseClip);
        }
        onRelease?.Invoke();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
