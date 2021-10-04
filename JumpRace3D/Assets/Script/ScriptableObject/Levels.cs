using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Levels", menuName = "Configurations/Levels")]
public class Levels : ScriptableObject
{
    public List<LevelInformation> LevelsList;
}



[System.Serializable]
public class LevelInformation
{
    public int LevelIndex;
    public string LevelName;
    public LevelHolder Level;
}



