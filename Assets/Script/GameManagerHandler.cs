using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerHandler : MonoBehaviour
{
    private bool rotateSet, rotateButtonPressed;
    public GameObject[] currentSet;
    public Quaternion tempRotation;
    public GameObject currentBackground;
    public Vector3[] tempPosition;
    public float rotationSpeed = 2f;

    [Header("PatternData")]
    public List<GamePattern> gamePatterns = new List<GamePattern>();

    public Text currentTime;
    public string currentLevel;
    public int moves;
    public Text MovesText;
    public float timeRemaining;

    private string minutes, seconds;
    private int lastDisplayedSecond = int.MinValue;
    public Text currentScore;
    public int score;
    public static GameManagerHandler instance;

    [Header("LeveData")]
    public int levelNumber;
    public bool finiteTime;
    private bool isPlaying;

    [Header("PopUps")]
    public Text discountinuePopUpTimer;
    public GameObject discontineuPopUp;
    public Text timeoutPopUpTimer;
    public GameObject timeOutPopUp;

    private UnrankedScoreDataHandler unrankedScoreDataHandler;
    public Text submitPanelScore;
    public Text submitPanelTime;
    public Text submitPanelMoves;
    public Text timeOutScore;
    public Text timeOutTime;

    public AudioSource backgroundMusic;
    public AudioSource clickMusic;
    public AudioClip switchaudio, rotateaudio;

    void Start()
    {
        instance = this;

        rotateSet = false;
        rotateButtonPressed = true;
        moves = 0;
        MovesText.text = moves.ToString();
        score = 0;
        currentScore.text = score.ToString();
        isPlaying = true;

        if (3 >= PlayerPrefs.GetInt("CurrentGameTime", 1))
        {
            timeRemaining = PlayerPrefs.GetInt("CurrentGameTime", 1) * 60;
            currentTime.text = timeRemaining.ToString();
            finiteTime = true;

        }
        else
        {
            currentTime.text = "Infinity";
            finiteTime = false;

        }

        var json = PlayerPrefs.GetString("unrankedscores", "{}");
        unrankedScoreDataHandler = JsonUtility.FromJson<UnrankedScoreDataHandler>(json) ?? new UnrankedScoreDataHandler();
        if (unrankedScoreDataHandler.unrankedScores == null)
            unrankedScoreDataHandler.unrankedScores = new List<ScoresData>();

        backgroundMusic.volume = .01f * PlayerPrefs.GetFloat("audioLevel", 1);
        clickMusic.volume = .1f * PlayerPrefs.GetFloat("audioLevel", 1);

    }

    void Update()
    {

        if (isPlaying)
        {
            if (rotateSet == true)
            {

                float blend = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);
                for (int i = 0; i < 4; i++)
                    currentSet[i].transform.localPosition = Vector3.Lerp(
                        currentSet[i].transform.localPosition, tempPosition[(i + 1) % 4], blend);
                currentBackground.transform.rotation = Quaternion.Slerp(currentBackground.transform.rotation, tempRotation, blend);

                if ((currentSet[0].transform.localPosition - tempPosition[1]).sqrMagnitude < 0.0001f)
                {
                    for (int i = 0; i < 4; i++)
                        currentSet[i].transform.localPosition = tempPosition[(i + 1) % 4];
                    currentBackground.transform.rotation = tempRotation;
                    rotateButtonPressed = true;
                    rotateSet = false;

                }
            }

            if (finiteTime == true)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining = Mathf.Max(0f, timeRemaining - Time.deltaTime);

                    int displayedSecond = Mathf.CeilToInt(timeRemaining);
                    if (displayedSecond != lastDisplayedSecond)
                    {
                        lastDisplayedSecond = displayedSecond;
                        int displayedMinutes = displayedSecond / 60;
                        int displayedSeconds = displayedSecond % 60;
                        currentTime.text = $"{displayedMinutes:00}:{displayedSeconds:00}";
                    }
                }
                else
                {
                    isPlaying = false;
                    timeOutPopUp.SetActive(true);

                    minutes = Mathf.Floor((PlayerPrefs.GetInt("CurrentGameTime", 1) * 60 )/ 60).ToString("00");
                    seconds = ((PlayerPrefs.GetInt("CurrentGameTime", 1) * 60) % 60).ToString("00");
                    timeoutPopUpTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                    currentTime.text = "00:00";

                    submitPanelScore.text = score.ToString();
                    submitPanelMoves.text = moves.ToString();
                    submitPanelTime.text = "0"+PlayerPrefs.GetInt("CurrentGameTime", 1).ToString()+":00";

                    timeOutScore.text = "Your time is up. Your score is "+score.ToString();
                    timeOutTime.text = submitPanelTime.text;

                }
            }
        }

    }

    public void CurrentSetAnchorPress(GameObject currentTile)
    {
        if (!rotateButtonPressed || !isPlaying) return;
        rotateSet = true;
        rotateButtonPressed = false;
        Transform tile = currentTile.transform;
        for (int i = 0; i < 4; i++)
        {
            currentSet[i] = tile.GetChild(i).gameObject;
            tempPosition[i] = currentSet[i].transform.localPosition;
            currentSet[i].GetComponent<PatternData>().ChangePosition();
        }
        currentBackground = tile.GetChild(4).gameObject;
        tempRotation = Quaternion.Euler(0, 0, currentBackground.transform.eulerAngles.z + 90);
        currentTile.GetComponent<Dot>().FindCompleteBoardScoreFromRotate();
        CountMoves();
        clickMusic.PlayOneShot(rotateaudio);
    }

    public void CountScore(int m_score)
    {
        score = score + m_score;
        currentScore.text = score.ToString();
    }

    public void CountMoves()
    {
        moves++;
        MovesText.text = moves.ToString();
    }
    public void LaunchScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void SwitchSound()
    {
        clickMusic.PlayOneShot(switchaudio);
    }
    public void OnStopPress()
    {
        if (timeRemaining > 0)
        {
            isPlaying= false;
            discontineuPopUp.SetActive(true);

            float TotalTimeTaken = (PlayerPrefs.GetInt("CurrentGameTime", 1) * 60) - timeRemaining;
            minutes = Mathf.Floor(TotalTimeTaken / 60).ToString("00");
            seconds = (TotalTimeTaken% 60).ToString("00");
            discountinuePopUpTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if(PlayerPrefs.GetInt("CurrentGameTime", 1)==4)
            {
                discountinuePopUpTimer.text = "Infinity";
            }

            submitPanelScore.text = score.ToString();
            submitPanelMoves.text = moves.ToString();
            submitPanelTime.text = discountinuePopUpTimer.text;
        }

    }

    public void OnSubmitScore()
    {
        currentLevel = (PlayerPrefs.GetInt("CurrentLevel", 0) + 2).ToString() + "x" + (PlayerPrefs.GetInt("CurrentLevel", 0) + 2).ToString();

        System.DateTime dt = System.DateTime.Now;
        string dateString = dt.ToString("dd MM yyyy ");

        AddScore(new ScoresData(currentLevel, score,dateString));
        SaveUnrankedScore();
    }

    public void OnRankSubmitScore()
    {
        PlayerPrefs.SetInt("ScoreValue", score);
        RankedSubmitScoreHandler.instance.SubmitScoreRanked();
    }

    public void OnResumePress()
    {
        isPlaying = true;
        discontineuPopUp.SetActive(false);

    }

    #region LeaderBoard Unranked

    public IEnumerable<ScoresData> GetHighScores()
    {
        return unrankedScoreDataHandler.unrankedScores.OrderByDescending(x => x.scoreValue);
    }

    public void AddScore(ScoresData scoress)
    {
       unrankedScoreDataHandler.unrankedScores.Add(scoress);
    }

    public void SaveUnrankedScore()
    {
        var json = JsonUtility.ToJson(unrankedScoreDataHandler);
        PlayerPrefs.SetString("unrankedscores", json);
    }

    #endregion

}

[System.Serializable]
public class GamePattern
{
    public string patternName;
    public PatternDetails patternData;
}
[System.Serializable]
public class PatternDetails
{
    public GameObject[] TimeSet;

}

[System.Serializable]
public class ScoresData
{
    public string levelname;
    public int scoreValue;
    public string scoreDate;

    public ScoresData(string levelname, int scoreValue, string scoreDate)
    {
        this.levelname = levelname;
        this.scoreValue = scoreValue;
        this.scoreDate = scoreDate;
    }
}

