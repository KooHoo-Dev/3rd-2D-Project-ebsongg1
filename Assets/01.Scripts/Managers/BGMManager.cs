using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class BGMManager : MonoBehaviour
{
    [Header("효과음 전용 스피커")]
    public AudioSource sfxSource;

    public static BGMManager instance;
    private AudioSource audioSource;

    [Header("오디오 믹서 연결")]
    public AudioMixer mainMixer;

    [Header("기본 BGM 볼륨")]
    public float maxBGMVolume = 1f;

    // 아무 스크립트에서나 이거 부르면 브금 꺼짐
    public void StopBGM() { audioSource.Stop(); }

    // 아무 스크립트에서나 이거 부르면 효과음 재생됨
    public void PlaySFX(AudioClip clip) { sfxSource.PlayOneShot(clip); }

    public void Awake()
    {
        // 씬이 넘어가도 파괴되지 않게 싱글톤 유지
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>(); // AudioSource 부품 찾아오기
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (mainMixer != null)
        {
            mainMixer.SetFloat("BGMVolume", 0f);
            mainMixer.SetFloat("SFXVolume", 0f);
        }
    }

    // 아무 스크립트에서나 이 함수를 부르면 브금이 바뀜
    public void PlayBGM(AudioClip newClip)
    {
        // 틀어달라고 한 브금이 이미 재생 중인 브금이랑 똑같으면 무시 (처음부터 다시 재생되는 거 방지)
        if (audioSource.clip == newClip) return;

        if (mainMixer != null)
        {
            mainMixer.SetFloat("BGMVolume", 0f);
        }

        audioSource.Stop();       // 기존 음악 끄기

        audioSource.volume = maxBGMVolume;

        audioSource.clip = newClip; // 새 음악 테이프 갈아 끼우기
        audioSource.Play();       // 재생
    }


    public void CrossFadeBGM(AudioClip newClip, float fadeTime = 2f)
    {
        // 똑같은 음악이면 무시
        if (audioSource.clip == newClip) return;

        // 부드럽게 음악을 갈아끼우는 코루틴 시작
        StartCoroutine(FadeRoutine(newClip, fadeTime));
    }

    IEnumerator FadeRoutine(AudioClip newClip, float fadeTime)
    {
        // 기존 음악 볼륨 서서히 줄이기 (Fade Out)
        float timer = 0f;
        float startVolume = audioSource.volume;

        while (timer < fadeTime / 2f)
        {
            timer += Time.deltaTime;
            // 서서히 0으로 만들기
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / (fadeTime / 2f));
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        // 새 음악 틀기
        audioSource.clip = newClip;
        audioSource.Play();

        // 새 음악 볼륨 서서히 키우기 (Fade In)
        timer = 0f;
        while (timer < fadeTime / 2f)
        {
            timer += Time.deltaTime;
            // 서서히 원래 볼륨으로 키우기
            audioSource.volume = Mathf.Lerp(0f, maxBGMVolume, timer / (fadeTime / 2f));
            yield return null;
        }

        // 혹시 모르니 목표 볼륨으로 확실하게 고정
        audioSource.volume = maxBGMVolume;
    }

    public void FadeOutBGM(float fadeTime = 1.5f)
    {
        StartCoroutine(FadeOutRoutine(fadeTime));
    }

    IEnumerator FadeOutRoutine(float fadeTime)
    {
        float timer = 0f;
        float startVolume = audioSource.volume; // 현재 볼륨 기억하기

        // 지정한 시간(fadeTime) 동안 서서히 볼륨을 0으로 깎아내림
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
            yield return null;
        }

        // 볼륨이 0이 되면 완전히 정지
        audioSource.volume = 0f;
        audioSource.Stop();

        // 나중에 다른 음악 틀 때를 대비해서 볼륨을 다시 원래대로 복구해둠
        audioSource.volume = maxBGMVolume;
    }

}
