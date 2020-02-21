using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Rumble
{
    public static float rumble = 0;

    public static IEnumerator BurstRumble(float intensity, float time)
    {
        Gamepad.current.SetMotorSpeeds(intensity, intensity);
        rumble = intensity;
        yield return new WaitForSeconds(time);
        Clear();
    }

    public static IEnumerator BurstRumbleContinuous(float intensity, float time, float intensity_continuous)
    {
        Gamepad.current.SetMotorSpeeds(intensity, intensity);
        rumble = intensity;
        yield return new WaitForSeconds(time);
        Gamepad.current.SetMotorSpeeds(intensity_continuous, intensity_continuous);
        rumble = intensity_continuous;
    }

    public static void SetRumble(float intensity)
    {
        Gamepad.current.SetMotorSpeeds(intensity, intensity);
        rumble = intensity;
    }

    public static void Clear()
    {
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
        rumble = 0.0f;
    }
}
