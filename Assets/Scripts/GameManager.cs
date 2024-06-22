using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private enum State { compeleted, failed, playing}
    public static GameManager Instance;
    [SerializeField] private Canvas completedCanvas;
    [SerializeField] private Image[] starsCollected;
    [SerializeField] private Sprite spriteStarCompleted;
    [SerializeField] private Canvas failedCanvas;

    private State state;
    private float elapsedTime;
    private int stars = 0;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        state = State.playing;
    }
    void Update()
    {
        if(state == State.playing)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void WinState()
    {
        state = State.compeleted;
        stars = CalculateStars(elapsedTime);
        SaveHighScore();
        for(int i = 0; i < starsCollected.Length; i++)
        {
            if (i <= stars - 1)
            {
                starsCollected[i].sprite = spriteStarCompleted;
            }
        }
        StartCoroutine(ActiveGameCanvas(completedCanvas));
    }

    public void FailState()
    {
        state = State.failed;
        StartCoroutine(ActiveGameCanvas(failedCanvas));
    }

    private IEnumerator ActiveGameCanvas(Canvas canvas)
    {
        yield return new WaitForSeconds(0.6f);
        canvas.enabled = true;
        Time.timeScale = 0f;
    }

    private int CalculateStars(float time)
    {
        if (time <= 10f)
        {
            return 3;
        }
        else if (time <= 20)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private void SaveHighScore()
    {
        int level = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;

        if(PlayerPrefs.HasKey("Stars Of Level " + level))
        {
            if(stars > PlayerPrefs.GetInt("Stars Of Level " + level, stars))
            {
                PlayerPrefs.SetInt("Stars Of Level " + level, stars);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Stars Of Level " + level, stars);
        }

        Debug.Log(level +":"+stars);

        if(PlayerPrefs.HasKey("Level Unlocked"))
        {
            PlayerPrefs.SetInt("Level Unlocked", level + 1);
        }
        PlayerPrefs.Save();
    }
}
