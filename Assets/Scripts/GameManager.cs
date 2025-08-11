using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _fruitPrefab;
    [SerializeField] private GameObject _wallPrefab;

    [SerializeField] private int _levelWidth = 10;
    [SerializeField] private int _levelHeight = 10;

    [SerializeField] private GameObject _snakePartPrefab;
    [SerializeField] private Vector3 _startingLocation = new Vector3(5, 5, 0);
    
    // TODO: many parts
    private GameObject _snakePart;
    private GameObject _currentFruit;
    private int _score;

    public static GameManager Instance { get; private set; }
    
    public GameState CurrentGameState { get; private set; } = GameState.NotStarted;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        StartGame();
    }

    private void StartGame()
    {
        BuildLevel();
        CreateSnakePart();
        SpawnFruit();
        _score = 0;
    }

    private void CreateSnakePart()
    {
        _snakePart = Instantiate(_snakePartPrefab, _startingLocation, Quaternion.identity);
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
        return new Vector3(x, y, 0);
    }

    public void MoveSnake(Vector3 nextDirection)
    {
        _snakePart.transform.position += nextDirection;
        
        if (_currentFruit != null &&
            Vector3.Distance(_snakePart.transform.position, _currentFruit.transform.position) < 0.1f)
        {
            Destroy(_currentFruit);
            RaiseScore();
            SpawnFruit();
        }
    }

    private void RaiseScore()
    {
        _score += 1;
        Debug.Log($"Score: {_score}");
    }
}
