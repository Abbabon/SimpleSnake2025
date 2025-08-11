using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _fruitPrefab;
    [SerializeField] private GameObject _wallPrefab;

    [SerializeField] private int _levelWidth = 10;
    [SerializeField] private int _levelHeight = 10;

    [SerializeField] private GameObject _snakePartPrefab;
    [SerializeField] private Vector3 _startingLocation = new Vector3(5, 5, 0);
    
    private List<GameObject> _snakeParts = new();
    private GameObject _currentFruit;
    private int _score;

    public static GameManager Instance { get; private set; }
    
    public GameState CurrentGameState { get; private set; } = GameState.NotStarted;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        StartGame();
    }

    private void StartGame()
    {
        BuildLevel();
        CreateSnakePart(_startingLocation);
        SpawnFruit();
        _score = 0;
    }

    private void CreateSnakePart(Vector3 position)
    {
        var newPart = Instantiate(_snakePartPrefab, position, Quaternion.identity);
        _snakeParts.Add(newPart);
    }

    private void Start()
    {
        PlayerController.Instance.OnPlayerInput += OnPlayerInput;
    }

    private void OnPlayerInput()
    {
        if (CurrentGameState == GameState.NotStarted)
        {
            CurrentGameState = GameState.InProgress;   
        }
    }

    private void BuildLevel()
    {
        var bottomWall = Instantiate(_wallPrefab, new Vector3(-1, -1, 0), Quaternion.identity);
        bottomWall.transform.localScale = new Vector3(_levelWidth + 2, 1, 1);
        
        var topWall = Instantiate(_wallPrefab, new Vector3(-1, _levelHeight, 0), Quaternion.identity);
        topWall.transform.localScale = new Vector3(_levelWidth + 2, 1, 1);
        
        var leftWall = Instantiate(_wallPrefab, new Vector3(-1, 0, 0), Quaternion.identity);
        leftWall.transform.localScale = new Vector3(1, _levelHeight, 1);
        
        var rightWall = Instantiate(_wallPrefab, new Vector3(_levelWidth, 0, 0), Quaternion.identity);
        rightWall.transform.localScale = new Vector3(1, _levelHeight, 1);
    }

    private void SpawnFruit()
    {
        _currentFruit = Instantiate(_fruitPrefab, GetSpawnPosition(), Quaternion.identity);
    }

    private Vector3 GetSpawnPosition()
    {
        var x = UnityEngine.Random.Range(1, _levelWidth);
        var y = UnityEngine.Random.Range(1, _levelHeight);
        var suggestedPosition = new Vector3(x, y, 0);

        if (IsOnSnake(suggestedPosition))
        {
            // dont hate me
            return GetSpawnPosition();
        }

        return suggestedPosition;
    }

    public void TryMoveSnake(Vector3 nextHeadDirection)
    {
        var headPosition = _snakeParts.Last().transform.position;
        var newHeadPosition = headPosition + nextHeadDirection;

        var isOnFruit = _currentFruit &&
                        Vector3.Distance(newHeadPosition, _currentFruit.transform.position) < 0.1f;
        if (isOnFruit)
        {
            Destroy(_currentFruit);
            RaiseScore();
            SpawnFruit();
            ElongateSnake(newHeadPosition);
            return;
        }
        
        var isOnWall = newHeadPosition.y >= _levelHeight || 
                       newHeadPosition.y < 0 ||
                       newHeadPosition.x >= _levelWidth ||
                       newHeadPosition.x < 0;
        if (isOnWall)
        {
            HandleGameOver();
            return;
        }
        
        var isOnSnake = IsOnSnake(newHeadPosition);
        if (isOnSnake)
        {
            HandleGameOver();
            return;
        }
        
        MoveSnake(newHeadPosition);
    }

    private bool IsOnSnake(Vector3 position)
    {
        return _snakeParts.Any(part => 
            Vector3.Distance(part.transform.position, position) < 0.1f);
    }

    private void HandleGameOver()
    {
        CurrentGameState = GameState.GameOver;
        // TODO: Spawn Menu...
    }

    private void MoveSnake(Vector3 newHeadPosition)
    {
        CreateSnakePart(newHeadPosition);
        RemoveSnakeTail();
    }

    private void RemoveSnakeTail()
    {
        var tail = _snakeParts.FirstOrDefault();
        if (tail != null)
        {
            _snakeParts.Remove(tail);
            Destroy(tail);    
        }
    }

    private void ElongateSnake(Vector3 newHeadPosition)
    {
        CreateSnakePart(newHeadPosition);
    }

    private void RaiseScore()
    {
        _score += 1;
        Debug.Log($"Score: {_score}");
    }
}
