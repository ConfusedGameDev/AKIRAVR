using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeSplineCollider : MonoBehaviour
{
    public BikeSplineMovement movementController;
    public BikeController bController;
    public SplineCollider currentSCol;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {         
        SplineCollider col;
        if(other.TryGetComponent<SplineCollider>(out col))
        {
            currentSCol = col;            
        }
    }
    public void OnTriggerExit(Collider other)
    {
        SplineCollider col;
        if (other.TryGetComponent<SplineCollider>(out col))
        {
            if(col == currentSCol && bController && movementController)
            {   
                movementController.UpdateSplineTarget(currentSCol.getNextSpline(bController.hInput));
                col.gameObject.SetActive(false);
                currentSCol = null;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
