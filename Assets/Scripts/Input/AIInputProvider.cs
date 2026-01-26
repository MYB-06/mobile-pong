using PongGame.Core;
using PongGame.Gameplay;
using UnityEngine;

namespace PongGame.Input
{
    public class AIInputProvider : MonoBehaviour, IInputProvider
    {
        [Header("AI Settings")]
        [SerializeField] private Transform ballTransform;
        [SerializeField] private float reactionSpeed;
        [SerializeField] private float reactionDelay;
        [SerializeField] private float smoothSpeed;
        [SerializeField] private bool usePrediction = true;
        [SerializeField] private bool useDifficultySettings = true;
        [Header("Movement Settings")]
        [SerializeField] private float safetyOffset;

        private Rigidbody2D _ballRigidbody;

        private float _effectiveLeftWall;
        private float _effectiveRightWall;
        private float _leftWall;
        private float _rightWall;

        private float _targetX;
        private float _currentInput;
        private Vector2 _delayedBallPos;
        private Vector2 _delayedBallVel;
        private float _delayTimer;

        private void Awake()
        {
            if(ballTransform != null) _ballRigidbody = ballTransform.GetComponent<Rigidbody2D>();     
        }
        private void Start()
        {
            InitializeDifficultySettings();
            InitializeBoundaries();
            InitializeBallTracking();
        }
        private void Update()
        {
            if (ballTransform == null) return;

            UpdateDelayedBallData();
            CalculateTargetPosition();
            SmoothInput();          
        }
        private void InitializeDifficultySettings()
        {
            if (!useDifficultySettings) return;

            var settings = DifficultySettings.GetAISettings(DifficultySettings.LoadDifficulty());
            reactionDelay = settings.reactionDelay;
            reactionSpeed = settings.reactionSpeed;
            smoothSpeed = settings.smoothSpeed;
        }

        private void InitializeBallTracking()
        {
            if (ballTransform == null || _ballRigidbody == null) return;
    
            _delayedBallPos = ballTransform.position;
            _delayedBallVel = _ballRigidbody.linearVelocity;
        }
        private void InitializeBoundaries()
        {
            if (GameBoundaries.Instance == null)
            {
                SetFallbackBoundaries();
                return;
            }

            _leftWall = GameBoundaries.Instance.MinX;
            _rightWall = GameBoundaries.Instance.MaxX;

            CalculateEffectiveBoundaries();
        }
        private void CalculateEffectiveBoundaries()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
    
            if (collider == null)
            {
                _effectiveLeftWall = _leftWall;
                _effectiveRightWall = _rightWall;
                return;
            }

            float paddleHalfWidth = collider.size.x / 2f;
            _effectiveLeftWall = _leftWall + paddleHalfWidth + safetyOffset;
            _effectiveRightWall = _rightWall - paddleHalfWidth - safetyOffset;
        }

        private void SetFallbackBoundaries()
        {
            _leftWall = -3f;
            _rightWall = 3f;
            _effectiveLeftWall = -2.5f;
            _effectiveRightWall = 2.5f;
        }
        public float GetHorizontalInput()
        {
            return _currentInput;
        }
        private void SmoothInput()
        {
            float paddleX = transform.position.x;
            float difference = _targetX - paddleX;
            float targetInput;

            if (Mathf.Abs(difference) < 0.1f)
            {
                targetInput = 0f;
            }
            else if (difference > 0)
            {
                targetInput = reactionSpeed;
            }
            else
            {
                targetInput = -reactionSpeed;
            }

            _currentInput = Mathf.Lerp(_currentInput, targetInput, Time.deltaTime * smoothSpeed);
        }
        private void CalculateTargetPosition()
        {
            if (ballTransform == null) return;

            Vector2 ballPos = _delayedBallPos;
            Vector2 ballVelocity = _delayedBallVel;

            float predictedX;

            if(usePrediction && Mathf.Abs(ballVelocity.y) > 0.01f)
            {
                bool ballComing = 
                    (ballVelocity.y < 0 && ballPos.y > transform.position.y) || 
                        (ballVelocity.y > 0 && ballPos.y < transform.position.y);

                if (ballComing)
                {
                    predictedX = PredictBallPositionWithBounce(ballPos, ballVelocity);
                }
                else
                {
                    predictedX = 0f;
                }
            }
            else
            {
                predictedX = ballPos.x;
            }
      
            _targetX = Mathf.Clamp(predictedX, _effectiveLeftWall, _effectiveRightWall);
        }
        private float PredictBallPositionWithBounce(Vector2 ballPos, Vector2 ballVel)
        {
            float targetY = transform.position.y;

            Vector2 currentPos = ballPos;
            Vector2 currentVel = ballVel;

            for (int i = 0; i < 3; i++)
            {
                float timeToReachY = Mathf.Abs((targetY - currentPos.y) / currentVel.y);
                float predictedX = currentPos.x + (currentVel.x * timeToReachY);

                if(predictedX >= _leftWall && predictedX <= _rightWall)
                {
                    return predictedX;
                }

                float wallX = predictedX > _rightWall ? _rightWall : _leftWall;
                float timeToWall = Mathf.Abs((wallX - currentPos.x) / currentVel.x);

                Vector2 hitPoint = new Vector2(wallX, currentPos.y + (currentVel.y * timeToWall));

                currentVel.x = -currentVel.x;
                
                currentPos = hitPoint;
            }
            return ballPos.x;
        }
        private void UpdateDelayedBallData()
        {
            _delayTimer += Time.deltaTime;

            if(_delayTimer >= reactionDelay)
            {
                _delayedBallPos = ballTransform.position;
                _delayedBallVel = _ballRigidbody.linearVelocity;

                _delayTimer = 0f;
            }
        }
    }
}

