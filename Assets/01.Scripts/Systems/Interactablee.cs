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
    public void ToggleFakeHand(bool toggle)
    {
        if (!fakeHand) return;
            
        if (toggle )
        {
            Grab();
          
        }
        else if(!toggle)
        {
            Release();
        }

    }
    public void Grab()
    {
        isGrabbed = true;
        fakeHand.SetActive(true);
    }
    public void Release()
    {
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
