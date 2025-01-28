using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement; 

using Random = UnityEngine.Random;

// Define the WallState enum to represent the walls of a cell in the maze.
// Using the [Flags] attribute allows bitwise operations on these values.
[Flags]
public enum WallState
{
    LEFT = 1,  // 0001: Represents the left wall
    RIGHT = 2, // 0010: Represents the right wall
    UP = 4,    // 0100: Represents the top wall
    DOWN = 8,  // 1000: Represents the bottom wall
    VISITED = 128, // 1000 0000: Marks a cell as visited
}

// Position struct to represent the coordinates of a cell in the maze.
public struct Position
{
    public int X; // X-coordinate of the cell
    public int Y; // Y-coordinate of the cell
}

// Neighbour struct to represent a neighboring cell and the wall it shares with the current cell.
public struct Neighbour
{
    public Position Position;    // Position of the neighboring cell
    public WallState SharedWall; // The wall shared between the current cell and the neighboring cell
}

// Static class containing the maze generation logic.
public static class MazeGenerator
{
    private const int zero = 0;
    private const int one = 1;

    // Method to get the opposite of a given wall.
    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT; // Default case, though ideally unreachable
        }
    }

    // Core method to apply the Recursive Backtracker algorithm to generate the maze.
    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height)
    {
        var rng = new System.Random(); // Random number generator
        var positionStack = new Stack<Position>(); // Stack to hold the cells for backtracking
        var position = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) }; // Start from a random position

        maze[position.X, position.Y] |= WallState.VISITED; // Mark the starting cell as visited
        positionStack.Push(position); // Push the starting position onto the stack

        while (positionStack.Count > zero)
        {
            var current = positionStack.Pop(); // Pop the current cell from the stack
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height); // Get unvisited neighbors

            if (neighbours.Count > zero)
            {
                positionStack.Push(current); // Push the current cell back onto the stack

                // Choose a random neighbor to visit next
                var randIndex = rng.Next(zero, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                // Update the walls of the current cell and the chosen neighbor
                var nPosition = randomNeighbour.Position;
                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall; // Remove the shared wall
                maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall); // Remove the opposite wall
                maze[nPosition.X, nPosition.Y] |= WallState.VISITED; // Mark the neighbor as visited

                positionStack.Push(nPosition); // Push the neighbor onto the stack
            }
        }

        return maze; // Return the generated maze
    }

    // Method to get all unvisited neighbors of a given cell.
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();

        // Check the left neighbor
        if (p.X > zero && !maze[p.X - one, p.Y].HasFlag(WallState.VISITED))
        {
            list.Add(new Neighbour
            {
                Position = new Position { X = p.X - one, Y = p.Y },
                SharedWall = WallState.LEFT
            });
        }

        // Check the bottom neighbor
        if (p.Y > zero && !maze[p.X, p.Y - one].HasFlag(WallState.VISITED))
        {
            list.Add(new Neighbour
            {
                Position = new Position { X = p.X, Y = p.Y - one },
                SharedWall = WallState.DOWN
            });
        }

        // Check the top neighbor
        if (p.Y < height - one && !maze[p.X, p.Y + one].HasFlag(WallState.VISITED))
        {
            list.Add(new Neighbour
            {
                Position = new Position { X = p.X, Y = p.Y + one },
                SharedWall = WallState.UP
            });
        }

        // Check the right neighbor
        if (p.X < width - one && !maze[p.X + one, p.Y].HasFlag(WallState.VISITED))
        {
            list.Add(new Neighbour
            {
                Position = new Position { X = p.X + one, Y = p.Y },
                SharedWall = WallState.RIGHT
            });
        }

        return list; // Return the list of unvisited neighbors
    }

    public static WallState[,] Generate(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];

        // Initially all the walls EXIST
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                maze[i, j] = initial; // 1111
            }
        }



        if ((SceneManager.GetActiveScene().name == "level1"))
        {

            maze[zero, UnityEngine.Random.Range(zero, height)] &= ~WallState.LEFT; // Remove left wall of leftmost cell
            maze[width - one, Random.Range(zero, height)] &= ~WallState.RIGHT; // Remove right wall of rightmost cell

        }

        if ((SceneManager.GetActiveScene().name == "level2") || (SceneManager.GetActiveScene().name == "level3"))
        {
            maze[width - one, Random.Range(zero, height)] &= ~WallState.RIGHT; // Remove right wall of rightmost cell
        }



        return ApplyRecursiveBacktracker(maze, width, height);

    }
}
    
