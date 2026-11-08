using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardHandler : MonoBehaviour
{
    public GameObject unrankedScorePrefab;
    public  GameManagerHandler gameManager;

    void Start()
    {
        foreach (ScoresData entry in gameManager.GetHighScores())
        {
            var unrankedScore = Instantiate(unrankedScorePrefab,transform);
            unrankedScore.transform.GetChild(0).gameObject.GetComponent<Text>().text = entry.levelname;
            unrankedScore.transform.GetChild(1).gameObject.GetComponent<Text>().text = entry.scoreValue.ToString() ;
            unrankedScore.transform.GetChild(2).gameObject.GetComponent<Text>().text = entry.scoreDate;

        }
    }

}
