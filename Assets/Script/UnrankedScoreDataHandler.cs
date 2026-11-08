using System;
using System.Collections.Generic;

[Serializable]
public class UnrankedScoreDataHandler
{
    //LeaderBoardData
    public List<ScoresData> unrankedScores;

   public UnrankedScoreDataHandler()
    {
       unrankedScores = new List<ScoresData>();
    }
}
