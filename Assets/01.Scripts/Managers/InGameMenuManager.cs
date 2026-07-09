using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject menuPanel;   // 반투명 배경이 포함된 전체 메뉴 패널
    public GameObject optionPanel; // 옵션 창

    private bool isPaused = false;

    void Start()
    {
        // 시작할 때는 인게임 메뉴가 무조건 꺼져있게
        if (menuPanel != null) menuPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
    }

    void Update()
    {
        // 키보드 ESC를 누르면 메뉴를 껐다 켰다 할 수 있게
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // 메뉴 켜기 & 시간 정지
    public void PauseGame()
    {
        isPaused = true;
        if (menuPanel != null) menuPanel.SetActive(true);

        Time.timeScale = 0f; 
    }

    // 1. Continue 버튼 
    public void ResumeGame()
    {
        isPaused = false;
        if (menuPanel != null) menuPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false); // 옵션창도 같이 끄기

        Time.timeScale = 1f; 
    }

    // 2. New Game 버튼 
    public void OnClickNewGame()
    {
        // 씬 넘어가기 전에 무조건 시간을 1로 돌려놓기
        Time.timeScale = 1f;
        DataManager.killCount = 0;

        // 현재 활성화된 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 3. Option 버튼
    public void OnClickOption()
    {
        if (optionPanel != null)
        {
            // 옵션창이 꺼져있으면 켜고, 켜져있으면 끄는 스위치 역할
            optionPanel.SetActive(!optionPanel.activeSelf);
        }
    }

    // 4. Exit 버튼
    public void OnClickExit()
    {
        Time.timeScale = 1f;
        DataManager.killCount = 0;

        SceneManager.LoadScene("Scenes_Title"); 
    }
}