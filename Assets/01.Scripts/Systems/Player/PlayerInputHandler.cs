using KVRL.KVRLENGINE.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem.XR.Haptics;

public class PlayerInputHandler : SingletonComponent<PlayerInputHandler>
{
    [BoxGroup("Default Rumble Parameters")]
    [Tooltip("Defines The number of times the Controller with rumble when there are no parameters specified")]
    public int defaultNumberOfRumbles = 7;
    [BoxGroup("Default Rumble Parameters")]
    [Tooltip("Defines The time between Rumbles in a Periodic Rumble Call")]
    public float defaultSpacing = 0.25f;
    [BoxGroup("Default Rumble Parameters")]
    [Tooltip("The Channel of The Vibration (currently only 0 is acceptable except for oculus)")]
    public int defaultChannel = 0;
    [BoxGroup("Default Rumble Parameters")]
    [Tooltip("Defines Minimum and maximum Power of the vibration (0->1f)")]
    [MinMaxSlider(0, 1f)]
    public Vector2 defaultAmplitude = new Vector3(0.1f, 1f);
    [BoxGroup("Default Rumble Parameters")]
    [Tooltip("Defines The Duration of the rumble when no parameters are specified")]
    public float defaultDuration = 0.1f;

    public HandController leftHand, rightHand;
    public void RumbleHand(HandController hand, int times, float spacing)
    {
        StartCoroutine(RumbleHandPeriodically(hand, times, spacing));
    }
    IEnumerator RumbleHandPeriodically(HandController hand, int times, float spacing)
    {
        int timesRumbled = 0;
        while (timesRumbled < times)
        {
            timesRumbled++;
            yield return new WaitForSeconds(spacing);
            RumbleHand(hand);
        }
    }
    public void TestVibration(HandController hand)
    {
        RumbleHand(hand);
    }
    public void RumbleHand(HandController hand)
    {
        SendHaptics(hand, defaultChannel, UnityEngine.Random.Range(defaultAmplitude.x, defaultAmplitude.y), defaultDuration);
    }
    UnityEngine.InputSystem.InputDevice GetHandController(HandType hand)
    {
        return hand == HandType.Left
            ? XRController.leftHand
            : XRController.rightHand;
    }
    private void SendHaptics(HandController hand, int channel, float amplitude, float duration)
    {
        if (hand.handT== HandType.Both)
        {
            var leftController = GetHandController(HandType.Left);
            var rightController = GetHandController(HandType.Right);
            var leftCommand = SendHapticImpulseCommand.Create(channel, amplitude, duration);
            var rightCommand = SendHapticImpulseCommand.Create(channel, amplitude, duration);
            leftController?.ExecuteCommand(ref leftCommand);
            rightController?.ExecuteCommand(ref rightCommand);
        }
        else
        {
            var controller = GetHandController(hand.handT);
            var command = SendHapticImpulseCommand.Create(channel, amplitude, duration);
            controller?.ExecuteCommand(ref command);
        }
    }
}
