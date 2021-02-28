using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _spriteRenderer;
    [SerializeField] private GameObject[] _walls;
    public Vector2Int Position;
    public GameManager.CellMaster CellMast = GameManager.CellMaster.empty;


    public void SelectColor(GameManager.CellMaster val)
    {
        if(val == GameManager.CellMaster.enemy)
        {
            for(int i = 0; i < _spriteRenderer.Length; i++)
            {
                _spriteRenderer[i].color = Color.blue;
            }
        }
        else if(val == GameManager.CellMaster.player)
        {
            for (int i = 0; i < _spriteRenderer.Length; i++)
            {
                _spriteRenderer[i].color = Color.red;
            }
        }
        GameManager.unit.Remove(gameObject);
        CellMast = val;
    }



    public void SwitchWall(int i , bool mode)
    {
        _walls[i].SetActive(mode);
    }
}
