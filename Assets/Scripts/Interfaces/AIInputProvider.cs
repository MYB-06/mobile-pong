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

        [Header("Wall Settings")]
        [SerializeField] private float leftWall;
        [SerializeField] private float rightWall;

        private Rigidbody2D _ballRigidbody;

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
            if (ballTransform != null && _ballRigidbody != null)
            {
                _delayedBallPos = ballTransform.position;
                _delayedBallVel = _ballRigidbody.linearVelocity;
            }
        }
        private void Update()
        {
            if (ballTransform == null) return;

            UpdateDelayedBallData();
            CalculateTargetPosition();
            SmoothInput();          
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
      
            _targetX = predictedX;
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

                if(predictedX >= leftWall && predictedX <= rightWall)
                {
                    return predictedX;
                }

                float wallX = predictedX > rightWall ? rightWall : leftWall;
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

