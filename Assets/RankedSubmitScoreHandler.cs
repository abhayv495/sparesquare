using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RankedSubmitScoreHandler : MonoBehaviour
{
    public string API_URL;
    public string API_KEY;
    public string API_SECRET_TOKEN;

    private string userName;
    public static RankedSubmitScoreHandler instance;

    public GameObject rankedScorePrefab;
    public GameObject rankedLeaderBoardContainer;
    private UnityWebRequest CreateRequest(string path, string method)
    {
        var request = new UnityWebRequest(API_URL + path, method);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader(API_KEY, API_SECRET_TOKEN);
        return request;
    }

    void Start()
    {
        instance = this;
    }

    public void SubmitScoreRanked()
    {
        StartCoroutine(SubmitScoreOverApi());
    }

    IEnumerator SubmitScoreOverApi()
    {

        SubmitScoreClass scoreClass = new SubmitScoreClass();
        scoreClass.userName = PlayerPrefs.GetString("UserName","test");
        scoreClass.levelName = (PlayerPrefs.GetInt("CurrentLevel", 0) + 2).ToString() + "x" + (PlayerPrefs.GetInt("CurrentLevel", 0) + 2).ToString();
        scoreClass.scoreValue = PlayerPrefs.GetInt("ScoreValue",0);

        System.DateTime dt = System.DateTime.Now;
        string dateString = dt.ToString("yyyy-MM-dd");
        string timeString = dt.ToString("HH:mm");
        scoreClass.scoreDate = dateString;
        scoreClass.scoreTime = timeString;
        var jsonBody = JsonUtility.ToJson(scoreClass);

        using var request = CreateRequest("score/create", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log("error " + request.error);
            Debug.Log(request.downloadHandler.text);

        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log(request.downloadHandler.text);

            CreatNewUserResponse response = JsonUtility.FromJson<CreatNewUserResponse>(request.downloadHandler.text);
            Debug.Log(response.success);

            if (response.success == "true")
            {

                StartCoroutine(ScoreUpdating());
            }
            else
            {

                Debug.Log("failed ");
            }

        }
    }

    IEnumerator ScoreUpdating()
    {
        using var request = CreateRequest("leaderboard/update_score/" + PlayerPrefs.GetString("UserName", "test"), "PUT");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log("errorrr");
        }
        else
        {
            CreatNewUserResponse response = JsonUtility.FromJson<CreatNewUserResponse>(request.downloadHandler.text);
            Debug.Log(response.success);

            if (response.success == "true")
            {
                StartCoroutine(FetchBeysianScore());

            }
            else
            {

                Debug.Log("failed ");
            }
        }
    }

    IEnumerator FetchBeysianScore()
    {
        using var request = CreateRequest("leaderboard/top_50", "GET");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log("error " + request.error);
            Debug.Log(request.downloadHandler.text);

        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log(request.downloadHandler.text);
            BayesianAverageScore response = JsonUtility.FromJson<BayesianAverageScore>("{\"bayesianScoresDatas\":" + request.downloadHandler.text + "}");
            if (response?.bayesianScoresDatas == null) yield break;
            var userDatas = response.bayesianScoresDatas;

            for (int i = 0; i < userDatas.Count; i++)
            {
                var rankedScore = Instantiate(rankedScorePrefab, rankedLeaderBoardContainer.transform);
                rankedScore.transform.GetChild(0).gameObject.GetComponent<Text>().text = (i + 1).ToString();
                rankedScore.transform.GetChild(1).gameObject.GetComponent<Text>().text = userDatas[i].username;
                rankedScore.transform.GetChild(2).gameObject.GetComponent<Text>().text = ((float)userDatas[i].bayesian_average).ToString("#.##");

            }

        }
    }
}

[Serializable]
public class SubmitScoreClass
{
    public string userName;
    public string levelName;
    public int scoreValue;
    public string scoreDate;
    public string scoreTime;
}
