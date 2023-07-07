using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using static SplineFollower;
using static UnityEngine.Splines.SplineComponent;

 
public class BikeSplineMovement : MonoBehaviour
{
    [SerializeField, Tooltip("The target spline to follow.")]
    SplineContainer splineTarget,nextTarget;
    public int splineIndex;
    [SerializeField, Tooltip("The coordinate space that the GameObject's up and forward axes align to.")]
    AlignmentMode m_AlignmentMode = AlignmentMode.SplineElement;

    [SerializeField, Tooltip("Which axis of the GameObject is treated as the forward axis.")]
    AlignAxis m_ObjectForwardAxis = AlignAxis.ZAxis;

    [SerializeField, Tooltip("Which axis of the GameObject is treated as the up axis.")]
    AlignAxis m_ObjectUpAxis = AlignAxis.YAxis;
    readonly float3[] m_AlignAxisToVector = new float3[] { math.right(), math.up(), math.forward(), math.left(), math.down(), math.back() };

    [Range(0f,1f)]
    public float Speed = 0f;
    [Range(0f, 1f)]
    public float currentTime;

    Vector3 position;
    Quaternion rotation;
    public Vector3 Offset;

    public void UpdateSplineTarget(SplineContainer newTarget)
    {
        if (newTarget == null || splineTarget== newTarget) return;
        splineTarget = null;
        nextTarget = newTarget;
        currentTime= 0;
        StartCoroutine(GotoSplineStartPosition());

    }
    IEnumerator GotoSplineStartPosition()
    {
        var delta = 0f;
        Vector3 currentPos, destinyPosition;
        Quaternion currentRot,destinyRotation;
        UpdateTransform(out destinyPosition, out destinyRotation, currentTime, nextTarget);
        currentPos = transform.position;
        currentRot = transform.rotation;
        while (delta<1f)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(currentPos, destinyPosition, delta);
            transform.rotation = Quaternion.Lerp(currentRot, destinyRotation, delta);
            delta +=  Speed;
        }
        delta = 1;
        transform.position = Vector3.Lerp(currentPos, destinyPosition, delta);
        transform.rotation = Quaternion.Lerp(currentRot, destinyRotation, delta);
        splineTarget = nextTarget;
        nextTarget = null;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    protected float3 GetAxis(AlignAxis axis)
    {
        return m_AlignAxisToVector[(int)axis];
    }
    public void UpdateTransform(out Vector3 position, out Quaternion rotation, float t, SplineContainer target)
    {
        position = target.EvaluatePosition(target[splineIndex], t);

        rotation = Quaternion.identity;

        // Correct forward and up vectors based on axis remapping parameters
        var remappedForward = GetAxis(m_ObjectForwardAxis);
        var remappedUp = GetAxis(m_ObjectUpAxis);
        var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

        if (m_AlignmentMode != AlignmentMode.None)
        {
            var forward = Vector3.forward;
            var up = Vector3.up;

            switch (m_AlignmentMode)
            {
                case AlignmentMode.SplineElement:
                    forward = Vector3.Normalize(target.EvaluateTangent(target[splineIndex], t));
                    up = target.EvaluateUpVector(target[splineIndex], t);
                    break;

                case AlignmentMode.SplineObject:
                    var objectRotation = target.transform.rotation;
                    forward = objectRotation * forward;
                    up = objectRotation * up;
                    break;

                default:
                    Debug.Log($"{m_AlignmentMode} animation aligment mode is not supported!");
                    break;
            }

            rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;
        }
        else
            rotation = axisRemapRotation;
    }
    // Update is called once per frame
    void Update()
    {
        if (!splineTarget) return;
        UpdateTransform(out position, out rotation, currentTime,splineTarget);
        transform.position = position;

        if (m_AlignmentMode != AlignmentMode.None)
            transform.rotation = rotation;
        transform.localPosition += transform.right * Offset.x;
        currentTime += Speed*Time.deltaTime;
    }
}
