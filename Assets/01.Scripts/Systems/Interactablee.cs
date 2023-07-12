using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactablee : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject fakeHand;
    public bool isGrabbed;
    public bool requireGrip;
    public bool isDistancedBased;
    [ShowIf("isDistancedBased")]
    public float maxDistance = 0.5f;

    protected HandController currentHand;
    public HandType compatibleHand;
    protected void Awake()
    {
        if (fakeHand)
            fakeHand.SetActive(false);
    }
    //Implement Parent To Object
    public void ToggleFakeHand(bool toggle, HandController hand=null)
    {
        
        if (!fakeHand) return;
            
        if (toggle )
        {
            Grab(hand);
          
        }
        else if(!toggle)
        {
            Release();
        }

    }
    public virtual void Grab(HandController newHand)
    {
        if (compatibleHand != HandType.Both && newHand && newHand.handT != compatibleHand) return;
        currentHand = newHand;
        isGrabbed = true;
        fakeHand.SetActive(true);
    }
    public virtual void Release()
    {
        currentHand = null;
        isGrabbed = false;
        fakeHand.SetActive(false);
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
