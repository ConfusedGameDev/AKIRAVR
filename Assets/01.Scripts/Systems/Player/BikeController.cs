using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class BikeController : MonoBehaviour
{
    public SplineFollower currentSplineF;
    public float horizontalMovementSpeed = 1f;
    public InputActionAsset inputMap;
    private InputAction horizontalMovementAction;
    public void MoveHorizontally(float input )
    {
        if(currentSplineF)
        {
            Debug.Log(input);
            
                currentSplineF.Offset.x = input * horizontalMovementSpeed;
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(inputMap!=null)
        {
           horizontalMovementAction= inputMap.FindAction("DebugMoveHorizontal");

           
        }
    }

     
    // Update is called once per frame
    void Update()
    {
        if(horizontalMovementAction!=null)
        {

            MoveHorizontally(horizontalMovementAction.ReadValue<float>());

        }
    }
}
