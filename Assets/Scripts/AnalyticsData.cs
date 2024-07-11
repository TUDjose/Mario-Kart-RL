using System;

[Serializable]
public struct AnalyticsData
{
    public string AgentID;
    public bool FinishedLap;
    public int EpisodeLength;
    public float Reward;
    public int CurrLesson;

    public string ToCSV()
    {
        return AgentID + ";" +
               FinishedLap + ";" +
               EpisodeLength + ";" +
               Reward.ToString("G") + ";" +
               CurrLesson +  
               "\n";
    }
}
