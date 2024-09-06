using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public int gridSize = 25;
    private GameObject[,] grid;
    private bool[,] currentState;
    private bool[,] nextState;
    
    public GameObject cellPrefab;
    public Material aliveMaterial;
    public Material deadMaterial;
    public float iterationSpeed = 0.5f; // Public variable to set the speed of iteration
    private bool isRunning = false;
    private bool isDragging = false;
    private bool paintAlive = true;

    void Start()
    {
        grid = new GameObject[gridSize, gridSize];
        currentState = new bool[gridSize, gridSize];
        nextState = new bool[gridSize, gridSize];

        InitializeGrid();
        CenterCameraOverGrid();
        InvokeRepeating("UpdateGrid", 1f, iterationSpeed);
    }

    private void CenterCameraOverGrid()
    {
        Camera.main.transform.position = new Vector3(gridSize / 2, gridSize, gridSize / 2);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRunning = !isRunning;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            paintAlive = Input.GetMouseButtonDown(0);
            PaintCell(paintAlive);
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            PaintCell(paintAlive);
        }
    }

    void InitializeGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                grid[x, y] = Instantiate(cellPrefab, position, Quaternion.identity);
                currentState[x, y] = Random.value > 0.5f;
                UpdateCellColor(x, y);
            }
        }
    }

    void UpdateGrid()
    {
        if (!isRunning) return;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                if (currentState[x, y])
                {
                    nextState[x, y] = aliveNeighbors == 2 || aliveNeighbors == 3;
                }
                else
                {
                    nextState[x, y] = aliveNeighbors == 3;
                }
            }
        }

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                currentState[x, y] = nextState[x, y];
                UpdateCellColor(x, y);
            }
        }
    }

    void UpdateCellColor(int x, int y)
    {
        Renderer renderer = grid[x, y].GetComponent<Renderer>();
        if (currentState[x, y])
        {
            renderer.material = aliveMaterial;
        }
        else
        {
            renderer.material = deadMaterial;
        }
    }

    void PaintCell(bool state)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPosition = hit.transform.position;
            int x = Mathf.RoundToInt(hitPosition.x);
            int y = Mathf.RoundToInt(hitPosition.z);

            if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
            {
                currentState[x, y] = state;
                UpdateCellColor(x, y);
            }
        }
    }

    int CountAliveNeighbors(int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                int nx = x + i;
                int ny = y + j;
                if (nx >= 0 && nx < gridSize && ny >= 0 && ny < gridSize)
                {
                    if (currentState[nx, ny]) count++;
                }
            }
        }
        return count;
    }
}