using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // 오디오 믹서

public class OptionMenuManager : MonoBehaviour
{
    [Header("오디오 믹서")]
    public AudioMixer mainMixer;

    [Header("UI 슬라이더")]
    public Slider sfxSlider;
    public Slider bgmSlider; 
     

    public void Start()
    {
        // 슬라이더의 최소/최대값 세팅 
        // 유니티 믹서는 로그 스케일(-80dB ~ 0dB)을 쓰기 때문에 0을 쓰면 에러가 남. 0.0001이 최소임
        bgmSlider.minValue = 0.0001f;
        bgmSlider.maxValue = 1f;

        sfxSlider.minValue = 0.0001f;
        sfxSlider.maxValue = 1f;

        // 슬라이더 기본값을 1(최대 볼륨)로 둠
        bgmSlider.value = 1f;
        sfxSlider.value = 1f;
    }

    // Music 슬라이더가 움직일 때 실행될 함수
    public void SetBGMVolume(float volume)
    {
        // Mathf.Log10을 써야 슬라이더가 중간에 있을 때 소리가 갑자기 확 줄어드는 현상을 막을 수 있음
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }

    // Sound 슬라이더가 움직일 때 실행될 함수
    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    // Ok 버튼용 함수
    public void OnClickOkButton()
    {
        // 옵션창 닫기
        gameObject.SetActive(false);
    }
}
