using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRPushButton : Interactablee
{
    public Vector3 startPosition, endPosition;

    Vector3 startGrabPosition, GrabPositionDelta;
    [Button]
    public void saveStartPos() => startPosition = transform.localPosition;
    [Button]
    public void saveEndPos() => endPosition = transform.localPosition;

    [Button]
    public void ResetPos() => transform.localPosition= startPosition;

    [FoldoutGroup("Constrains")]
    public bool parentX;
    [FoldoutGroup("Constrains")]
    public bool parentY;
    [FoldoutGroup("Constrains")]     
    public bool parentZ;

    public UnityEvent onClick;

    public AudioClip onClickSound;
    public bool isMultiPress = true;
    public void Awake()
    {
        base.Awake();
        ResetPos();
    }
    public override void Grab(HandController newHand)
    {
        base.Grab(newHand);
        startGrabPosition = newHand.transform.localPosition; 
    }

    public override void Release()
    {
        base.Release();
        ResetPos();
    }
    public void Click()
    {
        onClick?.Invoke();
        if(currentHand)
        {
            currentHand.tryReleaseInteractable(this);
        }
        coolOffGrabability(3f);
        if(audioPlayer && onClickSound)
        {
            audioPlayer.PlayOneShot(onClickSound);
        }
        if(!isMultiPress)
        {
            ToggleGrabability(false);
        }
        
    }
    public void Update()
    {
        if(isGrabbed && currentHand)
        {
            GrabPositionDelta = currentHand.transform.localPosition-startGrabPosition;
            Debug.Log(GrabPositionDelta);
            Vector3 newPosition= transform.localPosition+GrabPositionDelta;
            newPosition.x =Mathf.Min( Mathf.Max(newPosition.x, Mathf.Min(startPosition.x, endPosition.x)), Mathf.Max(startPosition.x, endPosition.x));
            newPosition.y = Mathf.Min(Mathf.Max(newPosition.y, Mathf.Min(startPosition.y, endPosition.y)), Mathf.Max(startPosition.y, endPosition.y));
            newPosition.z = Mathf.Min(Mathf.Max(newPosition.z, Mathf.Min(startPosition.z, endPosition.z)), Mathf.Max(startPosition.z, endPosition.z));

            transform.localPosition = new Vector3(parentX ? newPosition.x : transform.localPosition.x,
                                                  parentY ? newPosition.y : transform.localPosition.y,
                                                  parentZ ? newPosition.z : transform.localPosition.z);
            if(parentZ && Mathf.Abs(transform.localPosition.z-endPosition.z)<0.1f)
            {
                Click();
            }
        }
    }
}
