using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Speedup,
    GasUp,

}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]

public class Pickup : MonoBehaviour
{
    public AudioSource audioPlayer;
    public AudioClip onGrabAudio;
    public float rotatingSpeed= 1f;
    public float CoolOffTime = 10f;
    public GameObject mesh;
    public SphereCollider col;
    public PickupType typePickup;
    public float value = 2f;
    public void OnTriggerEnter(Collider other)
    {
        BikeVehicle bMG;
        if (other.transform.parent.TryGetComponent<BikeVehicle>(out bMG))
        {
            if(!bMG.isAiControlled)
            {
                bMG.applyPowerup(this,typePickup);
                OnTouch();
            }

        }
    }
    public void OnTouch()
    {
        if(audioPlayer && onGrabAudio)
        {
            audioPlayer.PlayOneShot(onGrabAudio);
        }
        StartCoroutine(coolOff());
    }
    public virtual void Execute()
    {

    }

    IEnumerator coolOff()
    {
        if (mesh)
            mesh.SetActive(false);
        if (col)
            col.enabled = false;
        yield return new WaitForSeconds(CoolOffTime);
        if (mesh)
            mesh.SetActive(true);
        if (col)
            col.enabled = true;


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotatingSpeed, 0);
    }
}
