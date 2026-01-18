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
        private IInputProvider _inputProvider;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _inputProvider = GetComponent<IInputProvider>();

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
    
            float paddleHeight = GetComponent<BoxCollider2D>().bounds.size.y;
            float hitOffset = contact.point.y - transform.position.y;
    
            float normalizedHit = hitOffset / (paddleHeight / 2f);
            normalizedHit = Mathf.Clamp(normalizedHit, -1f, 1f);
    
            float bounceAngle = normalizedHit * maxBounceAngle;
            float angleInRadians = bounceAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    
            direction.x = contact.normal.x > 0 ? Mathf.Abs(direction.x) : -Mathf.Abs(direction.x);
    
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            float ballSpeed = ballRb.linearVelocity.magnitude;
            ballRb.linearVelocity = direction.normalized * ballSpeed;
        }
    }
}