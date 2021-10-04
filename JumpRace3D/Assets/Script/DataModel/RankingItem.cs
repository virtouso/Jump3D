using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingItem 
{
    public string Name;
    public float FinishTime;
    public float CurrentIndex;

    public RankingItem(string name, float finishTime, float currentIndex)
    {
        Name = name;
        FinishTime = finishTime;
        CurrentIndex = currentIndex;
    }
}
