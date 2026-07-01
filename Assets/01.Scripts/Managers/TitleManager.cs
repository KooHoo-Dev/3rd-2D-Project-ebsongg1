using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("Scenes_Stage_01");
    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }
}