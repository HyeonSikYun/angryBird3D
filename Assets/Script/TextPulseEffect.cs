using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextPulseEffect : MonoBehaviour
{
    public float minScale = 1f; // 최소 크기
    public float maxScale = 1.2f; // 최대 크기
    public float pulseSpeed = 1f; // 펄스 속도

    public TextMeshProUGUI textComponent;
    private bool isScalingUp = true;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        // 시작할 때 텍스트 크기 초기화
        textComponent.transform.localScale = Vector3.one * minScale;
    }

    void Update()
    {
        // 크기 변화 처리
        float scale = Mathf.PingPong(Time.time * pulseSpeed, 1f) * (maxScale - minScale) + minScale;
        textComponent.transform.localScale = Vector3.one * scale;
    }
}