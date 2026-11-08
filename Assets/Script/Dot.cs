using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dot : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    private Board board;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("Pattern Details")]
    public GameObject[] patternsObject;
    public Sprite[] patterns;

    public string[] patternShape;
    public string[] patternColor;
    public Color32[] patternColors;
    public string[] patternType;
    public Sprite[] patternsShapeSprite;
    public List<int> positionList = new List<int>();
    public List<int> colorListValue = new List<int>();
    public List<int> shapeListValue = new List<int>();

    public bool isMatched = false;

    public GameObject[] patternObjectAfterRotation;

    bool isDragging;
    public static GameObject objectBeingDragged;
    public Vector3 startPosition;
    public float distanceToCamera;
    public int tempColumn;
    public int tempRow;
    public int tempColumnCurrent;
    public int tempRowCurrent;
    public int dragResult = 0;
    public bool rotationDrag = false;

    void Start()
    {
        board = GetComponentInParent<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        tempColumnCurrent = column;
        tempRowCurrent = row;
        AssignSprites();

        rotationDrag = false;
        isDragging = false;

    }

    void Update()
    {
        if (isDragging == false)
        {
            Vector3 targetPosition = new Vector3(column, row, transform.position.z);
            if ((transform.position - targetPosition).sqrMagnitude > 0.01f)
            {
                float smoothing = 1f - Mathf.Exp(-12f * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
            }
            else if (transform.position != targetPosition)
            {
                transform.position = targetPosition;
            }
        }

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        objectBeingDragged = gameObject;
        startPosition = transform.position;
        distanceToCamera = Mathf.Abs(startPosition.z - Camera.main.transform.position.z) - 0.01f;
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int targetColumn = Mathf.RoundToInt(transform.position.x);
        int targetRow = Mathf.RoundToInt(transform.position.y);
        if (targetColumn >= 0 && targetColumn < board.width &&
            targetRow >= 0 && targetRow < board.height &&
            (targetColumn != column || targetRow != row))
        {
            GameObject target = board.allDots[targetColumn, targetRow];
            if (target != null && (target.transform.position - transform.position).sqrMagnitude < 0.04f)
            {
                Dot targetDot = target.GetComponent<Dot>();
                int oldColumn = column;
                int oldRow = row;
                column = targetDot.column;
                row = targetDot.row;
                targetDot.column = oldColumn;
                targetDot.row = oldRow;
                board.allDots[column, row] = gameObject;
                board.allDots[oldColumn, oldRow] = target;
            }
        }

        isDragging = false;
        StartCoroutine(FindCompleteBoardMatches());
    }

    public void OnDrag(PointerEventData eventData)
    {
        rotationDrag = false;
        Vector2 pointer = eventData.position;
        objectBeingDragged.transform.position = Camera.main.ScreenToWorldPoint(
            new Vector3(pointer.x, pointer.y, distanceToCamera));
    }

    private void OnMouseDown()
    {

        rotationDrag = true;

    }

    private void OnMouseUp()
    {

        if (rotationDrag == true)
        {
            GameManagerHandler.instance.CurrentSetAnchorPress(gameObject);
        }

    }

    public void AssignSprites()
    {
        for (int i = 0; i < patternsObject.Length; i++)
        {
            GameObject pattern = patternsObject[i];
            PatternData data = pattern.GetComponent<PatternData>();
            SpriteRenderer renderer = pattern.GetComponent<SpriteRenderer>();
            Transform shape = pattern.transform.GetChild(0);

            int positionIndex = Random.Range(0, positionList.Count);
            int position = positionList[positionIndex];
            renderer.sprite = patterns[position];
            data.patternShape = patternShape[position];
            shape.GetComponent<SpriteMask>().sprite = patterns[position];
            positionList.RemoveAt(positionIndex);

            int colorIndex = Random.Range(0, colorListValue.Count);
            int color = colorListValue[colorIndex];
            data.patternColor = patternColor[color];
            renderer.color = patternColors[color];
            colorListValue.RemoveAt(colorIndex);

            int shapeIndex = Random.Range(0, shapeListValue.Count);
            int shapeValue = shapeListValue[shapeIndex];
            data.patternType = patternType[shapeValue];
            shape.GetComponent<SpriteRenderer>().sprite = patternsShapeSprite[shapeValue];
            shapeListValue.RemoveAt(shapeIndex);
        }
    }

    public void FindCompleteBoardScoreFromRotate()
    {
        for (int i = 0; i < 4; i++)
        {
            int j = patternsObject[i].GetComponent<PatternData>().patternPosition;
            patternObjectAfterRotation[j] = patternsObject[i];
        }
        StartCoroutine(FindCompleteBoardMatches());
    }
    IEnumerator FindCompleteBoardMatches()
    {
        yield return new WaitForSeconds(.2f);
        int score = 0;
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (j < board.height - 1)
                {
                    score += MatchingAttributes(i, j, 0, i, j + 1, 2);
                    if (i == 0) score += MatchingAttributes(i, j, 3, i, j + 1, 3);
                    if (i == board.width - 1) score += MatchingAttributes(i, j, 1, i, j + 1, 1);
                }
                if (i < board.width - 1)
                {
                    score += MatchingAttributes(i, j, 1, i + 1, j, 3);
                    if (j == 0) score += MatchingAttributes(i, j, 2, i + 1, j, 2);
                    if (j == board.height - 1) score += MatchingAttributes(i, j, 0, i + 1, j, 0);
                }
            }
        }
        GameManagerHandler manager = GameManagerHandler.instance;
        manager.CountScore(score - manager.score);
    }

    private int MatchingAttributes(int columnA, int rowA, int sideA,
        int columnB, int rowB, int sideB)
    {
        PatternData first = board.allDots[columnA, rowA].GetComponent<Dot>()
            .patternObjectAfterRotation[sideA].GetComponent<PatternData>();
        PatternData second = board.allDots[columnB, rowB].GetComponent<Dot>()
            .patternObjectAfterRotation[sideB].GetComponent<PatternData>();
        int points = 0;
        if (first.patternShape == second.patternShape) points++;
        if (first.patternColor == second.patternColor) points++;
        if (first.patternType == second.patternType) points++;
        return points;
    }

    private void CountMatchingAttributes(int columnA, int rowA, int sideA,
        int columnB, int rowB, int sideB)
    {
        GameManagerHandler.instance.CountScore(
            MatchingAttributes(columnA, rowA, sideA, columnB, rowB, sideB));
    }

    public void CalculateScoreLeft(int i, int j) => CountMatchingAttributes(i, j, 3, i - 1, j, 1);
    public void CalculateScoreRight(int i, int j) => CountMatchingAttributes(i, j, 1, i + 1, j, 3);
    public void CalculateScoreUp(int i, int j) => CountMatchingAttributes(i, j, 0, i, j + 1, 2);
    public void CalculateScoreDown(int i, int j) => CountMatchingAttributes(i, j, 2, i, j - 1, 0);
    public void CalculateScoreSideLeftUpper(int i, int j) => CountMatchingAttributes(i, j, 0, i - 1, j, 0);
    public void CalculateScoreSideLeftDown(int i, int j) => CountMatchingAttributes(i, j, 2, i - 1, j, 2);
    public void CalculateScoreSideRightUpper(int i, int j) => CountMatchingAttributes(i, j, 0, i + 1, j, 0);
    public void CalculateScoreSideRightDown(int i, int j) => CountMatchingAttributes(i, j, 2, i + 1, j, 2);
    public void CalculateScoreSideUpLeft(int i, int j) => CountMatchingAttributes(i, j, 3, i, j + 1, 3);
    public void CalculateScoreSideUpRight(int i, int j) => CountMatchingAttributes(i, j, 1, i, j + 1, 1);
}
