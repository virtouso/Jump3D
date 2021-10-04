using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerController : BaseCharacterController
{

    [Inject] private UiManager _uiManager;

    [SerializeField] private float _dragMultiplier;
    [SerializeField] private float _ignorableChangeAmount;
    [SerializeField] private GameObject _fireEffect;
    [SerializeField] private float _fireEffectTime;
    [SerializeField] private GameObject _waterSplashPrefab;


    [SerializeField] private Transform _underFootDetector;

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private CurvedLineRenderer _curveLineRederer;
    [SerializeField] private Color _detectedColor;
    [SerializeField] private Color _undetectedColor;
    [SerializeField] private Color _unvisibleColor;
    [SerializeField] private Transform _ragDollParent;
    private GameObject _waterSplashObject;


    private float _lastXMousePosition;




    #region Utility

    private void CheckState(GameStates gameState)
    {
        switch (gameState)
        {
            case GameStates.NotStarted:
                break;
            case GameStates.Running:
                StartPlaying();
                break;
            case GameStates.Failed:
                break;
            case GameStates.Finished:
                break;

        }
    }


    private void StartPlaying()
    {
        RigidBody.isKinematic = false;

        _lastXMousePosition = Input.mousePosition.x;

    }




    private void StartGame()
    {
        if (_gameStateManager.CurrentGameState != GameStates.NotStarted)
            return;

        if (!Input.GetMouseButtonDown(0)) return;
        _gameStateManager.UpdateGameState(GameStates.Running);

    }

    private void PlayGame()
    {
        if (_gameStateManager.CurrentGameState != GameStates.Running)
            return;

        if (Input.GetMouseButtonDown(0))
            _lastXMousePosition = Input.mousePosition.x;


        if (Input.GetMouseButton(0))
        {
            RigidBody.velocity = new Vector3(transform.forward.x * ForwardForceAmount, RigidBody.velocity.y, transform.forward.z * ForwardForceAmount);

            float changeAmount = Input.mousePosition.x - _lastXMousePosition;
            if (Mathf.Abs(changeAmount) < _ignorableChangeAmount) return;

            transform.Rotate(Vector3.up, changeAmount * _dragMultiplier);

            _lastXMousePosition = Input.mousePosition.x;

        }
        else
        {
            RigidBody.velocity = new Vector3(0, RigidBody.velocity.y, 0);
        }

    }

    private void CheckHitWater()
    {
        _gameStateManager.UpdateGameState(GameStates.Failed);
        ShowWaterSplash(_underFoot.transform.position);
    }


    private bool _alreadyFinishedGame = false;

    private void CheckFinishGame(JumpingPanel panel)
    {
        if (_alreadyFinishedGame) return;
        if (panel.PanelIndex == _baseGameManager.CurrentLevelHolder.Panels.Count - 1)
        {
            RigidBody.isKinematic = true;
            _alreadyFinishedGame = true;
            EndTime = Time.time;
            _gameStateManager.UpdateGameState(GameStates.Finished);
            panel.GetComponent<EndingPanel>().ShowFinalFlares();
        }
    }

    private void HitSpecial(JumpingPanel panel)
    {
        panel.ShowFireUpEffect();
        panel.Destroy();
        StartCoroutine(FireUp());
    }

    private IEnumerator FireUp()
    {
        _fireEffect.SetActive(true);
        yield return new WaitForSeconds(_fireEffectTime);
        _fireEffect.SetActive(false);
    }


    private void HitSupporter(JumpingPanel panel)
    {
        panel.Disappear();
    }

    private void UpdateSlider(JumpingPanel panel)
    {
        _uiManager.ProgressSlider.UpdateProgressBar(panel.PanelIndex);
    }



    private void InitWaterSplash()
    {
        _waterSplashObject = Instantiate(_waterSplashPrefab);
        _waterSplashObject.SetActive(false);
    }
    private void ShowWaterSplash(Vector3 position)
    {
        _waterSplashObject.transform.position = position;
        _waterSplashObject.SetActive(true);
    }

    private void ShowBigJumpText()
    {
        _uiManager.ShowHint(_uiManager.GameTexts.LongJumpMessage);
    }



    private void SetUnderFootPosition()
    {
        if (IsOverPanel)
        {
            _underFootDetector.transform.position = new Vector3(transform.position.x, CurrentPanelTransform.transform.position.y + 0.1f, transform.position.z);
            _lineRenderer.sharedMaterial.SetColor("_Color", _detectedColor);
        }
        else
        {
            _underFootDetector.transform.position = transform.position + Vector3.down * 100;

            _lineRenderer.sharedMaterial.SetColor("_Color", _undetectedColor);

        }

    }

    private void SetRagDoll(bool enable)
    {
        Rigidbody[] bodies = _ragDollParent.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = !enable;
        }
        Animator.enabled = !enable;
    }

    #endregion




    #region UnityCallbacks


    private void Start()
    {
        _gameStateManager.OnGameStateChanged += CheckState;
        OnHitWater += CheckHitWater;
        OnHitPanel += CheckFinishGame;
        OnHitPanel += UpdateSlider;
        OnHitSupporter += HitSupporter;
        OnHitSpecial += HitSpecial;
        InitWaterSplash();
        OnBigJump += ShowBigJumpText;
        SetRagDoll(false);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        StartGame();
        PlayGame();
        SetUnderFootPosition();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit obstacle");
        _gameStateManager.UpdateGameState(GameStates.Failed);
        _curveLineRederer.enabled = false;
        _lineRenderer.enabled = false;
        RigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        SetRagDoll(true);
    }

    #endregion



}
