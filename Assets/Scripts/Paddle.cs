using DG.Tweening;
using PongGame.Audio;
using PongGame.Core;
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
        [SerializeField] private float safetyOffset;
        [Header("Bounce Settings")]
        [SerializeField] private float maxBounceAngle;
        [SerializeField] private float momentumTransfer = 1f;
        [Header("Animation Settings")]
        [SerializeField] private float paddleScaleX;
        [SerializeField] private float paddleScaleY;
        [SerializeField] private float animDuration;
        private float _paddleHalfWidth;
        private float _minX;
        private float _maxX;
        private float _effectiveMinX;
        private float _effectiveMaxX;
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
        void Start()
        {
            if(GameBoundaries.Instance != null)
            {
                _minX = GameBoundaries.Instance.MinX;
                _maxX = GameBoundaries.Instance.MaxX;

                _paddleHalfWidth = _boxCollider.size.x / 2f;
            }
            else
            {
                Debug.LogError("[Paddle] GameBoundaries not found!");
            }
            _effectiveMinX = _minX + _paddleHalfWidth + safetyOffset;
            _effectiveMaxX = _maxX - _paddleHalfWidth - safetyOffset;
        }
        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            float input = _inputProvider.GetHorizontalInput();
            
            if((transform.position.x <= _effectiveMinX && input < 0) || (transform.position.x >= _effectiveMaxX && input > 0))
            {
                input = 0;
            }

            _rigidbody2D.linearVelocity = new Vector2(input * speed, 0f);           
        }  
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<Ball>(out Ball ball))
            {
                HandleBallBounce(collision, ball);

                Sequence seq = DOTween.Sequence();
                seq.Append(transform.DOScaleX(paddleScaleX, animDuration));
                seq.Join(transform.DOScaleY(paddleScaleY, animDuration));   
                seq.Append(transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutElastic)).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
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

            float paddleMomentum = _rigidbody2D.linearVelocity.x * momentumTransfer;
            float currentSpeed = ball.Rigidbody.linearVelocity.magnitude;

            Vector2 finalVelocity = newDirection * currentSpeed;
            finalVelocity.x += paddleMomentum;

            ball.Rigidbody.linearVelocity = finalVelocity.normalized * currentSpeed;

            AudioManager.Instance.PlayPaddleHit();
        }
    }
}