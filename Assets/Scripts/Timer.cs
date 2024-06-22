using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpTimer;
    [SerializeField] private float time = 45f;
    private float eslapsedTime;
    private int minutes, seconds;
    private bool failed = false;
    void Start()
    {
        eslapsedTime = time;
    }

    void Update()
    {
        if (!failed)
        {
            eslapsedTime -= Time.deltaTime;
            minutes = Mathf.FloorToInt(eslapsedTime / 60);
            seconds = Mathf.FloorToInt(eslapsedTime % 60);
            tmpTimer.text = string.Format("{00:00}:{1:00}", minutes, seconds);
            if (minutes == 0 && seconds == 0)
            {
                GameManager.Instance.FailState();
                failed = true;
            }
        }
    }
}
