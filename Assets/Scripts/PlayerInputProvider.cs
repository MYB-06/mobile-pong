using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PongGame.Input
{
    public class PlayerInputProvider : MonoBehaviour, IInputProvider
    {
        [SerializeField] private float touchSensivity;
        private PlayerInputActions _inputActions;
        private Vector2 _moveInput;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
        }

        public float GetHorizontalInput()
        {
            return _moveInput.x;
        }
        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            Vector2 rawInput = ctx.ReadValue<Vector2>();
            _moveInput.x = Mathf.Clamp(rawInput.x * touchSensivity, -1f, 1f);
        }
        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
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
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
        }
    }
}

