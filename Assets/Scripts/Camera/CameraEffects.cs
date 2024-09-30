using Cinemachine;
using Core;
using UnityEngine;

/*
 * Allow for certain camera-based effects to be played. e.g Camera Sway or Camera Shake
 */

public class CameraEffects : MonoBehaviour {
    public Vector2 cameraWorldSize;
    public CinemachineFramingTransposer cinemachineFramingTransposer;
    [SerializeField] private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;
    public float screenYDefault;
    public float screenYTalking;

    [Range(0, 10)] [HideInInspector] public float shakeLength = 10;

    [SerializeField] public CinemachineVirtualCamera _virtualCamera;

    public void Start() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();

        cinemachineFramingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        screenYDefault               = cinemachineFramingTransposer.m_ScreenX;

        Player.Instance.cameraEffects = this;
        _multiChannelPerlin           = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _virtualCamera.Follow = Player.Instance.transform;
    }

    public void Update() {
        _multiChannelPerlin.m_FrequencyGain +=
            (0 - _multiChannelPerlin.m_FrequencyGain) * Time.deltaTime * (10 - shakeLength);
    }

    public void Shake(float shake, float length) {
        shakeLength                         = length;
        _multiChannelPerlin.m_FrequencyGain = shake;
    }
}