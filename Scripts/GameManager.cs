using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] private CellGenerate _cellGenerate;
	[SerializeField] private GameObject _flipButton;
	[SerializeField] private Sprite[] _numberBones;
	[SerializeField] private SpriteRenderer _firstBone;
	[SerializeField] private SpriteRenderer _secondBone;
	[SerializeField] private Sprite[] _animationBones;
	[SerializeField] private Text _amountMoveText;
	[SerializeField] private Panel _panel;
	private Vector2 _startPosition;
	public static List<GameObject> unit; // массив всех юнитов, которых мы можем выделить
	public static List<GameObject> unitSelected; // массив выделенных юнитов
	private List<GameObject> _enemyCells = new List<GameObject>();
	[SerializeField] private Vector2Int _startEnemyPos;
	[SerializeField] private Vector2Int _startPlayerPos;
	private Vector2Int[] _direction = new Vector2Int[4]
	{
		new Vector2Int(1,0),
		new Vector2Int(-1,0),
		new Vector2Int(0,1),
		new Vector2Int(0,-1)
	};

	public GUISkin skin;
	private Rect rect;
	private bool draw;
	private Vector2 startPos;
	private Vector2 endPos;
	private int _maxCells;
	public enum CellMaster { player, enemy, empty }
	private int _xLine;
	private int _yLine;
	private bool _isThrowBones;
	private int _moves = 1;
	private int _playerSelected;
	private Coroutine _anim;
	public bool IsCanHover;


	private int _enemyScore;
	private int _playerScore;

	void Awake()
	{
		unit = new List<GameObject>();
		unitSelected = new List<GameObject>();
	}

	private void Start()
	{
		ThrowBones();
		SetCell(_startEnemyPos, CellMaster.enemy, true);
		PlayerFlipFirst();
	}

	public void ThrowBones()
	{
		_xLine = Random.Range(1, 6);
		_yLine = Random.Range(1, 6);

		_maxCells = _xLine + _yLine;
		_amountMoveText.text = _maxCells.ToString();
		_playerSelected = _maxCells;
		_anim = StartCoroutine(BonesAnim());
	}

	public bool CheckToHaveMove()
	{
		for(int i = 0; i < _enemyCells.Count; i++)
		{
			for(int j = 0; j < _direction.Length; j++)
			{
				if (CheckToCanMove(new Vector2Int((int)_enemyCells[i].transform.position.x, (int)_enemyCells[i].transform.position.y)))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void PlayerFlipFirst()
	{
		ThrowBones();
		SetCell(_startPlayerPos, CellMaster.player, true);
	}

	public void PlayerFlip()
	{
		_isThrowBones = true;
		ThrowBones();
		_flipButton.SetActive(false);
	}

	public void SetBone()
	{
		_firstBone.sprite = _numberBones[_xLine];
		_secondBone.sprite = _numberBones[_yLine];
	}

	public void GameQueue()
	{
		if(unit.Count > 0)
		{
			if(CheckToCanMove() < 5)
			{
				Endgame();
			}
			if (!CheckToHaveMove())
			{
				_panel.OpenResultPanel("Вы Выйграли", _playerScore.ToString(), _enemyScore.ToString());
				_isThrowBones = false;
			}
				

			if (_moves % 2 == 0)
			{
				_flipButton.SetActive(false);
				ThrowBones();
				int i = Random.Range(0,_enemyCells.Count - 1);
				Vector2Int pos = new Vector2Int((int)_enemyCells[i].transform.position.x, (int)_enemyCells[i].transform.position.y);
				SetCell(pos, CellMaster.enemy, false); 
				_isThrowBones = false;
			}
			else
			{
				_playerScore += _maxCells;
				_flipButton.SetActive(true);
			}
			_moves++;
		}
		else
		{
			Endgame();
		}
	}


	private void Endgame()
	{
		if(_playerScore > _enemyScore)
		{
			_panel.OpenResultPanel("Вы Выйграли", _playerScore.ToString(), _enemyScore.ToString());
			IsCanHover = false;
		}
		else
		{
			_panel.OpenResultPanel("Вы проиграли", _playerScore.ToString(), _enemyScore.ToString());
			IsCanHover = false;
		}
	}

	public void SetCell(Vector2Int pos, CellMaster mast, bool isFirst)
	{
		int errorAmount = 0;

		Vector2Int startPos = pos;
		Vector2Int prevousPos = pos;
		_cellGenerate.Cells[startPos.x, startPos.y].SelectColor(mast);
		if (mast == CellMaster.enemy)
			_enemyCells.Add(_cellGenerate.Cells[startPos.x, startPos.y].gameObject);

		startPos += _direction[Random.Range(0, _direction.Length)];
		if(isFirst)
			_maxCells--;
		System.Random rnd = new System.Random();
		
		for (int i = 0; i < _maxCells; i++)
		{
			for (int j = 0; j < _direction.Length; j++)
			{
				startPos += _direction[j];
				if (CheckToCanSelect(startPos, mast))
				{
					_cellGenerate.Cells[startPos.x, startPos.y].SelectColor(mast);
					DeleteNaghbourWall(startPos, mast);
					_enemyScore++;
					prevousPos = startPos;
					if (mast == CellMaster.enemy)
						_enemyCells.Add(_cellGenerate.Cells[startPos.x, startPos.y].gameObject);
					break;
				}
				else
				{
					startPos = prevousPos;
				}
			}
			if (errorAmount > 10000)
			{
				break;
			}
			errorAmount++;
		}
		if (mast == CellMaster.enemy)
			Invoke(nameof(GameQueue), 0.3f);
	}


	private IEnumerator BonesAnim()
	{
		int i = 0;

		while (true)
		{
	
			_secondBone.sprite = _animationBones[i];
			_firstBone.sprite = _animationBones[i];
			i++;
			if(i >= _animationBones.Length)
			{
				SetBone();
				break;
			}

			yield return new WaitForSeconds(0.1f);
		}
	}

	public bool CheckToCanSelect(Vector2Int pos, CellMaster mast)
	{
		if(CheckToArea(pos))
		{
			if (_cellGenerate.Cells[pos.x, pos.y].CellMast == CellMaster.empty && CheckToNeighboor(pos, mast))
			{
				return true;
			}
		}


		return false;

	}

	private int CheckToCanMove()
	{
		int g = 0;
		for(int i = 0; i < unit.Count - 1; i++)
		{
			Vector2Int pos = new Vector2Int((int)unit[i].transform.position.x , (int)unit[i].transform.position.y);
			if(CheckToNeighboor(pos, CellMaster.empty))
			{
				g++;
			}
		}

		return g;
	}


	private void CheckToHaveNotSetted(CellMaster mast)
	{
		for (int i = 0; i < unit.Count - 1; i++)
		{
			Vector2Int pos = new Vector2Int((int)unit[i].transform.position.x, (int)unit[i].transform.position.y);

			if (CheckToNeighboor(pos, mast))
			{
				SetCell(pos, mast, false);
			}
		}
	
	}

	public bool CheckToCanMove(Vector2Int pos)
	{
		Vector2Int res = pos;
		for (int i = 0; i < _direction.Length; i++)
		{
			res += _direction[i];
			if (CheckToArea(res))
			{
				if (_cellGenerate.Cells[res.x, res.y].CellMast == CellMaster.empty)
				{
					return true;
				}
			}
			res = pos;
		}

		return false;

	}

	public bool CheckToNeighboor(Vector2Int pos, CellMaster mast)
	{
		for(int i = 0; i < _direction.Length; i++)
		{
			Vector2Int cPos = pos + _direction[i];
			if (CheckToArea(cPos))
			{

				if (_cellGenerate.Cells[cPos.x, cPos.y].CellMast == mast)
				{
					return true;
				}
			}
		}
		return false;
	}


	public void DeleteNaghbourWall(Vector2Int pos, CellMaster mast)
	{
		for (int i = 0; i < _direction.Length; i++)
		{
			Vector2Int cPos = pos + _direction[i];
			if (CheckToArea(cPos))
			{
				if (_cellGenerate.Cells[cPos.x, cPos.y].CellMast == mast)
				{
					_cellGenerate.Cells[pos.x, pos.y].SwitchWall(i , false);
					_cellGenerate.Cells[cPos.x, cPos.y].SwitchWall(ReverseWall(i) , false);
				}
				else
				{
					_cellGenerate.Cells[cPos.x, cPos.y].SwitchWall(i, true);
				}
			}
		}
	}

	private int ReverseWall(int i)
	{
		switch (i)
		{
			case 0:
				return 1;
			case 1:
				return 0;
			case 2:
				return 3;
			case 3:
				return 2;
		}

		return i;
	}

	public bool CheckToArea(Vector2Int pos)
	{
		if (pos.x >= 2 && pos.x <= _cellGenerate.XSize + 1
			&& pos.y >= 5 && pos.y <= _cellGenerate.YSize + 4)
		{
			return true;
		}
		return false;
	}

	// проверка, добавлен объект или нет
	bool CheckUnit(GameObject unit)
	{
		bool result = false;
		foreach (GameObject u in unitSelected)
		{
			if (u == unit) result = true;
		}
		return result;
	}

	void Select()
	{
		if (unitSelected.Count > 0)
		{
			for (int j = 0; j < unitSelected.Count; j++)
			{
				if(unitSelected.Count <= _playerSelected)
				{
					if(CheckToNeighboor(new Vector2Int((int)unitSelected[j].transform.position.x, (int)unitSelected[j].transform.position.y), CellMaster.player))
					{
						_playerSelected -= CheckWhoIsMine();
						_amountMoveText.text = _playerSelected.ToString();
						SetSelectedCells();
						break;
					}
				}
			}
		}
	}

	private int CheckWhoIsMine()
	{
		int res = 0;

		for (int j = 0; j < unitSelected.Count; j++)
		{
			if(unitSelected[j].GetComponent<Cell>().CellMast != CellMaster.player)
			{
				res++;
			}
		}
		return res;
	}

	void SetSelectedCells()
	{
		for (int j = 0; j < unitSelected.Count; j++)
		{
			var cell = unitSelected[j].GetComponent<Cell>();
			DeleteNaghbourWall(new Vector2Int((int)cell.gameObject.transform.position.x, (int)cell.gameObject.transform.position.y), CellMaster.player);
			cell.SelectColor(CellMaster.player);
		}
		if (_playerSelected <= 0)
		{
			_playerSelected = 0;
			_amountMoveText.text = _playerSelected.ToString();
			unitSelected.Clear();
			GameQueue();
		}

	}


	void OnGUI()
	{
	if (_isThrowBones && IsCanHover)
	{


		GUI.skin = skin;
		GUI.depth = 99;

		if (Input.GetMouseButtonDown(0))
		{
			startPos = Input.mousePosition;
			draw = true;
		}

		if (Input.GetMouseButtonUp(0))
		{
			draw = false;
			Select();
		}

		if (draw)
		{
			unitSelected.Clear();
			endPos = Input.mousePosition;
			if (startPos == endPos) return;

			rect = new Rect(Mathf.Min(endPos.x, startPos.x),
							Screen.height - Mathf.Max(endPos.y, startPos.y),
							Mathf.Max(endPos.x, startPos.x) - Mathf.Min(endPos.x, startPos.x),
							Mathf.Max(endPos.y, startPos.y) - Mathf.Min(endPos.y, startPos.y)
							);

			GUI.Box(rect, "");

			for (int j = 0; j < unit.Count; j++)
			{
				// трансформируем позицию объекта из мирового пространства, в пространство экрана
				Vector2 tmp = new Vector2(Camera.main.WorldToScreenPoint(unit[j].transform.position).x, Screen.height - Camera.main.WorldToScreenPoint(unit[j].transform.position).y);

				if (rect.Contains(tmp)) // проверка, находится-ли текущий объект в рамке
				{
					if (unitSelected.Count == 0)
					{
						unitSelected.Add(unit[j]);
					}
					else if (!CheckUnit(unit[j]))
					{
						unitSelected.Add(unit[j]);
					}
				}
			}
		}
	}
}
}