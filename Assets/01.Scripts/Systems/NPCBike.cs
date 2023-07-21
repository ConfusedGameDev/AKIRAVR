using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBike : MonoBehaviour
{
    public Vector3 getPosition() => transform.position;
    public float getDistance(Transform origin) => Vector3.Distance(origin.position, transform.position);

    public GameObject[] rewards;
    public GameObject deadVFX;
    public float maxGas = 0.5f;
    public float minDistToPlayer=120;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void DestroyBike(bool v)
    {
        if (v)
        {
            if (rewards.Length > 0)
            {
                var i = UnityEngine.Random.Range(0, rewards.Length * 2);
                if (i < rewards.Length)
                {
                    Destroy(GameObject.Instantiate(rewards[i], transform.position, transform.rotation), 3f);
                }
            }
        }
            if(deadVFX)
            Destroy(GameObject.Instantiate(deadVFX,transform.position,Quaternion.identity),3f);
            Destroy(gameObject);
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.transform.CompareTag("DeadZone"))
        {
            Debug.Log("NPC Resseting" + collision.transform.name);
            GetComponent<BikeVehicle>().ResetBike();
        }
        

    }
}
