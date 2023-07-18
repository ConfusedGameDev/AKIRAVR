using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableRotator : Interactablee
{
    // Start is called before the first frame update
    public float initialRotationZ;
    public float rotationZDelta;
    public float maxRotation = 1f;

    public UnityEvent<float> updateInput;
    public override void Grab(HandController newHand)
    {
        base.Grab(newHand);
        initialRotationZ = newHand.transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        if(isGrabbed && currentHand)
        {
            rotationZDelta = currentHand.transform.rotation.eulerAngles.z - initialRotationZ;
            updateInput.Invoke(Mathf.Clamp01(rotationZDelta / maxRotation));

        }
    }
}
