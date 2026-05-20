using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.FinalFinalFinalGame);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
