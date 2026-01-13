using PongGame.Input;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] 
public class Paddle : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [Header("Boundaries")]
    [SerializeField] private float minX = -2f;
    [SerializeField] private float maxX = 2f;
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
}