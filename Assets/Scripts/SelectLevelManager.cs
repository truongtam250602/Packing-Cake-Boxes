using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelManager : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private Sprite starActiveSprite;

    private int levelUnlocked;
    void Start()
    {
        if(!PlayerPrefs.HasKey("Level Unlocked"))
        {
            PlayerPrefs.SetInt("Level Unlocked", 1);
        }
        levelUnlocked = PlayerPrefs.GetInt("Level Unlocked");
        LoadMenuLevel();
    }

    private void LoadMenuLevel()
    {
        for(int i = 0; i < levelButtons.Length; i++)
        {
            if (i <= levelUnlocked)
            {
                levelButtons[i].transform.GetChild(0).gameObject.SetActive(true); // TMP
                levelButtons[i].transform.GetChild(1).gameObject.SetActive(false); // Image
                levelButtons[i].GetComponent<Button>().enabled = true;
                int stars = PlayerPrefs.HasKey("Stars Of Level " + i) ? PlayerPrefs.GetInt("Stars Of Level " + i) : 0;
                LoadHighScore(stars, levelButtons[i].transform.GetChild(2));
            }
            else
            {
                levelButtons[i].transform.GetChild(0).gameObject.SetActive(false);
                levelButtons[i].transform.GetChild(1).gameObject.SetActive(true);
                levelButtons[i].GetComponent<Button>().enabled = false;
            }
        }
    }

    private void LoadHighScore(int stars, Transform parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (i < stars)
            {
                parent.GetChild(i).GetComponent<Image>().sprite = starActiveSprite;
            }
        }
    }
}
