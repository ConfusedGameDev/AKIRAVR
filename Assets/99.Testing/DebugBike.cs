using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBike : MonoBehaviour
{
    public Rigidbody rb;
    public WheelCollider wheelF, wheelB;
    [Range(-1,1)]
    public float steer;
    [Range(0, 1)]
    public float gas;
    public float maxAngle = 30;
    public float maxTorque = 500;
    public float maxHoverForce = 1f;
    private void FixedUpdate()
    {
        rb.AddForceAtPosition(9.81f * rb.mass *maxHoverForce*Vector3.up, transform.position+ rb.centerOfMass);
        rb.velocity=transform.forward * maxTorque * gas;
        transform.rotation = Quaternion.Euler(0, steer * maxAngle, 0);
        Debug.Log(Mathf.PingPong(Time.time, 1f));
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnDrawGizmos()
    {
        if (!rb) Start();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position+ rb.centerOfMass, 0.3f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
