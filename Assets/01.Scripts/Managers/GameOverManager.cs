using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI killCountText;

    void Start()
    {
        // 씬이 켜지자마자, 아까 DataManager에 보관해뒀던 숫자를 가져와서 글씨로 바꿔줌
        killCountText.text = "처치한 적: " + DataManager.killCount + " 마리";
    }
}
