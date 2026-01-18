using UnityEngine;

namespace PongGame.Input
{
    public class AIInputProvider : MonoBehaviour, IInputProvider
    {
        [Header("AI Settings")]
        [SerializeField] private Transform ballTransform;
        [SerializeField] private float reactionSpeed;
        [SerializeField] private float errorMargin;
        [SerializeField] private float smoothSpeed;
        [SerializeField] private bool usePrediction = true;

        private float _targetX;
        private float _currentInput;

        private void Update()
        {
            if (ballTransform == null) return;

            CalculateTargetPosition();
            SmoothInput();
        }
        public float GetHorizontalInput()
        {
            return _currentInput;
        }
        private void CalculateTargetPosition()
        {
            if (ballTransform == null) return;

            Vector2 ballPos = ballTransform.position;

            Rigidbody2D ballRigidbody = ballTransform.GetComponent<Rigidbody2D>();
            Vector2 ballVelocity = ballRigidbody.linearVelocity;

            float predictedX;

            if(usePrediction && Mathf.Abs(ballVelocity.y) > 0.01f)
            {
                bool ballComing = 
                    (ballVelocity.y < 0 && ballPos.y > transform.position.y) || 
                        (ballVelocity.y > 0 && ballPos.y < transform.position.y);

                if (ballComing)
                {
                    float timeToReach = Mathf.Abs((transform.position.y - ballPos.y) / ballVelocity.y);
                    predictedX = ballPos.x + (ballVelocity.x * timeToReach);
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

            float randomError = Random.Range(-errorMargin, errorMargin);
            _targetX = predictedX + randomError;
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
    }
}

