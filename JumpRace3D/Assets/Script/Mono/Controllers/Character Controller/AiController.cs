using UnityEngine;

public class AiController : BaseCharacterController
{
    public string Name;

    private bool _finished;



    private int _goalIndex => CurrentPanelIndex + 1;
    private bool _isDone = false;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _curveHeight;
    [SerializeField] private float _positionRandomness;
    [SerializeField] private float _speedRandomness;


    // physics based Ai Needs Lots of testing to be reliable
    private void PlayGame()
    {
        if (_finished) return;


        Vector3 goalDirection = Vector3.zero;

        goalDirection = _baseGameManager.CurrentLevelHolder.Panels[_goalIndex].transform.position -
                                 transform.position;

        goalDirection = new Vector3(goalDirection.x, 0, goalDirection.z);
        transform.rotation = Quaternion.LookRotation(goalDirection, Vector3.up);


        bool isOverGoalPanel = false;
        if (IsOverPanel)
        {
            JumpingPanel current = CurrentPanelTransform.GetComponent<JumpingPanel>();


            if (current.PanelIndex == _goalIndex)
                isOverGoalPanel = true;
        }

        if (!isOverGoalPanel)
        {
            RigidBody.velocity = new Vector3(transform.forward.x * ForwardForceAmount, RigidBody.velocity.y, transform.forward.z * ForwardForceAmount);
        }
        else
        {
            RigidBody.velocity = new Vector3(0, RigidBody.velocity.y, 0);
        }

    }




    private Vector3 _curveHelper;
    private float _lerper = 0;
    private Vector3 _startCache;
    private Vector3 _endCache;

    private void PlayGameLerpingCurve()
    {

        if (_gameStateManager.CurrentGameState == GameStates.NotStarted) return;
        if (_isDone) return;

        _lerper += _moveSpeed * Time.deltaTime;
        transform.position = UtilityCurve.CalculateBezier3d(_startCache, _curveHelper, _endCache, _lerper);

        if (_lerper >= 0.99f)
        {

            if (_baseGameManager.CurrentLevelHolder.Panels.Count - 1 == _goalIndex)
            {
                _isDone = true;
                EndTime = Time.time;
                CurrentPanelIndex++;

            }
            else
            {
                CurrentPanelIndex++;
                _lerper = 0;
                _curveHelper = SetCurveHelperPosition();
                Animator.CrossFade(CharacterAnimatorKeys.FallToRoll, 0.2f);
            }




        }
    }

    private Vector3 SetCurveHelperPosition()
    {

        _startCache = _baseGameManager.CurrentLevelHolder.Panels[CurrentPanelIndex].Center.position;
        _endCache = _baseGameManager.CurrentLevelHolder.Panels[_goalIndex].Center.position +=
            new Vector3(Random.Range(-_positionRandomness, _positionRandomness), 0, Random.Range(-_positionRandomness, _positionRandomness));


        _moveSpeed = _moveSpeed + Random.Range(-_speedRandomness, _speedRandomness);

        Vector3 lineCenter = (_startCache + _endCache) / 2f;


        return lineCenter + new Vector3(0, lineCenter.y + _curveHeight, 0);


    }




    private void DecideHitPanel(JumpingPanel panel)
    {
        if (panel.PanelIndex == _baseGameManager.CurrentLevelHolder.Panels.Count - 1)
            _isDone = true;

        else CurrentPanelIndex++;




    }


    private void RelocateOnHitWater()
    {
        transform.position = _baseGameManager.CurrentLevelHolder.Panels[_goalIndex].transform.position + new Vector3(0, 5, 0);
        AlreadyHitWater = false;

    }



    private void Finished()
    {
        _finished = true;
    }






    #region Unity Callbacks




    private void Awake()
    {
        OnHitPanel += DecideHitPanel;
        OnHitWater += RelocateOnHitWater;
    }

    private void Start()
    {
        _curveHelper = SetCurveHelperPosition();
    }


    private void Update()
    {

        PlayGameLerpingCurve();

    }



    #endregion



}
