using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject splashScreen, playMenu;
    public GameObject[] UnrankedLevel;
    public GameObject[] RankedLevel;
    public GameObject LevelTypeTillBy4, LevelTypeTillBy9;
    public GameObject rankLevelTypeTillBy4, rankLevelTypeTillBy9;
    private int currentLevel = 0;
    public Text LevelText,LevelText2;
    public Text rankLevelText, rankLevelText2;
    public GameObject[] timerCheckButton,timerCheckButton2;
    public GameObject[] ranktimerCheckButton, ranktimerCheckButton2;
    public Button PlayButton1, PlayButton2;
    public Button rankPlayButton1, rankPlayButton2;

    private UnrankedScoreDataHandler unrankedScoreDataHandler;
    public Text unrankedBestScoretill4;
    public Text unrankedBestScoretill9;

    public GameObject unrankedScorePrefab;
    public GameObject unrankedLeaderBoardContainer;

    public AudioSource backgroundMusic;
    public AudioSource clickMusic;
    public Slider audioSlider;
    public Text audioSliderTExt;

    public static MainMenuHandler instance;
    void Start()
    {
        instance = this;
        StartCoroutine(HideSplashScreen());

        var json = PlayerPrefs.GetString("unrankedscores", "{}");
        unrankedScoreDataHandler = JsonUtility.FromJson<UnrankedScoreDataHandler>(json) ?? new UnrankedScoreDataHandler();
        if (unrankedScoreDataHandler.unrankedScores == null)
            unrankedScoreDataHandler.unrankedScores = new List<ScoresData>();

        var scores = GetHighScores().ToArray();
        if (scores.Length > 0)
        {
            unrankedBestScoretill4.text = "Best Score : "+scores[0].scoreValue.ToString();
            unrankedBestScoretill9.text = "Best Score : " + scores[0].scoreValue.ToString();

            for (int i = 0; i < scores.Length; i++)
            {
                var unrankedScore = Instantiate(unrankedScorePrefab, unrankedLeaderBoardContainer.transform);
                unrankedScore.transform.GetChild(0).gameObject.GetComponent<Text>().text = scores[i].levelname;
                unrankedScore.transform.GetChild(1).gameObject.GetComponent<Text>().text = scores[i].scoreValue.ToString();
                unrankedScore.transform.GetChild(2).gameObject.GetComponent<Text>().text = scores[i].scoreDate;

            }
        }
        else
        {
            unrankedBestScoretill4.text = "0";
            unrankedBestScoretill9.text = "0";
        }

        SetAudioValueAtStart();

    }

    IEnumerator HideSplashScreen()
    {
        yield return new WaitForSeconds(1);
        splashScreen.SetActive(false);
        playMenu.SetActive(true);
    }

    public void LaunchScene()
    {
        SceneManager.LoadScene(1);
    }
    public void LaunchRankedScene()
    {
        SceneManager.LoadScene(2);
    }
    public void UnrankedGameStart() { currentLevel = 0; ShowLevel(false); }
    public void RankedGameStart() { currentLevel = 1; ShowLevel(true); }

    public void UnrankedNextButton() { MoveLevel(false, 1); }
    public void UnrankedPreviousButton() { MoveLevel(false, -1); }
    public void RankedNextButton() { MoveLevel(true, 1); }
    public void RankedPreviousButton() { MoveLevel(true, -1); }

    private void MoveLevel(bool ranked, int direction)
    {
        GameObject[] levels = ranked ? RankedLevel : UnrankedLevel;
        int count = Mathf.Min(ranked ? 8 : 3, levels.Length);
        if (count == 0) return;
        currentLevel = (currentLevel + direction + count) % count;
        ShowLevel(ranked);
    }

    private void ShowLevel(bool ranked)
    {
        GameObject[] levels = ranked ? RankedLevel : UnrankedLevel;
        if (levels.Length == 0) return;
        currentLevel = Mathf.Clamp(currentLevel, 0, levels.Length - 1);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);

        int size = currentLevel + 2;
        string label = $"{size}*{size}";
        Text first = ranked ? rankLevelText : LevelText;
        Text second = ranked ? rankLevelText2 : LevelText2;
        first.text = label;
        second.text = label;

        bool large = currentLevel > 2;
        (ranked ? rankLevelTypeTillBy4 : LevelTypeTillBy4).SetActive(!large);
        (ranked ? rankLevelTypeTillBy9 : LevelTypeTillBy9).SetActive(large);
        for (int i = 0; i < levels.Length; i++) levels[i].SetActive(i == currentLevel);
    }

    public void SelectTimer(int timerValue) => SelectTimerOption(
        timerValue, timerCheckButton, timerCheckButton2, PlayButton1, PlayButton2);

    public void SelectRankTimer(int timerValue) => SelectTimerOption(
        timerValue, ranktimerCheckButton, ranktimerCheckButton2, rankPlayButton1, rankPlayButton2);

    private static void SelectTimerOption(int value, GameObject[] first, GameObject[] second,
        Button playFirst, Button playSecond)
    {
        int selected = value - 1;
        if (selected < 0 || selected >= first.Length || selected >= second.Length) return;
        for (int i = 0; i < first.Length; i++) first[i].SetActive(i == selected);
        for (int i = 0; i < second.Length; i++) second[i].SetActive(i == selected);
        PlayerPrefs.SetInt("CurrentGameTime", value);
        playFirst.interactable = true;
        playSecond.interactable = true;
    }

    public IEnumerable<ScoresData> GetHighScores()
    {
        return unrankedScoreDataHandler.unrankedScores.OrderByDescending(x => x.scoreValue);
    }

    public void SaveUnrankedScore()
    {
        var json = JsonUtility.ToJson(unrankedScoreDataHandler);
        PlayerPrefs.SetString("unrankedscores", json);
    }

    public void OnAudioValueChange()
    {
        backgroundMusic.volume = .01f * audioSlider.value;
        clickMusic.volume = .1f * audioSlider.value;

        PlayerPrefs.SetFloat("audioLevel", audioSlider.value);
        if (audioSlider.value > 7)
        {
            audioSliderTExt.text = "High";
        }
        else if (audioSlider.value > 4)
        {
            audioSliderTExt.text = "Medium";
        }
        else
        {
            audioSliderTExt.text = "Low";
        }
    }

    private void SetAudioValueAtStart()
    {
        audioSlider.value = PlayerPrefs.GetFloat("audioLevel", 1);
        backgroundMusic.volume = .01f * audioSlider.value;
        clickMusic.volume = .1f * audioSlider.value;
    }
}

