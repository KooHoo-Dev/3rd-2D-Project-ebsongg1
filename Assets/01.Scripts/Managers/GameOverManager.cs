using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI gemScoreText;

    public void Start()
    {

        // 게임 오버 씬이 켜지자마자 DataManager에 저장된 숫자를 가져와서 글씨로 출력
        if (killCountText != null)
        {
            killCountText.text = "쓰러트린 적의 수: " + DataManager.killCount;
        }

        if (gemScoreText != null)
        {
            gemScoreText.text = "획득한 보석 점수: " + DataManager.gemScore;
        }
    }

    // [다시 도전하기] 버튼용 함수 
    public void OnClickRetry()
    {
        // 다시 시작할 때는 킬카운트를 0으로 리셋해줘야 함
        DataManager.killCount = 0;
        DataManager.gemScore = 0;
        SceneManager.LoadScene("Scenes_Stage_01");
    }

    // [타이틀로 이동] 버튼용 함수
    public void OnClickTitle()
    {
        DataManager.killCount = 0;
        DataManager.gemScore = 0;
        SceneManager.LoadScene("Scenes_Title");
    }
}
