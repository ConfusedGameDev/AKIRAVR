using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Interactablee currentInteractable;
    public GameObject handMesh;

    public float currentGrip = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Interactablee newInteractable;
        if (other.TryGetComponent<Interactablee>(out newInteractable))
        {
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
    private void OnTriggerExit(Collider other)
    {
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
        if (currentInteractable)
        {
            if (currentInteractable.isGrabbed)
            {
                CheckForRelease();
            }
            else if(currentInteractable.requireGrip)
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
