using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;

    int currentLevel;
    public Vector3[] cameraPosition;
    void Start()
    {

        currentLevel = Mathf.Max(0, PlayerPrefs.GetInt("CurrentLevel", 0));
        width = currentLevel + 2;
        height = width;
        allDots = new GameObject[width, height];
        if (Camera.main != null && cameraPosition != null && currentLevel < cameraPosition.Length)
        {
            Camera.main.transform.position = cameraPosition[currentLevel];
        }
        SetUp();

    }

    private void SetUp()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i,j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity, transform);
                backgroundTile.name = $"({i},{j})";

                GameObject dot = Instantiate(dots[0], tempPosition, Quaternion.identity, transform);
                dot.name = $"({i},{j})";
                allDots[i,j] = dot;

            }
        }

        allDots[0,0].GetComponent<Dot>().FindCompleteBoardScoreFromRotate();
    }
}
