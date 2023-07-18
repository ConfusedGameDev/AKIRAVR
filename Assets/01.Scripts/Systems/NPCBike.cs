using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBike : MonoBehaviour
{
    public Vector3 getPosition() => transform.position;
    public float getDistance(Transform origin) => Vector3.Distance(origin.position, transform.position);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
