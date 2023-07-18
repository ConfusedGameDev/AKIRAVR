using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public enum HandType
{
    Left,
    Right,
    Both
}
public enum UpdateMethod
{
    Update,
    LateUpdate,
    FixedUpdate
}
public class HandController : MonoBehaviour
{
    public HandType handT;
    public Interactablee currentInteractable;
    public HandBlocker currentHandBlocker;
    public GameObject handMesh;

    public float currentGrip = 0;

    public UpdateMethod updateMethod;
    public UnityEngine.SpatialTracking.TrackedPoseDriver handTracker; 

    [Button]
    void Start()
    {
        handTracker = GetComponent< UnityEngine.SpatialTracking.TrackedPoseDriver >();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currentHandBlocker) return;
        HandBlocker hb;
        if (other.TryGetComponent<HandBlocker>(out hb))
        {
            currentHandBlocker = hb;
            if (handTracker) 
                handTracker.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
            return; 

        }
        Interactablee newInteractable;
        if (other.TryGetComponent<Interactablee>(out newInteractable))
        {
            if (!newInteractable.canBeGrabbed) return;
            if (newInteractable.rumbleOnTouch)
                PlayerInputHandler.Instance.RumbleHand(this);
            if (!currentInteractable || !currentInteractable.isGrabbed)
            {
                if(currentInteractable && !currentInteractable.isGrabbed && newInteractable!= currentInteractable)
                {
                    currentInteractable.ToggleFakeHand(false);

                }
                currentInteractable = newInteractable;

                if (!currentInteractable.requireGrip)
                {
                    currentInteractable.ToggleFakeHand(true,this);
                    handMesh.SetActive(false);
                }
            }
        }
    }

    internal void tryReleaseInteractable(Interactablee interactable)
    {
        if (currentInteractable == interactable)
            ReleaseInteractable();
    }

    private void OnTriggerExit(Collider other)
    {
        if(currentHandBlocker)
        {
            HandBlocker hb; 
            if(other.TryGetComponent<HandBlocker>(out hb))
            {
                if(currentHandBlocker == hb)
                {
                    currentHandBlocker.CoolOff();
                    currentHandBlocker = null;
                    if (handTracker)
                        handTracker.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                }
            }
        }
        Interactablee newInteractable;
        if (currentInteractable && !currentInteractable.isDistancedBased && !currentInteractable.requireGrip  && other.TryGetComponent<Interactablee>(out newInteractable))
        {
            ReleaseInteractable();
        }
    }

    private void ReleaseInteractable()
    {        
        currentInteractable.ToggleFakeHand(false);
        handMesh.SetActive(true);
        currentInteractable = null;
    }
    private void GrabInteractable()
    {
        currentInteractable.ToggleFakeHand(true,this);
        handMesh.SetActive(false);
        
    }

    // TODO add Vibration and USe VR Grip/Trigger
    void Update()
    {
        if(updateMethod== UpdateMethod.Update)
        checkState();
    }
    private void FixedUpdate()
    {
        if (updateMethod == UpdateMethod.FixedUpdate)
            checkState();
    }
    private void LateUpdate()
    {
        if (updateMethod == UpdateMethod.LateUpdate)
            checkState();
    }

    private void checkState()
    {
        if (currentInteractable)
        {
            if (currentInteractable.isGrabbed)
            {
                CheckForRelease();
            }
            else if (currentInteractable.requireGrip)
            {
                CheckForGrab(currentGrip);
            }
        }
    }

    private void CheckForGrab(float currentGrip)
    {
        if(currentGrip>=1f)
        {
            GrabInteractable();
        }
        else if(currentInteractable.isDistancedBased && isInteractableOutofRange())
        {
            currentInteractable = null;
        }

    }

    private void CheckForRelease()
    {
        if (currentInteractable.isDistancedBased)
        {
            if (!currentInteractable.requireGrip)
            {
                if (isInteractableOutofRange())
                {
                    ReleaseInteractable();

                }
            }
            else if (currentGrip <= 0)
            {
                if (isInteractableOutofRange())
                {
                    ReleaseInteractable();

                }
            }
        }
        else if(currentInteractable.requireGrip)
        {
            if (currentGrip <= 0)
            {
                ReleaseInteractable();

            }
        }
    }

    private bool isInteractableOutofRange() => currentInteractable ? Vector3.Distance(transform.position, currentInteractable.transform.position) > currentInteractable.maxDistance : false;
    
}
