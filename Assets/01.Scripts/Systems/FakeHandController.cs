using KVRL.KVRLENGINE.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FollowObject))]
public class FakeHandController : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 originalPosition;
    Quaternion originalRotation;
    FollowObject motionController;
    public HandType handType;
    private void Awake()
    {
        motionController = GetComponent<FollowObject>();
        motionController.parent = handType == HandType.Left ? PlayerInputHandler.Instance.leftHand.transform : PlayerInputHandler.Instance.rightHand.transform;
    }
    private void OnEnable()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        if (motionController.parent)
            motionController.automatic = true;

    }
    private void OnDisable()
    {
        transform.localPosition=originalPosition;
        transform.localRotation=originalRotation ;
        motionController.automatic = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
