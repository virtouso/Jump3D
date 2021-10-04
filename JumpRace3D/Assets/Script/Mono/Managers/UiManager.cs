using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UiManager : MonoBehaviour
{

    public bool FinalPanelShown = false;
    [SerializeField] public GameText GameTexts;
    [SerializeField] public ProgressSlider ProgressSlider;



    [Inject] private GameStateManager _gameStateManager;
    [Inject] private BaseGameManager _baseGameManager;



    [SerializeField] private Text _startText;
    [SerializeField] private Text _endingText;
    [SerializeField] private Text _hintText;
    [SerializeField] private Ranking _ranking;
    [SerializeField] private FinalPanel _finalPanel;
    [SerializeField] private float _hintMessageTime;

    
    [SerializeField] private float ShowPanelWaitTime;
    [SerializeField] private float _timeAfterShowingFinalPanel;
    public void InitUiForGameStart()
    {
        ShowStartText(GameTexts.StartMessage, true);
        ShowEndMessage("", false);
        _hintText.gameObject.SetActive(false);
    }


    public void ShowStartText(string message, bool show)
    {
        _startText.text = message;
        _startText.gameObject.SetActive(show);
    }

    public void ShowEndMessage(string message, bool show)
    {
        _endingText.text = message;
        _endingText.gameObject.SetActive(show);
    }

    public void ShowHint(string message)
    {
        _hintText.text = message;
        StartCoroutine(HideHint());
    }

    public void UpdateRanking(string first, string second, string third)
    {
        _ranking.FirstText.text = first;
        _ranking.SecondText.text = second;
        _ranking.ThirdText.text = third;
    }

    private IEnumerator HideHint()
    {
        _hintText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_hintMessageTime);
        _hintText.gameObject.SetActive(false);
    }


    private void ShowFail(List<string> rankings)
    {
        ShowEndMessage(GameTexts.FailMessage, true);
        StartCoroutine(ShowDelayedFinalPanel(rankings));
    }


    private void ShowFinish(List<string> rankings)
    {
        ShowEndMessage(GameTexts.FinishMessage, true);
        StartCoroutine(ShowDelayedFinalPanel(rankings));
    }

    private IEnumerator ShowDelayedFinalPanel(List<string> rankings)
    {

        yield return new WaitForSeconds(ShowPanelWaitTime);
        _finalPanel.Show(rankings[0], rankings[1], rankings[2], rankings[3]);
        yield return new WaitForSeconds(_timeAfterShowingFinalPanel);
        FinalPanelShown = true;

    }


    private void CheckState(GameStates state)
    {
        switch (state)
        {
            case GameStates.NotStarted:
                break;
            case GameStates.Running:
                ShowStartText("", false);
                break;
            case GameStates.Failed:

                List<string> ranking = _baseGameManager.CalculateRanking();
                ShowFail(ranking);
                break;
            case GameStates.Finished:
                List<string> rankings = _baseGameManager.CalculateRanking();
                ShowFinish(rankings);
                break;

        }
    }



    #region UnityCallbacks

    private void Start()
    {
        _gameStateManager.OnGameStateChanged += CheckState;
    }

    

    #endregion


}

[System.Serializable]
public class Ranking
{
    [SerializeField] public Text FirstText;
    [SerializeField] public Text SecondText;
    [SerializeField] public Text ThirdText;
}

[System.Serializable]
public class ProgressSlider
{
    public Text StartText;
    public Text EndText;
    public Slider Slider;


    public void Init(string start, string end, int sliderMax)
    {
        StartText.text = start;
        EndText.text = end;
        Slider.value = 0;
        Slider.maxValue = (float)sliderMax;
    }

    public void UpdateProgressBar(int value)
    {
        Slider.value = value;
    }


}


