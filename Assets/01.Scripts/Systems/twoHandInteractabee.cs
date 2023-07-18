using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twoHandInteractabee : MonoBehaviour
{
    public Interactablee leftHandInteractable, rightHandInteractable;
    public bool isGrabbed;

    public void checkIsGrabbed()
    {
        isGrabbed = leftHandInteractable.isGrabbed && rightHandInteractable.isGrabbed;
        if(isGrabbed)
        {
            Grab();
        }
        if (isGrabbed)
        {
            Release();
        }
    }

    public virtual void Grab()
    {

    }
    public virtual void Release()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if(leftHandInteractable)
        {
            leftHandInteractable.onGrab.AddListener(checkIsGrabbed);
            leftHandInteractable.onRelease.AddListener(checkIsGrabbed);
        }
        if (leftHandInteractable)
        {
            rightHandInteractable.onGrab.AddListener(checkIsGrabbed);
            rightHandInteractable.onRelease.AddListener(checkIsGrabbed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
