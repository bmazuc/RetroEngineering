using System.Collections;
using System.Collections.Generic;
//using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FlowField : MonoBehaviour
{
    private static FlowField instance;
    public static FlowField Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<FlowField>();

            return instance;
        }
    }

    [SerializeField] private Grid grid;
    public Grid Grid { get { return grid; } }
    [SerializeField] private Vector3 goal;
   // Thread thread = null;

    private MeshRenderer navigationStatic;
    Queue<Cell> queue;
    List<Vector3> obstacles = new List<Vector3>();

    Cell current;
    Cell goalCell;

    private void OnDrawGizmos()
    {
        grid.Draw();
        if (!Application.isPlaying)
            return;

        if (goalCell != null)
            grid.DrawCell(goalCell, Color.cyan);
    }

    void Start()
    {
        queue = new Queue<Cell>();

        navigationStatic = GetComponent<MeshRenderer>();
        grid.Init(navigationStatic.bounds.size, navigationStatic.bounds.center - navigationStatic.bounds.extents);
        grid.Generate();
        GenerateObstacle();
        //ThreadStart threadStart = new ThreadStart(Generate);
        //thread = new Thread(threadStart);
    }

    public void AddObstacle(Vector3 position)
    {
        obstacles.Add(position);
    }

    public void GeneratePathTo(Vector3 target)
    {
        goal = target;
        Generate();
        //thread.Start();
    }

    void Generate()
    {
        GenerateHeatMap();
        GenerateVectorField();
    }

    void GenerateObstacle()
    {
        int length = obstacles.Count;
        for (int i = 0; i < length; ++i)
        {
            Cell cell = grid.GetCellFromWorld(obstacles[i]);
            if (cell != null) cell.unpassable = true;
        }
    }

    //wavefront algorithm
    private void GenerateHeatMap()
    {
        goalCell = grid.GetCellFromWorld(goal);
        goalCell.distance = 0;
        goalCell.isMarked = true;
        queue.Enqueue(goalCell);

        while (queue.Count > 0)
        {
            current = queue.Dequeue();
            if (current.unpassable)
                continue;
            Cell[] neighbours = grid.GetNeumannNeighbours(current);
            for (int i = 0; i < 4; ++i)
            {
                Cell currentNeighbour = neighbours[i];
                if (currentNeighbour == null || currentNeighbour.isMarked)
                    continue;

                currentNeighbour.distance = current.distance + 1;
                currentNeighbour.isMarked = true;
                queue.Enqueue(currentNeighbour);
            }
        }
    }

    private void GenerateVectorField()
    {
        int length = grid.cells.Length;
        for (int i = 0; i < length; ++i)
        {
            Cell current = grid.cells[i];
            if (current.unpassable)
                continue;
            Cell[] neighbours = grid.GetNeumannNeighbours(current);

            float up = (neighbours[0] != null && !neighbours[0].unpassable) ? neighbours[0].distance : current.distance;
            float right = (neighbours[1] != null && !neighbours[1].unpassable) ? neighbours[1].distance : current.distance;
            float down = (neighbours[2] != null && !neighbours[2].unpassable) ? neighbours[2].distance : current.distance;
            float left = (neighbours[3] != null && !neighbours[3].unpassable) ? neighbours[3].distance : current.distance;

            float x = left - right;
            float y = down - up;

            current.direction = new Vector2(x, y);
            current.direction.Normalize();

            current.isMarked = false;
        }
    }
}
