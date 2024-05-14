using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public GameObject headPrefab;
    public GameObject neckPrefab;
    public GameObject bodyPrefab;
    public GameObject tailPrefab;
    public GameObject itemPrefab;
    public float moveSpeed = 0.1f;
    public float timeToMove = 0.1f;

    private List<Transform> snakeParts = new List<Transform>();
    private Vector2Int direction = Vector2Int.right;
    private bool ateItem = false;
    private float timer;

    void Start()
    {
        timer = timeToMove;
        snakeParts.Add(CreateSnakePart(headPrefab, transform.position));
        snakeParts.Add(CreateSnakePart(neckPrefab, transform.position - new Vector3(1, 0, 0))); // Neck position is one unit to the left of the head

        // Spawn initial item
        SpawnItem();
    }

    void Update()
    {
        HandleMovement();
        HandleInput();
    }

    void HandleMovement()
    {
        timer += Time.deltaTime;

        if (timer >= timeToMove)
        {
            MoveSnake();
            timer = 0f;
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2Int.down)
            direction = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2Int.up)
            direction = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2Int.right)
            direction = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2Int.left)
            direction = Vector2Int.right;
    }

    void MoveSnake()
    {
        Vector2Int newPosition = new Vector2Int(Mathf.RoundToInt(snakeParts[0].position.x) + direction.x,
                                         Mathf.RoundToInt(snakeParts[0].position.y) + direction.y);

        // Check if snake hit border
        if (newPosition.x < -8 || newPosition.x > 8 || newPosition.y < -4 || newPosition.y > 4)
        {
            Debug.Log("Game Over");
            Destroy(gameObject);
            return;
        }

        // Move snake
        for (int i = snakeParts.Count - 1; i > 0; i--)
        {
            snakeParts[i].position = snakeParts[i - 1].position;
        }

        snakeParts[0].position = new Vector3(newPosition.x, newPosition.y, 0);

        // Check if snake ate item
        if (ateItem)
        {
            ateItem = false;
            snakeParts.Insert(2, CreateSnakePart(bodyPrefab, snakeParts[1].position)); // Insert body part after the neck
            SpawnItem();
        }
    }

    Transform CreateSnakePart(GameObject prefab, Vector3 position)
    {
        GameObject newPart = Instantiate(prefab, position, Quaternion.identity, transform.parent);
        return newPart.transform;
    }

    void SpawnItem()
    {
        Vector2Int position = Vector2Int.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            position = new Vector2Int(Random.Range(-8, 9), Random.Range(-4, 5));
            validPosition = !snakeParts.Exists(part => new Vector2Int((int)part.position.x, (int)part.position.y) == position);
        }

        Instantiate(itemPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, null);
    }

    public void EatItem()
    {
        ateItem = true;
    }
}
