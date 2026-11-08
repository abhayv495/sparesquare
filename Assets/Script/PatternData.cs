using UnityEngine;

public class PatternData : MonoBehaviour
{
    public int patternPosition;
    public string patternShape, patternColor, patternType;

    public void ChangePosition()
    {
        patternPosition = (patternPosition + 1) % 4;
    }
}
