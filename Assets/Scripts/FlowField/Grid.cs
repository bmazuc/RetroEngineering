using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Grid
{
    private int width;
    private int length;
    public Cell[] cells;
    private Vector3 cellSize = Vector3.one;
    int idx;

    private Vector3 gridPosition;

    [Header("Debug")]
    [SerializeField] private float yOffset = 0.1f;
    [SerializeField] private bool drawGrid = true;
    [SerializeField] private Color gridColor = Color.black;
    [SerializeField] private bool drawCoordinate = true;
    [SerializeField] private Color coordinateColor = Color.red;
    [SerializeField] private int coordinateFontSize = 12;
    [SerializeField] private bool drawDistanceLayer = true;
    [SerializeField] private Color distanceLayerColor = Color.red;
    [SerializeField] private int distanceLayerFontSize = 12;
    [SerializeField] private bool drawVectorLayer = true;
    [SerializeField] private Color vectorColor = Color.green;

    GUIStyle style;
    Vector3 bottomLeft;
    Vector3 topLeft;
    Vector3 bottomRight;
    Vector3 topRight;
    Vector3 center;

    public void Init(Vector3 inSize, Vector3 inPosition)
    {
        style = new GUIStyle();
        width = (int)inSize.x;
        length = (int)inSize.z;
        gridPosition = inPosition + cellSize / 2;
    }

    public void Generate()
    {
        cells = new Cell[width * length];

        for (int w = 0; w < width; ++w)
        {
            for (int l = 0; l < length; ++l)
            {
                idx = w * length + l;
                cells[idx] = new Cell(new Vector3(w, 0, l));
            }
        }
    }

    public Cell GetCellFromWorld(Vector3 position)
    {
        position -= gridPosition;
        position += cellSize / 2;

        return GetCellAtPosition(position);
    }

    private Cell GetCellAtPosition(Vector3 position)
    {
        if (position.x < 0 || position.x > width - 1)
            return null;

        if (position.z < 0 || position.z > length - 1)
            return null;

        int idx = (int)position.x * length + (int)position.z;

        return cells[idx];
    }

    public Cell[] GetNeighbours(Cell current)
    {
        Cell[] result = new Cell[4];

        Vector3 position = current.position;

        // North
        result[0] = GetCellAtPosition(position + Vector3.forward);

        // East
        result[1] = GetCellAtPosition(position + Vector3.right);

        // South
        result[2] = GetCellAtPosition(position + Vector3.back);

        // West
        result[3] = GetCellAtPosition(position + Vector3.left);

        return result;
    }

    public void Draw()
    {
        for (int w = 0; w < width; ++w)
        {
            for (int l = 0; l < length; ++l)
            {
                idx = w * length + l;
                if (idx >= cells.Length)
                    break;

                Cell cell = cells[idx];

                if (drawGrid)           DrawCell(cell, gridColor);
                if (drawCoordinate)     DrawCoordinate(cell, coordinateColor, coordinateFontSize);
                if (drawDistanceLayer)  DrawDistance(cell, distanceLayerColor, distanceLayerFontSize);
                if (drawVectorLayer)    DrawDirection(cell, vectorColor);
            }
        }
    }

    public void DrawCell(Cell cell, Color color)
    {
        Gizmos.color = color;

        bottomLeft = new Vector3(cell.position.x + gridPosition.x - 0.5f, yOffset, cell.position.z + gridPosition.z - 0.5f);
        topLeft = new Vector3(cell.position.x + gridPosition.x - 0.5f, yOffset, cell.position.z + gridPosition.z + 0.5f);

        bottomRight = new Vector3(cell.position.x + gridPosition.x + 0.5f, yOffset, cell.position.z + gridPosition.z - 0.5f);
        topRight = new Vector3(cell.position.x + gridPosition.x + 0.5f, yOffset, cell.position.z + gridPosition.z + 0.5f);

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);        
    }

    public void DrawCoordinate(Cell cell, Color color, int fontSize)
    {
        center = new Vector3(cell.position.x + gridPosition.x, yOffset, cell.position.z + gridPosition.z);
        style.normal.textColor = color;
        style.fontSize = fontSize;
        Handles.Label(center, cell.position.x + "  " + cell.position.z, style);
    }

    public void DrawDistance(Cell cell, Color color, int fontSize)
    {
        if (cell.distance == int.MaxValue)
            return;

        center = new Vector3(cell.position.x + gridPosition.x, yOffset, cell.position.z + gridPosition.z);
        style.normal.textColor = color;
        style.fontSize = fontSize;
        Handles.Label(center, cell.distance.ToString(), style);
    }

    public void DrawDirection(Cell cell, Color color)
    {
        center = new Vector3(cell.position.x + gridPosition.x, yOffset, cell.position.z + gridPosition.z);
        Gizmos.color = color;
        Gizmos.DrawLine(center, center + new Vector3(cell.direction.x, 0, cell.direction.y) * 0.2f);
    }
}
