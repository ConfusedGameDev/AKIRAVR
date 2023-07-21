using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameConfigMG : MonoBehaviour
{
    public bikeManager playerBike;
    public List<NPCBike> npcBikes;
    
    [Button]
    void Start()
    {
        if (!playerBike || npcBikes.Count <= 0) return;
        var data = Application.streamingAssetsPath + "/Config.txt";
        if (File.Exists(data))
        {
            var txt= File.ReadAllLines(data);
            if(txt.Length>=5)
            {
                foreach (var item in npcBikes)
                {
                    item.maxGas = getData(txt[0]);
                    item.minDistToPlayer = getData(txt[1]);
                }
                playerBike.setMaxSpeed(getData(txt[2]));
                playerBike.setSteeringAngle(getData(txt[3]));
                playerBike.setGasSpeed(getData(txt[4]));


            }

        }
        else
        {
            Debug.LogWarning("No Config File Found");
        }
    }

    private float getData(string txt)
    {
        float value = 0;
        var Data = txt.Split(':');
        if (Data.Length == 2)
        {
            float.TryParse(Data[1], out value);
        }
        return value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
