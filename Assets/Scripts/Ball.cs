using UnityEngine;

namespace PongGame.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ball : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float initialSpeed;
        [SerializeField] private float speedIncrease;

        private Rigidbody2D _rigidbody2D;
        private float _currentSpeed;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            Launch();
        }
        private void FixedUpdate()
        {
            _rigidbody2D.linearVelocity = _rigidbody2D.linearVelocity.normalized * _currentSpeed;
        }

        private void Launch()
        {
            _currentSpeed = initialSpeed;

            float x = Random.value > 0.5f ? 1f : -1f;
            float y = Random.Range(-0.5f,0.5f);

            Vector2 ballDirection = new Vector2(x,y).normalized;
            _rigidbody2D.linearVelocity = ballDirection * _currentSpeed;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _currentSpeed += speedIncrease;   
        }
    }
}
