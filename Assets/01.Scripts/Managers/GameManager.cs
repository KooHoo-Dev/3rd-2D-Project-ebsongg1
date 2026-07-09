using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 게임 어디서든 GameManager.instance 로 접근할 수 있게 해줌
    public static GameManager instance;

    [Header("다회차 데이터")]
    public int clearCount = 0; // 현재 클리어 횟수 

    public void Awake()
    {
        // 싱글톤 세팅 및 씬 이동 시 파괴 방지
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // 씬이 넘어가도 안 부서지고 유지
        }
        else
        {
            // (이미 매니저가 존재하는데 다른 씬에서 또 생기려고 하면 짝퉁을 파괴)
            Destroy(gameObject);
        }

        // 게임 켜질 때 저장된 회차 데이터 불러오기
        LoadGameData();
    }

    public void Update()
    {
        // 키보드 숫자 9를 누르면 강제로 클리어 횟수 증가
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            AddClearCount();
            Debug.Log("[치트키 발동] 강제로 클리어 횟수를 1 올렸습니다!");
        }

        // 키보드 숫자 0을 누르면 데이터 초기화 (다시 1회차로)
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetData();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    // [데이터 불러오기]
    public void LoadGameData()
    {
        // "ClearCount"라는 이름표로 저장된 숫자를 가져옴 (저장된 게 아예 없으면 기본값 0)
        clearCount = PlayerPrefs.GetInt("ClearCount", 0);
        Debug.Log("현재 클리어 횟수: " + clearCount);
    }

    // [보스 클리어 시 호출할 함수]
    public void AddClearCount()
    {
        clearCount++; // 클리어 횟수 1 증가
        PlayerPrefs.SetInt("ClearCount", clearCount); // PlayerPrefs에 기록
        PlayerPrefs.Save(); // 디스크에 확실하게 도장(이걸 해야 강종해도 안 날아감)

        Debug.Log("게임 클리어! 누적 클리어 횟수: " + clearCount);
    }

    // [테스트용: 데이터 초기화]
    // 2회차 연출 테스트하다가 다시 1회차로 돌리고 싶을 때 
    public void ResetData()
    {
        PlayerPrefs.DeleteKey("ClearCount");
        clearCount = 0;
        Debug.Log("클리어 데이터 초기화 완료! 다시 1회차로 돌아갑니다.");
    }
}
