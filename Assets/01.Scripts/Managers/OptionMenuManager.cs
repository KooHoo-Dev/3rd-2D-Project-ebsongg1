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

    [Header("끌 옵션창 패널 ")]
    public GameObject optionPanel;


    public void Start()
    {
 
    }

    // Music 슬라이더가 움직일 때 실행될 함수
    public void SetBGMVolume(float volume)
    {
        // (Mathf.Log10을 써야 슬라이더가 중간에 있을 때 소리가 갑자기 확 줄어드는 현상을 막을 수 있음)
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
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
    }
}
