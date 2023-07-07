using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class SplineSampler : MonoBehaviour
{
    [System.Serializable]
    public struct SamplePosition
    {
        public float3 center;
        public float3 right;
        public float3 left;

        public SamplePosition(float3 center, float3 right, float3 left)
        {
            this.center = center;
            this.right = right;
            this.left = left;
        }
    }
    public List<SamplePosition> samples= new List<SamplePosition>();
    [SerializeField]
    public SplineContainer splineContainer;
    [SerializeField]
    private int splineIndex;
    [SerializeField]
    [Range(0f,1f)]
    private float time;
  
    public float maxWidth;
   
    public int resolution = 10;
    public float debugSphereSize = 0.25f;

    public bool debug;
    // Update is called once per frame
    void Update()
    {
         
        samples.Clear();

        if (!splineContainer) return;
        for (int i = 0; i < resolution; i++)
        {
            float3 position, tangent, upVector;
            float3 p1, p2;
            splineContainer.Evaluate(splineIndex, i/(float)resolution, out position, out tangent, out upVector);
            float3 right = Vector3.Cross(tangent, upVector).normalized * maxWidth / 2f;
            p1 = position + (right * maxWidth / 2f);
            p2 = position + (-right * maxWidth / 2f);
            position.y = 0;
            p1.y = 0;
            p2.y = 0;
            samples.Add(new SamplePosition(position, p1, p2));

        }

    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!debug) return;
        foreach (var sample in samples)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(sample.center, debugSphereSize);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(sample.left, debugSphereSize);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(sample.right, debugSphereSize);

            Gizmos.DrawLine(sample.left, sample.right) ;
        }
      

    }
#endif
}
