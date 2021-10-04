using System.Collections;
using UnityEngine;
using Zenject;

public abstract class BaseCharacterController : MonoBehaviour
{
    public int CurrentPanelIndex;
    public float EndTime=float.MaxValue;

    [Inject] protected GameStateManager _gameStateManager;
    [Inject] protected BaseGameManager _baseGameManager;

    [SerializeField] protected Animator Animator;

    [SerializeField] protected LayerMask WaterLayer;
    [SerializeField] protected LayerMask PanelLayer;
    [SerializeField] protected Rigidbody RigidBody;



    [SerializeField] private float _jumpForceAmount;
    [SerializeField] private float _supportJumpForceAmount;
    [SerializeField] private float _specialJumpforceAmount;
    [SerializeField] protected float ForwardForceAmount;


    [SerializeField] protected Transform _underFoot;
    [SerializeField] private Transform _overHead;

    [SerializeField] private float _hitDistanceCheck;
    [SerializeField] private float _yChangeAmount;
    [SerializeField] private int _bigJumpSize;
    protected Vector3 PanelHitPosition;
    protected bool IsHitOverPanel;
    protected Vector3 PanelHitPoint;

    protected bool IsHitUnderPanel;
    protected bool IsOverPanel;
    protected Transform CurrentPanelTransform;
    protected Transform HittingPanel;
    protected System.Action<JumpingPanel> OnHitPanel;
    protected System.Action<JumpingPanel> OnHitSupporter;
    protected System.Action<JumpingPanel> OnHitSpecial;
    protected System.Action OnHitWater;
    protected System.Action OnBigJump;


    protected bool AlreadyHitWater = false;


    private bool _canJump = true;
    #region Utility


    private void CheckHitOverPanel()
    {
        IsHitOverPanel = UtilitySensors.CheckRayCastHit(_underFoot.position, Vector3.down, PanelLayer, _hitDistanceCheck, out HittingPanel, out PanelHitPoint);


    }

    private void CheckHitUnderPanel()
    {
        IsHitUnderPanel = UtilitySensors.CheckRayCastHit(_overHead.position, Vector3.up, PanelLayer, _hitDistanceCheck, out Transform panel, out Vector3 hitPoint);
    }

    private void CheckOverPanel()
    {
        IsOverPanel = UtilitySensors.CheckRayCastHit(_underFoot.position, Vector3.down, PanelLayer, float.MaxValue, out CurrentPanelTransform, out Vector3 hitPoint);


    }


    private void JumpUpIfHit()
    {
        if (!IsHitOverPanel) return;
        if (!_canJump) return;

        JumpingPanel hittingPanel = HittingPanel.GetComponent<JumpingPanel>();
        int hittingPanelIndex = hittingPanel.PanelIndex;
        if (hittingPanelIndex > CurrentPanelIndex)
        {
            if ((hittingPanelIndex - CurrentPanelIndex)>2)
            {
                OnBigJump.Invoke();
            }

            CurrentPanelIndex = hittingPanelIndex;
          
        }

        OnHitPanel?.Invoke(hittingPanel);
     
        StartCoroutine(EnableJump());
        RigidBody.velocity = new Vector3(RigidBody.velocity.x, 0, RigidBody.velocity.z);
        Animator.CrossFade(CharacterAnimatorKeys.FallToRoll,0.2f);
        hittingPanel.ShowWave();
        if (hittingPanel.IsSupporter)
        {
            RigidBody.AddForce(Vector3.up * _supportJumpForceAmount, ForceMode.Impulse);
            OnHitSupporter?.Invoke(hittingPanel);
        }
        else if (hittingPanel.IsSpecial)
        {
            RigidBody.AddForce(Vector3.up * _specialJumpforceAmount, ForceMode.Impulse);
            OnHitSpecial?.Invoke(hittingPanel);
        }
        else
        {
            RigidBody.AddForce(Vector3.up * _jumpForceAmount, ForceMode.Impulse);
        }

    }

    private void JumpDownIfHit()
    {

        if (!IsHitUnderPanel) return;
        if (RigidBody.velocity.y < 0) return;
      
        RigidBody.velocity = new Vector3(RigidBody.velocity.x, 0, RigidBody.velocity.z);

        RigidBody.AddForce(Vector3.down * _jumpForceAmount, ForceMode.Force);
    }


    private IEnumerator EnableJump()
    {
        _canJump = false;
        yield return new WaitForSeconds(1f);
        _canJump = true;
    }




    private void CheckHitWater()
    {
        if (AlreadyHitWater) return;
        bool hitWater = UtilitySensors.CheckRayCastHit(_underFoot.position, Vector3.down, WaterLayer, _hitDistanceCheck, out HittingPanel, out Vector3 hitPoint);

        if (hitWater)
        {
            AlreadyHitWater = true;
            OnHitWater?.Invoke();
        }

    }


    #endregion




    #region Unity Callbacks

    protected virtual void FixedUpdate()
    {
        CheckHitOverPanel();
        CheckHitUnderPanel();
        CheckOverPanel();
        JumpUpIfHit();
        JumpDownIfHit();
        CheckHitWater();
    }




    #endregion
}
