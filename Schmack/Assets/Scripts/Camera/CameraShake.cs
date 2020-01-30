using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCam;
    CinemachineBasicMultiChannelPerlin noise;

    // Start is called before the first frame update
    void Start()
    {
        //virtualCam = GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        noise = virtualCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartNoiseShake(float amplitude, float frequency)
    {
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
    }

    public void EndNoiseShake()
    {
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 1;
    }
}
