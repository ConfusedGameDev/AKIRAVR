using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.VFX;
[ExecuteAlways]
public class UIController : MonoBehaviour
{
    public Disc discSpeedmeter;
    public Line lineSpeedMeter, lineSpeedMeterBG, GasMeter;
    public TextMeshPro speedText;
    public TextMeshPro enemiesLabel;
    [PropertyRange(0, "speedThreshold")]
    public float currentSpeed;

    public float maxDiscSpeed=90;
    public float maxSpeed=1.01f;
    public float maxLineSpeed = 1.01f;
    public float visualSpeed = 999;
    public VisualEffect SpeedLines;
    public float maxLineGas = 1.01f;
    [Range(0, 100f)]
    public float currentGas = 100f;

    [Header("Animation")] public AnimationCurve chargeFillCurve;
    public AnimationCurve animChargeShakeMagnitude = AnimationCurve.Linear(0, 0, 1, 1);
    [Range(0, 0.05f)] public float chargeShakeMagnitude = 0.1f;
    public float chargeShakeSpeed = 1;
     public AnimationCurve shakeAnimX = AnimationCurve.Constant(0, 1, 0);
    public AnimationCurve shakeAnimY = AnimationCurve.Constant(0, 1, 0);

    public float speedLinesMax;
    public string speedLinesRadiusID;

    [MinMaxSlider(-5f, 5f)]
    public Vector2 speedLinesRadius = new Vector2(-5f, 0f);

    public float speedThreshold = 600f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updateDiscSpeed(float newSpeed)
    {
        currentSpeed = Mathf.Min(newSpeed, maxDiscSpeed);
        discSpeedmeter.AngRadiansEnd = Mathf.Deg2Rad*currentSpeed;
        maxSpeed = maxDiscSpeed;
        
    }
    public void updateLineSpeed(float newSpeed, float speedThreshold)
    {
        var speed = Mathf.Clamp01( newSpeed/speedThreshold)*maxLineSpeed;
        lineSpeedMeter.End= new Vector3(speed, lineSpeedMeter.End.y,lineSpeedMeter.End.z);
        maxSpeed = maxLineSpeed;
        float chargeAnim = chargeFillCurve.Evaluate(speed / maxSpeed);
        float chargeMag = animChargeShakeMagnitude.Evaluate(chargeAnim) * chargeShakeMagnitude;
        Vector2 origin = GetShake(chargeShakeSpeed, chargeMag); // do shake here
       
        lineSpeedMeter.End = new Vector3(lineSpeedMeter.End.x+ origin.x, origin.y, 0);
        if(lineSpeedMeterBG)
        lineSpeedMeterBG.End = new Vector3(maxLineSpeed+origin.x  , origin.y, 0);
    }

    public Vector2 GetShake(float speed, float amp)
    {
        float shakeVal = ShapesMath.Frac(Time.time * speed);
        float shakeX = shakeAnimX.Evaluate(shakeVal);
        float shakeY = shakeAnimY.Evaluate(shakeVal);
        return new Vector2(shakeX, shakeY) * amp;
    }

    void UpdateGasLine()
    {
        if (!GasMeter) return;
        GasMeter.End = new Vector3( maxLineGas * (currentGas / 100f), GasMeter.End.y,GasMeter.End.z);
        GasMeter.Color = Color.Lerp(Color.red, Color.green, currentGas / 100f);

    }

    // Update is called once per frame
    void Update()
    {

        if (speedText)
            speedText.text = string.Format("{0:D3}", ((int)((currentSpeed / speedThreshold) * visualSpeed)));
        if (SpeedLines)
        {
            SpeedLines.enabled = currentSpeed > 0;
            if(SpeedLines.HasFloat(speedLinesRadiusID))
            SpeedLines.SetFloat(speedLinesRadiusID, Mathf.Lerp(speedLinesRadius.x,speedLinesRadius.y, currentSpeed / maxSpeed));
        }
        if (lineSpeedMeter)
        {
            updateLineSpeed(currentSpeed, speedThreshold);
        }
        UpdateGasLine();
 

    }
    public void updateSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }
    public void UpdateGas(float newGas)
    {
        currentGas = newGas;
    }
    public void UpdateEnemyData(float dist,int remaining)
    {
        if(enemiesLabel)
        {
            enemiesLabel.text = remaining>0? $"NEXT TARGET: {dist}M \n REMAINING TARGETS: {remaining}":"GAME OVER";
        }
    }


}
