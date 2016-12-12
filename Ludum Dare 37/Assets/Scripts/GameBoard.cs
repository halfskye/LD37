﻿using UnityEngine;
using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour {
    #region Initialize Vars
    [SerializeField]
    private Transform _origin;

    [SerializeField]
    private int _width = 5;
    
    [SerializeField]
    private int _height = 12;

    [SerializeField]
    private Vector2 _dim = new Vector2(1.06f, 1.06f);

    // Row[0]: [0] [1] [2] [3]
    // Row[1]: [0] [1] [2] [3]
    private List<List<GameCell>> _board = null;

    private Fortification.Type _selectedFortificationType = Fortification.Type.NONE;

    private int _score = 0;
    public Text _scoreText;
    public Text _daysTil;

    private float _globalTimer = 500.0f;

    static private GameBoard _singleton = null;
    #endregion

    #region Create Board
    private void Awake()
    {
        _singleton = this;
        CreateBoard();
    }

    private void CreateBoard()
    {
        _board = new List<List<GameCell>>();
        for (int j = 0; j < _height; ++j)
        {
            List<GameCell> row = new List<GameCell>();
            for (int i = 0; i < _width; ++i)
            {
                GameCell newCell = new GameCell(_origin.position, i, j, _dim);
                row.Add(newCell);
            }
            _board.Add(row);
        }
    }
    #endregion

    static public GameBoard Get()
    {
        return _singleton;
    }

    private void Update()
    {
        DrawBoard();

        UpdatePlayerMouse();

        UpdatePlayerKeyInput();

        //GLobal Timer Stuff
        _globalTimer -= Time.deltaTime;

        _scoreText.text = _score.ToString();
    }

    static public Vector3 GetMouseWorldPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }

    private bool IsRemoveModifierActive()
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    private void UpdatePlayerMouse()
    {
        Vector3 pos = GetMouseWorldPos();
        //DrawMousePos(pos);

        GameCell cell = GetGameCellOnWorldPos(pos);
        if (cell != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsRemoveModifierActive())
                {
                    cell.RemoveFortification();
                }
                else
                {
                    if (_selectedFortificationType != Fortification.Type.NONE)
                    {
                        cell.SetFortification(_selectedFortificationType);
                        _selectedFortificationType = Fortification.Type.NONE;
                    }
                }
            }
            else
            {
                //cell.Draw(true);
            }
        }
    }

    public void SetSelectedFortificationType(Fortification.Type type)
    {
        _selectedFortificationType = type;
    }

    private void UpdatePlayerKeyInput()
    {
        //@TEMP: Mostly TEMP / DEBUG to set Fortification Types quickly.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedFortificationType = Fortification.Type.TOWER1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedFortificationType = Fortification.Type.TOWER2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedFortificationType = Fortification.Type.TOWER3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedFortificationType = Fortification.Type.TOWER4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedFortificationType = Fortification.Type.TOWER5;
        }
    }

    private void DrawMousePos(Vector3 pos)
    {
        float crosshair = 3;
        Debug.DrawRay(pos, Vector3.up * crosshair);
        Debug.DrawRay(pos, Vector3.right * crosshair);
        Debug.DrawRay(pos, Vector3.down * crosshair);
        Debug.DrawRay(pos, Vector3.left * crosshair);
    }

    private bool myRectContainsPoint(Rect rect, Vector3 pos)
    {
        float xMin = Mathf.Min(rect.min.x, rect.max.x);
        float xMax = Mathf.Max(rect.min.x, rect.max.x);
        float yMin = Mathf.Min(rect.min.y, rect.max.y);
        float yMax = Mathf.Max(rect.min.y, rect.max.y);
        return (xMin < pos.x && pos.x < xMax &&
            yMin < pos.y && pos.y < yMax);
    }

    private Rect getBoardRect()
    {
        return new Rect(_origin.position.x, _origin.position.y, _dim.x * _width, _dim.y * -_height);
    }

    public GameCell GetGameCellOnWorldPos(Vector3 pos)
    {
        Rect boardRect = getBoardRect();
        //DrawRect(boardRect, Color.magenta, false);
        //if (boardRect.Contains(pos))
        if (myRectContainsPoint(boardRect, pos))
        {
            float realXMin = boardRect.xMin;
            float realYMin = boardRect.yMax;
            float realXMax = boardRect.xMax;
            float realYMax = boardRect.yMin;

            //Debug.Log("CONTAINS!");
            int xIndex = Mathf.FloorToInt((pos.x - realXMin) / (realXMax - realXMin) * _width);
            //int yIndex = _board.Count - Mathf.FloorToInt((pos.y - realYMin) / (realYMax - realYMin) * _height) - 1;
            int yIndex = GetRowIndexOnWorldPos(pos);

            //Debug.Log(string.Format("{0} {1}", xIndex, yIndex));
            return _board[yIndex][xIndex];
        }
        else
        {
            return null;
        }
    }

    private bool IsCellPosValid(Vector2 pos)
    {
        return (0 <= pos.x && pos.x < _width &&
            0 <= pos.y && pos.y < _height);
    }

    public GameCell GetGameCellOnCellPos(Vector2 pos)
    {
        if (IsCellPosValid(pos))
        {
            return _board[Mathf.FloorToInt(pos.y)][Mathf.FloorToInt(pos.x)];
        }
        else
        {
            return null;
        }
    }

    public int GetRowIndexOnWorldPos(Vector3 pos)
    {
        Rect boardRect = getBoardRect();
        float yMin = Mathf.Min(boardRect.yMin, boardRect.yMax);
        float yMax = Mathf.Max(boardRect.yMin, boardRect.yMax);
        if (yMin < pos.y && pos.y < yMax)
        {
            int yIndex = _board.Count - Mathf.FloorToInt((pos.y - yMin) / (yMax - yMin) * _height) - 1;
            return yIndex;
        }
        else
        {
            return -1;
        }
    }

    public GameCell GetGameCellRightOfPos(Vector3 pos)
    {
        GameCell cell = GetGameCellOnWorldPos(pos);
        if (cell != null)
        {
            Vector2 cellPos = cell.GetCellPos();
            cellPos.x += 1;
            return GetGameCellOnCellPos(cellPos);
        }
        else
        {
            //@TODO: Based on position, find the proper row... then retrieve the first in the row.
            int rowIndex = GetRowIndexOnWorldPos(pos);
            if (rowIndex != -1)
            {
                Vector2 cellPos = new Vector2(0, rowIndex);
                return GetGameCellOnCellPos(cellPos);
            }
            else
            {
                return null;
            }
        }
    }

    private void DrawBoard(bool debug=false)
    {
        for (int i = 0; i < _board.Count; ++i)
        {
            List<GameCell> row = _board[i];
            for (int j = 0; j < row.Count; ++j)
            {
                GameCell cell = row[j];
                if (debug)
                {
                    cell.UpdateDim(_dim);
                }
                cell.Draw(debug);
            }
        }
    }

    static public void DrawRect(Rect rect, Color color, bool drawX=true)
    {
        Vector3 UL = rect.min;
        Vector3 UR = new Vector3(rect.xMax, rect.yMin);
        Vector3 LL = new Vector3(rect.xMin, rect.yMax);
        Vector3 LR = rect.max;
        DrawSquare(UL, UR, LL, LR, color, drawX);
    }

    static public void DrawSquare(Vector3 UL, Vector3 UR, Vector3 LL, Vector3 LR, Color color, bool drawX=true)
    {
        Debug.DrawLine(UL, UR, color);
        Debug.DrawLine(UL, LL, color);
        Debug.DrawLine(LL, LR, color);
        Debug.DrawLine(UR, LR, color);

        // X
        if (drawX)
        {
            Debug.DrawLine(UL, LR, color);
            Debug.DrawLine(LL, UR, color);
        }

        // Sanity
        //Debug.DrawRay(UL, Vector3.left * 3, Color.green);
        //Debug.DrawRay(LR, Vector3.right * 3, Color.cyan);
    }

    // This will probably move somewhere more appropriate?
    #region Score
    public void AddScore(int score)
    {
        _score += score;
    }
    public bool SpendScore(int score)
    {
        if (_score < score)
        {
            return false;
        }
        else
        {
            _score -= score;
            return true;
        }
    }
    #endregion Score

    private void OnGUI()
    {
        Rect drawPos = new Rect(0, 0, 100, 50);
        //GUI.Label(drawPos, string.Format("SCORE: {0}", _score));
        //GUI.Label(drawPos, string.Format("SELECTED TYPE: {0}", _selectedFortificationType));
        
        //Vector3 pos = GetMouseWorldPos();
        //GameCell cell = GetGameCellOnWorldPos(pos);
        //if (cell != null)
        //{
        //    Vector3 center = cell.getRect().center;
        //    GUI.Label(drawPos, string.Format("CELL CENTER: {0:0.00}, {1:0.00}", center.x, center.y));
        //}
        //GUI.Label(drawPos, string.Format("MOUSE POS: {0:0.00}, {1:0.00}", pos.x, pos.y));

        //Rect rect = getBoardRect();
        //drawPos.y += 50;
        //GUI.Label(drawPos, string.Format("RECT UL POS: {0:0.00}, {1:0.00}", rect.min.x, rect.min.y));
        //drawPos.y += 50;
        //GUI.Label(drawPos, string.Format("RECT LR POS: {0:0.00}, {1:0.00}", rect.max.x, rect.max.y));
    }
}
