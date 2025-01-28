using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{

    [SerializeField] private Transform exitPrefab;  // exit collider prefab

    [SerializeField, Range(1, 50)] private int width = 5;    // Maze width (modifiable in Unity Inspector)
    [SerializeField, Range(1, 50)] private int height = 5;   // Maze height (modifiable in Unity Inspector)

    [SerializeField] private float size = 1f;                // Size of each cell (adjustable)
    [SerializeField] private Transform wallPrefab = null;    // Wall prefab (assigned in Unity Inspector)
    [SerializeField] private Transform floorPrefab = null;   // Floor prefab (assigned in Unity Inspector)

    // Constants to avoid magic numbers
    private const float WALL_HEIGHT = 1f;

    private const float FLOOR_HEIGHT = 1f;

    private const float HALF_WALL_HEIGHT = 0.5f;

    private const int ZERO = 0;

    private const int ONE = 1;

    private const int TWO = 2;

    private const int NINETY = 90;


    void Start()
    {
        Time.timeScale = 1; // Reset time scale to normal, ensuring the game is not paused.

        // Generate the maze using MazeGenerator and draw it
        var maze = MazeGenerator.Generate(width, height);
        Draw(maze);
        PlaceExits(maze);
    }


    // Place exit colliders at the maze exits
    private void PlaceExits(WallState[,] maze)
    {
        for (int y = 0; y < height; y++)
        {
            // Left Exit (if left wall is missing)
            if (!maze[ZERO, y].HasFlag(WallState.LEFT))
            {
                CreateExit(new Vector3(-width / TWO - HALF_WALL_HEIGHT, ZERO, -height / TWO + y));
            }

            // Right Exit (if right wall is missing)
            if (!maze[width - ONE, y].HasFlag(WallState.RIGHT))
            {
                CreateExit(new Vector3(width / TWO - HALF_WALL_HEIGHT, ZERO, -height / TWO + y));
            }
        }
    }



    // Create exit collider at given position
    private void CreateExit(Vector3 position)
    {
        var exit = Instantiate(exitPrefab, transform);
        exit.position = position;
    }



    /// Draws the maze by placing floor and walls according to the maze data.
    private void Draw(WallState[,] maze)
    {
        // Instantiate the floor, scale it to match maze dimensions
        var floor = Instantiate(floorPrefab, transform);
        floor.localScale = new Vector3(width, FLOOR_HEIGHT, height);

        // Iterate through each cell in the maze grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = maze[x, y];
                var position = new Vector3(-width / TWO + (x * size), ZERO, -height / TWO + (y * size));

                // Draw top wall if UP flag is set
                if (cell.HasFlag(WallState.UP))
                {
                    CreateWall(position + new Vector3(ZERO, ZERO, size / TWO), Vector3.zero);
                }

                // Draw left wall if LEFT flag is set
                if (cell.HasFlag(WallState.LEFT))
                {
                    CreateWall(position + new Vector3(-size / TWO, ZERO, ZERO), new Vector3(ZERO, NINETY, ZERO));
                }

                // Draw right wall for the last column
                if (x == width - ONE && cell.HasFlag(WallState.RIGHT))
                {
                    CreateWall(position + new Vector3(size / TWO, ZERO, ZERO), new Vector3(ZERO, NINETY, ZERO));
                }

                // Draw bottom wall for the first row
                if (y == ZERO && cell.HasFlag(WallState.DOWN))
                {
                    CreateWall(position + new Vector3(ZERO, ZERO, -size / TWO), Vector3.zero);
                }
            }
        }
    }

    /// Instantiates a wall at a given position with specified rotation.
    private void CreateWall(Vector3 position, Vector3 rotation)
    {
        var wall = Instantiate(wallPrefab, transform);
        wall.position = position;
        wall.eulerAngles = rotation;
        wall.localScale = new Vector3(size, WALL_HEIGHT, wall.localScale.z);  // Consistent wall height
    }
}


