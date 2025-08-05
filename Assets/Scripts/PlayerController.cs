using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _snakePart;
    
    [Header("Movement Settings")]
    [SerializeField] private float _movementTimeout = 0.5f;

    private Vector3? _nextDirection;
    private float _movementCounter;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            _nextDirection = Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _nextDirection = Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _nextDirection = Vector3.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _nextDirection = Vector3.down;
        }
        
        _movementCounter += Time.deltaTime;
        if (_movementCounter >= _movementTimeout)
        {
            _movementCounter = 0f;
            if (_nextDirection.HasValue)
            {
                _snakePart.transform.position += _nextDirection.Value;
            }
        }
    }
}
