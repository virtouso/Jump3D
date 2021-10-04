using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
public class BaseGameManager : MonoBehaviour
{

    [Inject] private GameStateManager _gameStateManager;
    [Inject] private UiManager _uiManager;


    [SerializeField] private Levels _levels;// for memory management it should turn to addressable


    [SerializeField] private PlayerController _playerControllerPrefab;
    [SerializeField] private AiController _AiControllerPrefab;



    [SerializeField] private float CharacterPlacementYOffset;
    [SerializeField] private int OpponentsCount;
    [SerializeField] private float _rankingInterval;


    private PlayerController _playerController;

    public PlayerController PlayerController => _playerController;



    private List<AiController> _opponents;
    public List<PlayerController> Opponents => Opponents;


    private LevelInformation _currentLevel;
    public LevelInformation CurrentLevel => _currentLevel;
    [HideInInspector] public LevelHolder CurrentLevelHolder;





    #region utility


    public List<string> CalculateRanking()
    {
        List<string> result = new List<string>(OpponentsCount + 1);
        List<RankingItem> ranks = new List<RankingItem>(OpponentsCount + 1);
        foreach (AiController item in _opponents)
        {
            ranks.Add(new RankingItem(item.Name, item.EndTime, item.CurrentPanelIndex));
        }
        ranks.Add(new RankingItem(OpponentNames.PlayerName, _playerController.EndTime, _playerController.CurrentPanelIndex));

     
        List<RankingItem> finalRanking = ranks.OrderByDescending(x => x.CurrentIndex).ThenBy(x => x.FinishTime).ToList();

        for (int i = 0; i < finalRanking.Count; i++)
        {
            result.Add(finalRanking[i].Name);
        }
        return result;
    }




    private IEnumerator ShowLevelRanking()
    {
        while (true)
        {

            yield return new WaitForSeconds(_rankingInterval);

            if (_gameStateManager.CurrentGameState == GameStates.Running)
            {
                List<string> highRank = CalculateRanking();
                _uiManager.UpdateRanking(highRank[0], highRank[1], highRank[2]);
            }
        }

    }


    public void InitSlider()
    {
        int currentLevel = _currentLevel.LevelIndex;
        int nextLevel = currentLevel + 1;

        _uiManager.ProgressSlider.Init((currentLevel + 1).ToString(), (nextLevel + 1).ToString(), _currentLevel.Level.Panels.Count);
    }

    private void InitLevel()
    {
        int currentLevel = 0;

        if (!UtilityPlayerPrefs.CheckKeyExist(PlayerPrefsKeys.CurrentLevel))
        {
            UtilityPlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, currentLevel);
        }
        else
        {
            currentLevel = UtilityPlayerPrefs.GetInt(PlayerPrefsKeys.CurrentLevel);
        }

        _currentLevel = _levels.LevelsList[currentLevel];
        CurrentLevelHolder = Instantiate(_levels.LevelsList[currentLevel].Level);

    }

    private void SetGameState()
    {
        _gameStateManager.UpdateGameState(GameStates.NotStarted);
    }

    private void InitPlayerAndOpponents()
    {
        //init player
        _playerController = Instantiate(_playerControllerPrefab, CurrentLevelHolder.Panels[0].transform.position + new Vector3(0, CharacterPlacementYOffset, 0), CurrentLevelHolder.Panels[0].transform.rotation);



        //init opponents
        _opponents = new List<AiController>(OpponentsCount);
        for (int i = 0; i < OpponentsCount; i++)
        {
            AiController ai = Instantiate(_AiControllerPrefab, CurrentLevelHolder.Panels[i + 1].transform.position + new Vector3(0, CharacterPlacementYOffset, 0), CurrentLevelHolder.Panels[i + 1].transform.rotation);
            ai.CurrentPanelIndex = i + 1;
            ai.Name = OpponentNames.OpponentsNames[i];
            _opponents.Add(ai);
        }

    }


    private void InitUi()
    {
        _uiManager.InitUiForGameStart();
    }

    private void CheckForRestartOrNextLevel()
    {

        if (!_uiManager.FinalPanelShown) return;
        if (_gameStateManager.CurrentGameState == GameStates.Failed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(SceneNames.MainGamePlayScene);
            }
        }

        if (_gameStateManager.CurrentGameState == GameStates.Finished)
        {
            if (Input.GetMouseButton(0))
            {
                if (_currentLevel.LevelIndex == _levels.LevelsList.Count - 1)
                {
                    Debug.LogError("there is no more levels");
                    return;
                }

                UtilityPlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, _currentLevel.LevelIndex + 1);
                SceneManager.LoadScene(SceneNames.MainGamePlayScene);
            }
        }


    }



    #endregion





    #region Unity Callbacks


    private void Start()
    {

        InitLevel();
        SetGameState();
        InitPlayerAndOpponents();
        InitUi();
        StartCoroutine(ShowLevelRanking());
        InitSlider();
    }

    private void Update()
    {
        CheckForRestartOrNextLevel();
    }


    #endregion





}
