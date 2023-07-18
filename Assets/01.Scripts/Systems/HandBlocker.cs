using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBlocker : MonoBehaviour
{
    [SerializeField]  float coolOffTime;
    public Collider hbCollider;
    protected IEnumerator coolOffCollider()
    {
        hbCollider.enabled = false;
        yield return new WaitForSeconds(coolOffTime);
        hbCollider.enabled = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!hbCollider)
            hbCollider = GetComponent<Collider>();
    }
    public void CoolOff()
    {
        StartCoroutine(coolOffCollider());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
