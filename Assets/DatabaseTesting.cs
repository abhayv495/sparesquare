using UnityEngine;

public class DatabaseTesting : MonoBehaviour
{
}

[System.Serializable]
public class RankedScoresData
{
    public string userId;
    public string userName;
    public string levelname;
    public int scoreValue;
    public string scoreDate;
    public int averagescore;

    public RankedScoresData(string userId,string userName, string levelname, int scoreValue, string scoreDate, int averagescore)
    {
        this.userId = userId;
        this.userName = userName;
        this.levelname = levelname;
        this.scoreValue = scoreValue;
        this.scoreDate = scoreDate;
        this.averagescore = averagescore;
    }
}

[System.Serializable]
public class RankedScoresLogin
{
    public string userId;
    public string userName;

    public RankedScoresLogin(string userId, string userName)
    {
        this.userId = userId;
        this.userName = userName;

    }
}
