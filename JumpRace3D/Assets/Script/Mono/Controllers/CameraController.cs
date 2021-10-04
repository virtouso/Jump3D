using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    [Inject] private BaseGameManager _baseGameManager;
    [Inject] private GameStateManager _gameStateManager;

    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _lookOffset;
    [SerializeField] private float _cameraDistance;
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private float _lookSpeed;



    private bool _firstTime = true;

    private void FixedUpdate()
    {
        if (_baseGameManager.PlayerController == null) return;
        if (_gameStateManager.CurrentGameState == GameStates.Failed) return;

        Vector3 goalPosition;
        Vector3 goalLook;
        if (_firstTime)
        {

            goalPosition = _baseGameManager.PlayerController.transform.position - _baseGameManager.PlayerController.transform.forward * _cameraDistance;

            goalLook = _baseGameManager.PlayerController.transform.position + _lookOffset;
            transform.LookAt(goalLook);



            goalPosition = goalPosition + _positionOffset;
            transform.position = goalPosition;


            _firstTime = false;
            return;
        }

        goalPosition = _baseGameManager.PlayerController.transform.position - _baseGameManager.PlayerController.transform.forward * _cameraDistance;

        goalLook = _baseGameManager.PlayerController.transform.position + _lookOffset;
        transform.LookAt(goalLook);


        goalPosition = goalPosition + _positionOffset;
        Vector3 currentValocity = Vector3.zero;
        transform.position = Vector3.Lerp(transform.position, goalPosition, _lerpSpeed * Time.fixedDeltaTime);

    }


}
