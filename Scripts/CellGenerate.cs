using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGenerate : MonoBehaviour
{
    [SerializeField] private int _xSize;
    [SerializeField] private int _ySize;
    [SerializeField] private GameObject _cellPrefab;
    private Cell[,] _cells;

    public int XSize
    {
        get { return _xSize; }
    }

    public int YSize
    {
        get { return _ySize; }
    }

    public Cell[,] Cells
    {
        get { return _cells; }
    }

    void Start()
    {
        _cells = new Cell[_xSize + 2, _ySize + 5];
        GenerateCells();      
    }

    private void GenerateCells()
    {
        for(int x = 2; x < _xSize + 2; x++)
        {
            for (int y = 5; y < _ySize + 5; y++)
            {
                var cell = Instantiate(_cellPrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Cell>();
                _cells[x, y] = cell;
                GameManager.unit.Add(cell.gameObject);
            }
        }
    }

}
