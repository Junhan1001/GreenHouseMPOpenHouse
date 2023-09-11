using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level3;

public class RectGrid : MonoBehaviour
{
    public int mX = 20; // maximum number of columns
    public int mY = 11; // maximum number of rows.

    [SerializeField]
    private int startX = 0;
    [SerializeField]
    private int startY = 0;

    public GameObject rectGridCellPrefab;

    GameObject[,] cells = null;

    [SerializeField]
    //private Color COLOR_WALKABLE = Color.cyan;
    private Color COLOR_WALKABLE = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.5f);
    [SerializeField]
    //private Color COLOR_NON_WALKABLE = Color.black;
    private Color COLOR_NON_WALKABLE = new Color(Color.black.r, Color.black.g, Color.black.b, 0.5f);

    public Color COLOR_PATH = Color.green;

    public bool noDiagonalMovement = false;

    public LinePathFind line;
    public PathFinder pathFinder = new PathFinder();

    // Start is called before the first frame update
    void Start()
    {
        cells = new GameObject[mX, mY];
        for (int i = startX; i < mX; i++)
        {
            for (int j = startY; j < mY; j++)
            {
                Vector2Int index = new Vector2Int(i, j);
                cells[i, j] = Instantiate(rectGridCellPrefab, new Vector3(i, j, 0.0f), Quaternion.identity, transform);
                //cells[i, j].transform.SetParent(transform, false);

                RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
                if (gridCell != null)
                {
                    gridCell.index = index;
                    gridCell.SetInnerColor(COLOR_WALKABLE);
                }

                // lets set the name of the grid cell.
                cells[i, j].name = "cell_" + i + "_" + j;
            }
        }

        pathFinder.heuristicCost = ManhattanCost;
        pathFinder.traversalCost = EuclideanCost;
        pathFinder.getNeighbours = GetNeighbours;
    }
    void ToggleWalkable(RectGridCell gridCell)
    {
        if (gridCell.isWalkable)
        {
            gridCell.SetNonWalkable();
        }
        else
        {
            gridCell.SetWalkable();
        }
    }

    public void SetCellColor(Vector2Int cellIndex, Color color)
    {
        GameObject cell = cells[cellIndex.x, cellIndex.y];
        RectGridCell gridCell = cell.GetComponent<RectGridCell>();
        gridCell.SetInnerColor(color);
    }

    public void ResetCellColors()
    {
        for (int i = 0; i < mX; i++)
        {
            for (int j = 0; j < mY; j++)
            {
                GameObject cell = cells[i, j];
                RectGridCell gridCell = cell.GetComponent<RectGridCell>();
                if (!gridCell.isWalkable)
                {
                    gridCell.SetNonWalkable();
                }
                else
                {
                    gridCell.SetWalkable();
                }

            }

        }
    }

    // Delegate implementation
    public float ManhattanCost(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    public float EuclideanCost(Vector2Int a, Vector2Int b)
    {
        return Vector2Int.Distance(a, b);
    }

    public List<Vector2Int> GetNeighbours(Vector2Int a, bool diagonalMovement)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        int x = a.x;
        int y = a.y;

        // Check up
        if (y < mY - 1)
        {
            int i = x;
            int j = y + 1;

            RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
            if (gridCell.isWalkable)
            {
                // add to the list of neighbours.
                neighbours.Add(gridCell.index);
            }
        }
        if (diagonalMovement)
        {
            // Check top-right
            if (y < mY - 1 && x < mX - 1)
            {
                int i = x + 1;
                int j = y + 1;

                RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
                if (gridCell.isWalkable)
                {
                    // add to the list of neighbours.
                    neighbours.Add(gridCell.index);
                }
            }
        }
        // Check right
        if (x < mX - 1)
        {
            int i = x + 1;
            int j = y;

            RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
            if (gridCell.isWalkable)
            {
                // add to the list of neighbours.
                neighbours.Add(gridCell.index);
            }
        }
        if (diagonalMovement)
        {
            // Check right-down
            if (x < mX - 1 && y > 0)
            {
                int i = x + 1;
                int j = y - 1;

                RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
                if (gridCell.isWalkable)
                {
                    // add to the list of neighbours.
                    neighbours.Add(gridCell.index);
                }
            }
        }
        // Check down
        if (y > 0)
        {
            int i = x;
            int j = y - 1;

            RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
            if (gridCell.isWalkable)
            {
                // add to the list of neighbours.
                neighbours.Add(gridCell.index);
            }
        }
        if (diagonalMovement)
        {
            // Check down-left
            if (y > 0 && x > 0)
            {
                int i = x - 1;
                int j = y - 1;

                RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
                if (gridCell.isWalkable)
                {
                    // add to the list of neighbours.
                    neighbours.Add(gridCell.index);
                }
            }
        }
        // Check left
        if (x > 0)
        {
            int i = x - 1;
            int j = y;

            RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
            if (gridCell.isWalkable)
            {
                // add to the list of neighbours.
                neighbours.Add(gridCell.index);
            }
        }
        if (diagonalMovement)
        {
            // Check left-top
            if (x > 0 && y < mY - 1)
            {
                int i = x - 1;
                int j = y + 1;

                RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
                if (gridCell.isWalkable)
                {
                    // add to the list of neighbours.
                    neighbours.Add(gridCell.index);
                }
            }
        }
        return neighbours;
    }

    public Color WalkableColor
    {
        get
        {
            return COLOR_WALKABLE;
        }
    }

    public Color NonWalkableColor
    {
        get
        {
            return COLOR_NON_WALKABLE;
        }
    }
}
