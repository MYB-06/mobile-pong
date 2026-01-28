using UnityEngine;
using UnityEngine.InputSystem;

namespace PongGame.Input
{
    public class PlayerInputProvider : MonoBehaviour, IInputProvider
    {
        private PlayerInputActions _inputActions;
        private Camera _mainCamera;
        private float _lastValidWorldX;
        private bool _isTouching;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _mainCamera = Camera.main;

            _inputActions.Player.Move.performed += OnInputPerformed;
            _inputActions.Player.Move.canceled += OnInputCanceled;
        }
        void Start()
        {
            _lastValidWorldX = transform.position.x;
        }

        public InputType GetInputType()
        {
            return _isTouching ? InputType.Touch : InputType.Keyboard;
        }
        public float GetHorizontalInput()
        {
            if (!_isTouching)
            {
                return _inputActions.Player.Move.ReadValue<Vector2>().x;
            }
            return 0f;
        }
        public float GetTargetXPosition()
        {
           return _lastValidWorldX;
        }
        private void OnInputPerformed(InputAction.CallbackContext ctx)
        {
            Vector2 inputValue = ctx.ReadValue<Vector2>();

            if(inputValue.magnitude > 10f)
            {
                _isTouching = true;

                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(inputValue.x, inputValue.y, _mainCamera.nearClipPlane));

                _lastValidWorldX = worldPos.x;
            }
            else _isTouching = false;
        }
        private void OnInputCanceled(InputAction.CallbackContext ctx)
        {
            _isTouching = false;
        }
        private void OnEnable()
        {
            _inputActions?.Enable();
        }
        private void OnDisable()
        {
            _inputActions?.Disable();
        }
        private void OnDestroy()
        {
            _inputActions.Player.Move.performed -= OnInputPerformed;
            _inputActions.Player.Move.canceled -= OnInputCanceled;
        }
    }
}

