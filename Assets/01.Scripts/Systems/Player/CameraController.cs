using KVRL.KVRLENGINE.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [FoldoutGroup("Camera Setup")]
    [SerializeField] Transform headPosition, cameraTransform, cameraParent;
    public float reseterDelay = 3f;
    public TMPro.TextMeshPro cameraResetLabel;
    public GameObject mapUI;
    public GameObject startGameLabel;
    public VRPushButton StartButton;
    [SerializeField] bikeManager bikeMG;

    [Button]
    public void ResetCamera()
    {
        if(mapUI &&  headPosition && cameraTransform && cameraParent && cameraResetLabel)
        StartCoroutine(ResetCameraCO());
    }
    IEnumerator ResetCameraCO()
    {
        mapUI.SetActive(false);

        cameraResetLabel.gameObject.SetActive(true);
        if (!bikeMG.isBikeOn)
        {
            startGameLabel.SetActive(false);
            StartButton.ToggleGrabability(false);
        }
        var delta = reseterDelay;
        while(delta>0)
        {
            cameraResetLabel.text = "Reseting Camera In" + "\n" + delta.ToString();
            yield return new WaitForSeconds(1f);
            delta--;
        }
        cameraResetLabel.gameObject.SetActive(false);
        CameraReseter.ResetCamera(headPosition, cameraTransform, cameraParent, false);
        if(bikeMG.isBikeOn)
        {
            mapUI.SetActive(true);
        }        
        else
        {
            StartButton.ToggleGrabability(true);

            startGameLabel.SetActive(true);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
