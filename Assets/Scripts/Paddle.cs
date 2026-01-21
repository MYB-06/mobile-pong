using System;
using PongGame.Input;
using UnityEngine;

namespace PongGame.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))] 
    public class Paddle : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed;
        [Header("Boundaries")]
        [SerializeField] private float minX = -2f;
        [SerializeField] private float maxX = 2f;
        [Header("Bounce Settings")]
        [SerializeField] private float maxBounceAngle;
        private Rigidbody2D _rigidbody2D;
        private BoxCollider2D _boxCollider;
        private IInputProvider _inputProvider;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _inputProvider = GetComponent<IInputProvider>();
            _boxCollider = GetComponent<BoxCollider2D>();

            if(_inputProvider == null)
            {
                Debug.LogError($"[Paddle] IInputProvider is not found: {gameObject.name}");
            }
        }
        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            float input = _inputProvider.GetHorizontalInput();

            _rigidbody2D.linearVelocity = new Vector2(input * speed, 0f);

            ClampPosition();
        }
        private void ClampPosition()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<Ball>(out Ball ball))
            {
                HandleBallBounce(collision, ball);
            }
        }

        private void HandleBallBounce(Collision2D collision, Ball ball)
        {
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 contactPoint = contact.point;

            float paddleWidth = _boxCollider.size.x;
            float hitOffset = contactPoint.x - transform.position.x;
            float normalizedHit = hitOffset / (paddleWidth * 0.5f);
            normalizedHit = Mathf.Clamp(normalizedHit, -1f, 1f);

            float bounceAngle = normalizedHit * maxBounceAngle * Mathf.Deg2Rad;
            float ballDirection = transform.position.y > 0 ? -1f : 1f;

            Vector2 newDirection = new Vector2(Mathf.Sin(bounceAngle), ballDirection * Mathf.Cos(bounceAngle)).normalized;

            float paddleMomentum = _rigidbody2D.linearVelocity.x * 1f;
            Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D>();
            float currentSpeed = ballRigidbody.linearVelocity.magnitude;

            Vector2 finalVelocity = newDirection * currentSpeed;
            finalVelocity.x += paddleMomentum;

            ballRigidbody.linearVelocity = finalVelocity.normalized * currentSpeed;
        }
    }
}