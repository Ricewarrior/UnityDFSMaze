using UnityEngine;
using System.Collections.Generic;

public class CellGenerator : MonoBehaviour {
    public GameObject cellPrefab;

    #region Grid Generation Variables

    [SerializeField]
    int mazeWidth = 10, mazeHeight = 10;

    [SerializeField]
    float cellSpacingX = 1f, cellSpacingY = 1f;

    float nextTime = 0f;
    int x = 0, y = 0;

    bool mazeGenerated = false;

    GameObject[][] cells;

    bool gridHighlighted = true;

    #endregion

    #region DFS Variables

    bool dfsExecuted = true;
    float dfsNextTime = 0f;

    GameObject currentCell;
    int cellX = 0, cellY = 0;
    int totalCells = 0;
    int visitedCells = 0;

    Stack<GameObject> cellStack = new Stack<GameObject>();
    List<GameObject> validNeighbours = new List<GameObject>();

    #endregion

    [SerializeField]
    float tickDelay = 0.01f, dfsTickDelay = 0.01f;

    void Start () {
        nextTime = Time.time;
        dfsNextTime = Time.time;
        cells = new GameObject[mazeWidth][];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = new GameObject[mazeHeight];
        }
        totalCells = mazeHeight * mazeWidth;
	}

    void Update()
    {
        if (Time.time > nextTime)
        {
            if (!mazeGenerated)
            {
                generateGrid();
            }
            nextTime = Time.time + tickDelay;
        }
        
        if (Time.time > dfsNextTime)
        {
            if (!dfsExecuted)
            {
                depthFirstSearch();
            }
            dfsNextTime = Time.time + dfsTickDelay;
        }
    }

    #region DFS Algorithm Functions

    // Reference for DFS implementation http://www.mazeworks.com/mazegen/mazetut/

    void depthFirstSearch()
    {
        if (currentCell == null)
        {
            currentCell = getRandomCell();
            colorCell(currentCell, Color.red);
            visitedCells = 1;
        }

        if (visitedCells < totalCells)
        {
            updateValidNeighbours();
            if (validNeighbours.Count > 0)
            {
                GameObject randomValidNeighbour = getRandomValidNeighbour();
                breakWallsBetweenCells(currentCell, randomValidNeighbour);
                cellStack.Push(currentCell);
                currentCell = randomValidNeighbour;
                visitedCells++;
            }
            else
            {
                GameObject nextCell = cellStack.Pop();
                updateCellIndex(currentCell, nextCell);
                currentCell = nextCell;
                colorCell(currentCell, Color.grey);
            }
        }
        else
        {
            dfsExecuted = true;
            clearCellColors();
        }
    }

    #region DFS Helper Functions

    void breakWallsBetweenCells(GameObject firstCell, GameObject secondCell)
    {
        colorCell(firstCell, Color.magenta);
        colorCell(secondCell, Color.cyan);
        Vector3 firstCellPosition = firstCell.transform.position;
        Vector3 secondCellPosition = secondCell.transform.position;

        if (firstCellPosition.z > secondCellPosition.z)
        {
            hideSouthCellWall(firstCell);
            hideNorthCellWall(secondCell);
            cellY--;
        }

        if (firstCellPosition.z < secondCellPosition.z)
        {
            hideNorthCellWall(firstCell);
            hideSouthCellWall(secondCell);
            cellY++;
        }

        if (firstCellPosition.x > secondCellPosition.x)
        {
            hideWestCellWall(firstCell);
            hideEastCellWall(secondCell);
            cellX--;
        }

        if (firstCellPosition.x < secondCellPosition.x)
        {
            hideEastCellWall(firstCell);
            hideWestCellWall(secondCell);
            cellX++;
        }
    }

    void updateCellIndex(GameObject firstCell, GameObject secondCell) 
    {
        Vector3 firstCellPosition = firstCell.transform.position;
        Vector3 secondCellPosition = secondCell.transform.position;
        if (firstCellPosition.z > secondCellPosition.z)
        {
            cellY--;
        }

        if (firstCellPosition.z < secondCellPosition.z)
        {
            cellY++;
        }

        if (firstCellPosition.x > secondCellPosition.x)
        {
            cellX--;
        }

        if (firstCellPosition.x < secondCellPosition.x)
        {
            cellX++;
        }
    }

    GameObject getRandomValidNeighbour()
    {
        int index = Random.Range(0, validNeighbours.Count);
        return validNeighbours[index];
    }

    void updateValidNeighbours()
    {
        validNeighbours.Clear();
        GameObject westNeighbour = getWestNeighbour();
        GameObject eastNeighbour = getEastNeighbour();
        GameObject northNeighbour = getNorthNeighbour();
        GameObject southNeighbour = getSouthNeighbour();
        if (westNeighbour != null && westNeighbour.GetComponent<Cell>().allWallsIntact)
        {
            validNeighbours.Add(westNeighbour);
        }
        if (eastNeighbour != null && eastNeighbour.GetComponent<Cell>().allWallsIntact)
        {
            validNeighbours.Add(eastNeighbour);
        }
        if (northNeighbour != null && northNeighbour.GetComponent<Cell>().allWallsIntact)
        {
            validNeighbours.Add(northNeighbour);
        }
        if (southNeighbour != null && southNeighbour.GetComponent<Cell>().allWallsIntact)
        {
            validNeighbours.Add(southNeighbour);
        }
    }

    GameObject getRandomCell()
    {
        cellX = Random.Range(0, mazeWidth);
        cellY = Random.Range(0, mazeHeight);
        return cells[cellX][cellY];
    }

    GameObject getWestNeighbour()
    {
        if (cellX > 0)
        {
            return cells[cellX - 1][cellY];
        }
        return null;
    }

    GameObject getEastNeighbour()
    {
        if (cellX < mazeWidth - 1)
        {
            return cells[cellX + 1][cellY];
        }
        return null;
    }

    GameObject getNorthNeighbour()
    {
        if (cellY < mazeHeight - 1)
        {
            return cells[cellX][cellY + 1];
        }
        return null;
    }

    GameObject getSouthNeighbour()
    {
        if (cellY > 0)
        {
            return cells[cellX][cellY - 1];
        }
        return null;
    }

    #endregion

    #endregion

    #region Grid Generation Functions
    void generateGrid()
    {
        if (x < mazeWidth && y < mazeHeight)
        {
            Vector3 cellSpacingX = new Vector3(this.cellSpacingX * x, 0f);
            Vector3 cellSpacingY = new Vector3(0f, 0f, this.cellSpacingY * y);
            cells[x][y] = (GameObject)Instantiate(cellPrefab, Vector3.zero + cellSpacingX + cellSpacingY, Quaternion.identity);
        }
        x++;
        if (x > mazeWidth && y < mazeHeight)
        {
            x = 0; y++;
        }
        if (y == mazeHeight)
        {
            mazeGenerated = true;
            dfsExecuted = false;
            x = 0; y = 0;
        }
    }

    #endregion

    #region Cell Functions
    void clearCellColors()
    {
        foreach (GameObject[] cellRow in cells)
        {
            foreach (GameObject cell in cellRow)
            {
                colorCell(cell, Color.white);
            }
        }
    }

    void colorCell(GameObject cell, Color color)
    {
        cell.GetComponent<Cell>().floor.renderer.material.color = color;
    }

    void colorCellWalls(GameObject go)
    {
        Cell cell = go.GetComponent<Cell>();
        cell.northWall.renderer.material.color = Color.blue;
        cell.eastWall.renderer.material.color = Color.red;
        cell.westWall.renderer.material.color = Color.yellow;
        cell.southWall.renderer.material.color = Color.green;
    }

    void uncolorCellWalls(GameObject go) 
    {
        Cell cell = go.GetComponent<Cell>();
        cell.northWall.renderer.material.color = Color.white;
        cell.eastWall.renderer.material.color = Color.white;
        cell.westWall.renderer.material.color = Color.white;
        cell.southWall.renderer.material.color = Color.white;
    }

    void hideNorthCellWall(GameObject go)
    {
        Cell cell = go.GetComponent<Cell>();
        cell.northWall.renderer.enabled = false;
        cell.northWall.GetComponent<MeshCollider>().enabled = false;
    }

    void hideWestCellWall(GameObject go)
    {
        Cell cell = go.GetComponent<Cell>();
        cell.westWall.renderer.enabled = false;
        cell.westWall.GetComponent<MeshCollider>().enabled = false;
    }

    void hideEastCellWall(GameObject go)
    {
        Cell cell = go.GetComponent<Cell>();
        cell.eastWall.renderer.enabled = false;
        cell.eastWall.GetComponent<MeshCollider>().enabled = false;
    }

    void hideSouthCellWall(GameObject go)
    {
        Cell cell = go.GetComponent<Cell>();
        cell.southWall.renderer.enabled = false;
        cell.southWall.GetComponent<MeshCollider>().enabled = false;
    }

    #endregion
}
