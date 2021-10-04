using UnityEngine;

public class GameStateManager : MonoBehaviour
{


    public GameStates CurrentGameState => _currentGameState;
    public System.Action<GameStates> OnGameStateChanged;



    public void UpdateGameState(GameStates newGameState)
    {
        _currentGameState = newGameState;
        OnGameStateChanged?.Invoke(_currentGameState);
    }


    private GameStates _currentGameState;


}


public enum GameStates
{
    NotStarted,
    Running,
    Failed,
    Finished
}



