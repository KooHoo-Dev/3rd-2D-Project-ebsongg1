using UnityEngine;
using TMPro;

public class GemUI : MonoBehaviour
{
    [Header("보석 UI 그룹 (이미지 + 글씨 묶음)")]
    public GameObject gemUIPanel;

    [Header("보석 점수 텍스트")]
    public TextMeshProUGUI inGameGemText;

    public void Start()
    {
        // 씬이 시작될 때, 보석 점수가 0점이면 UI 전체를 숨겨버리기
        if (DataManager.gemScore == 0 && gemUIPanel != null)
        {
            gemUIPanel.SetActive(false);
        }
    }

    public void Update()
    {
        // 보석 점수가 1 이상인데 UI가 아직 꺼져있다면? -> 켜주기
        if (DataManager.gemScore > 0 && gemUIPanel != null && !gemUIPanel.activeSelf)
        {
            gemUIPanel.SetActive(true);
        }

        // UI가 켜져 있을 때만 점수 글씨를 업데이트해 주기
        if (gemUIPanel != null && gemUIPanel.activeSelf && inGameGemText != null)
        {
            // ": 10" 형식으로 출력
            inGameGemText.text = ": " + DataManager.gemScore;
        }
    }
}
