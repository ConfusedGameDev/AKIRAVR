using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineCollider : MonoBehaviour
{
    public
    SplineContainer splineTargetFwd, splineTargetLeft, splineTargetRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SplineContainer getNextSpline(float hInput)
    {
        if(splineTargetRight && hInput>0.5f)
        {
            return splineTargetRight;
        }
        else if (splineTargetLeft && hInput < -0.5f)
        {
            return splineTargetLeft;
        }
        return splineTargetFwd;

    }
}
