using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class BikeController : MonoBehaviour
{
    public float horizontalMovementSpeed = 1f;
    public InputActionAsset inputMap;
    private InputAction horizontalMovementAction;
    public BikeSplineMovement movementController;
    public float hInput;
    public void MoveHorizontally(float input )
    {        
            Debug.Log(input);
        if (movementController)
        {
            movementController.Offset.x = input*horizontalMovementSpeed;
        }
        hInput = input;

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
