using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeComingMisile : MonoBehaviour
{
    public float Speed = 1f;
    public Transform target;
    public float timeLife = 10f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,timeLife);
    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            transform.LookAt(target);
            transform.position += transform.forward * Speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("NPC")) return;
        Debug.Log("Crachsed with" + other.name);
        NPCBike bike = other.transform.getComponentInParent<NPCBike>();
        if(bike)
        {            
                bike.DestroyBike(true);
            Destroy(gameObject);
        }
    }

    
}

public static  partial class akiraExtensions
{
    public static T getComponentInParent<T>(this  Transform g) where T : MonoBehaviour
    {
        T result;
        if (g.TryGetComponent<T>(out result))
        {
            return result;
        }
        else if (g.parent)
        {
            return getComponentInParent<T>(g.parent);
        }
        return null;
    }
}
