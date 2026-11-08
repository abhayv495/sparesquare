using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RankedAccountController : MonoBehaviour
{

    public GameObject PlayMenuPanel;
    public GameObject LoginMainPanel;
    public GameObject RankedGamePanel;
    public GameObject LoginsuccessfullyPanel;
    public GameObject rankedScorePrefab;
    public GameObject rankedLeaderboardContainer;

    public string webClientId = "<your client id here>";
    private GoogleSignInConfiguration configuration;

    public GameObject userNamePanel;
    public GameObject loginPanel;
    public GameObject loginRequiredPanel;
    public GameObject logoutButton;

    public string API_URL;
    public string API_KEY;
    public string API_SECRET_TOKEN;
    public InputField userNameText;
    public Text loginWarningText;
    public GameObject congratulationsPopUp;
    public InputField deleteUserNameText;
    private UnityWebRequest CreateRequest(string path, string method)
    {
        var request = new UnityWebRequest(API_URL + path, method);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader(API_KEY, API_SECRET_TOKEN);
        return request;
    }

    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    private void Start()
    {
        GetBayesianScore();
        if (PlayerPrefs.GetInt("LoggedIn", 0) == 1)
        {
            logoutButton.SetActive(true);
        }

    }
    public void clickOnLoginButton()
    {
        if(PlayerPrefs.GetInt("LoggedIn",0)==0)
        {
            loginRequiredPanel.SetActive(true);
        }
        else
        {
            logoutButton.SetActive(true);
            PlayMenuPanel.SetActive(false) ;
            RankedGamePanel.SetActive(true);
            MainMenuController.instance.RankedGameStart();
        }
    }
    private void ConfigureGoogleSignIn(bool gameSignIn)
    {
        GoogleSignIn.Configuration = configuration;
        configuration.UseGameSignIn = gameSignIn;
        configuration.RequestIdToken = !gameSignIn;
        configuration.RequestEmail = true;
    }

    public void OnSignIn()
    {
        ConfigureGoogleSignIn(false);
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    public void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogException(task.Exception);
            return;
        }
        if (task.IsCanceled) return;
        PlayerPrefs.SetString("PlayerEmail", task.Result.Email);
        StartCoroutine(TurnoffPanels());
    }

    public void OnSignInSilently()
    {
        ConfigureGoogleSignIn(false);
        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        ConfigureGoogleSignIn(true);
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    public void LogoutRanked()
    {
        logoutButton.SetActive(false);
        PlayerPrefs.SetInt("LoggedIn", 0);
        OnSignOut();
    }
    IEnumerator TurnoffPanels()
    {
        yield return new WaitForSeconds(.2f);
        string email = PlayerPrefs.GetString("PlayerEmail", "");
        using var request = CreateRequest("accounts/email_exists/" + email, "GET");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.LogError(request.error);
            yield break;
        }
        CreatNewUserResponse response = JsonUtility.FromJson<CreatNewUserResponse>(request.downloadHandler.text);
        if (response != null && response.success == "false")
        {
            PlayerPrefs.SetInt("LoggedIn", 1);
            logoutButton.SetActive(true);
            LoginsuccessfullyPanel.SetActive(true);
            StartCoroutine(FetchUserNameFromEmail());
        }
        else
        {
            loginPanel.SetActive(false);
            userNamePanel.SetActive(true);
        }
    }

    IEnumerator FetchUserNameFromEmail()
    {
        using var request = CreateRequest("accounts/get/" + PlayerPrefs.GetString("PlayerEmail", ""), "GET");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.LogError(request.error);
            yield break;
        }
        GetUserDetailClass response = JsonUtility.FromJson<GetUserDetailClass>(request.downloadHandler.text);
        if (response != null) PlayerPrefs.SetString("UserName", response.userName);
    }

    public void CreateNewUser()
    {
        if (userNameText.text.Length < 6)
        {
            loginWarningText.text = "username of minimum 6 characters";
        }
        else
        {
            StartCoroutine(CreateNewUserId());
        }
    }

    IEnumerator CreateNewUserId()
    {
        var user = new CreatNewUserClass
        {
            email = PlayerPrefs.GetString("PlayerEmail", ""),
            userName = userNameText.text,
            country = "India"
        };
        using var request = CreateRequest("accounts/create", "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(user)));
        yield return request.SendWebRequest();

        string body = request.downloadHandler.text;
        CreatNewUserResponse response = string.IsNullOrEmpty(body)
            ? null : JsonUtility.FromJson<CreatNewUserResponse>(body);
        if (response != null && response.success == "true")
        {
            PlayerPrefs.SetInt("LoggedIn", 1);
            PlayerPrefs.SetString("UserName", userNameText.text);
            logoutButton.SetActive(true);
            congratulationsPopUp.SetActive(true);
        }
        else
        {
            loginWarningText.text = response?.message ?? request.error ?? "Unable to create user";
        }
    }

    public void DeleteUserId()
    {
        StartCoroutine(DeleteUser());
    }
    IEnumerator DeleteUser()
    {

        DeleteUserClass deleteUserClass = new DeleteUserClass();
        deleteUserClass.email = deleteUserNameText.text;

        using var request = CreateRequest("accounts/create/"+deleteUserClass.email, "DELETE");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log("error " + request.error);
            Debug.Log(request.downloadHandler.text);

        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode);

        }
    }

    public void GetBayesianScore()
    {
        StartCoroutine(FetchBayesianScore());
    }
    IEnumerator FetchBayesianScore()
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
            BayesianAverageScore response = JsonUtility.FromJson<BayesianAverageScore>("{\"bayesianScoresDatas\":" + request.downloadHandler.text +"}");
            if (response?.bayesianScoresDatas == null) yield break;
            var userDatas = response.bayesianScoresDatas;

            for (int i = 0; i < userDatas.Count; i++)
            {
                var rankedScore = Instantiate(rankedScorePrefab, rankedLeaderboardContainer.transform);
                rankedScore.transform.GetChild(0).gameObject.GetComponent<Text>().text = (i + 1).ToString();
                rankedScore.transform.GetChild(1).gameObject.GetComponent<Text>().text = userDatas[i].username;
                rankedScore.transform.GetChild(2).gameObject.GetComponent<Text>().text = ((float)userDatas[i].bayesian_average).ToString("#.##");

            }

        }
    }
}

[Serializable]
public class CreatNewUserClass
{
    public string email;
    public string userName;
    public string country;
}

[Serializable]
public class GetUserDetailClass
{
    public string email;
    public string userName;
    public string country;
    public int averageScore;
}

[System.Serializable]
public class CreatNewUserResponse
{
    public string success;
    public string message;
    public string email;
}

[Serializable]
public class DeleteUserClass
{
    public string email;

}

[Serializable]
public class BayesianAverageScore
{

    public List<BayesianScoresData> bayesianScoresDatas;

}

[System.Serializable]
public class BayesianScoresData
{
    public string username;
    public double bayesian_average;

    public BayesianScoresData(string userName, double bayesian_average)
    {
        this.username= userName;
        this.bayesian_average= bayesian_average;
    }

}

