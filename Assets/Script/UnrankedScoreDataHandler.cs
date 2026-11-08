using System;
using System.Collections.Generic;

[Serializable]
public class UnrankedScoreDataHandler
{
    //LeaderboardData
    public List<ScoresData> unrankedScores;

   public UnrankedScoreDataHandler()
    {
       unrankedScores = new List<ScoresData>();
    }
}
