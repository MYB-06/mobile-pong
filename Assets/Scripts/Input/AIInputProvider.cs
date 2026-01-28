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
        [SerializeField] private float smoothSpeed;
        [SerializeField] private float predictionAccuracy;
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

        private void Awake()
        {
            if(ballTransform != null) _ballRigidbody = ballTransform.GetComponent<Rigidbody2D>();     
        }
        private void Start()
        {
            InitializeDifficultySettings();
            InitializeBoundaries();
        }
        private void Update()
        {
            if (ballTransform == null) return;

            CalculateTargetPosition();
            SmoothInput();          
        }
        private void InitializeDifficultySettings()
        {
            if (!useDifficultySettings) return;

            var settings = DifficultySettings.GetAISettings(DifficultySettings.LoadDifficulty());
            reactionSpeed = settings.reactionSpeed;
            smoothSpeed = settings.smoothSpeed;
            predictionAccuracy = settings.predictionAccuracy;
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
        public InputType GetInputType()
        {
            return InputType.AI;
        }
        public float GetHorizontalInput()
        {
            return _currentInput;
        }
        public float GetTargetXPosition()
        {
            return transform.position.x;
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

            Vector2 ballPos = ballTransform.position;
            Vector2 ballVelocity = _ballRigidbody.linearVelocity;

            float predictedX;

            if(usePrediction && Mathf.Abs(ballVelocity.y) > 0.01f)
            {
                bool ballComing = 
                    (ballVelocity.y < 0 && ballPos.y > transform.position.y) || 
                        (ballVelocity.y > 0 && ballPos.y < transform.position.y);

                if (ballComing)
                {
                    float perfectPrediction = PredictBallPositionWithBounce(ballPos, ballVelocity);

                    float inaccuratePrediction = ballPos.x;
                    predictedX = Mathf.Lerp(inaccuratePrediction, perfectPrediction, predictionAccuracy);
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

             if(Mathf.Abs(currentVel.y) < 0.01f)
            {
                return currentPos.x;
            }

            for (int i = 0; i < 3; i++)
            {
                float timeToReachY =(targetY - currentPos.y) / currentVel.y;

                if(timeToReachY < 0 || timeToReachY > 10f)
                {
                    return currentPos.x;
                }

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
    }
}

