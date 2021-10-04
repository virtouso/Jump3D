using UnityEngine;



[CreateAssetMenu(fileName = "Game Texts", menuName = "Configurations/Game Texts")]
public class GameText : ScriptableObject
{
    public string StartMessage;
    public string FinishMessage;
    public string FailMessage;

    public string LongJumpMessage;

}
