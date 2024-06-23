using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    [CanBeNull][SerializeField] private Canvas startCanvas;
    [CanBeNull][SerializeField] private Canvas htpCanvas;

    private bool isDisplayHTP = false;
    public void LoadScene(int indexScene)
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(indexScene);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadHTP()
    {
        if (!isDisplayHTP)
        {
            startCanvas.enabled = false;
            htpCanvas.enabled = true;
            isDisplayHTP = true;
        }
        else
        {
            startCanvas.enabled = true;
            htpCanvas.enabled = false;
            isDisplayHTP = false;
        }
    }
}
