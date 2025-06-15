using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 格子对象 ： 记录格子对象的所有信息 
/// </summary>
public class GamePiece : MonoBehaviour
{
    /// <summary>
    /// 当前格子游戏对象的坐标
    /// </summary>
    private int x;
    private int y;
    public int X { get { return x; } 
        set
        {
            //判断是否能移动 能就更新坐标
            if (IsMovable())
            {
                x = value;
            }

        } }
    public int Y { get { return y; } 
        set
        {
            //判断是否能移动 能就更新坐标
            if (IsMovable())
            {
                y = value;
            }
        }
    }

    /// <summary>
    /// 该格子对象 值的分数
    /// </summary>
    public int score;

    /// <summary>
    /// 格子对象类型
    /// </summary>
    private Grid.PieceType type;
    public Grid.PieceType Type { get { return type; } }

    /// <summary>
    /// 所在的网格
    /// </summary>
    private Grid grid;
    public Grid Grid { get { return grid; } }

    /// <summary>
    /// 格子对象的移动脚本
    /// </summary>
    private MovablePiece movableCompinent;
    public MovablePiece MovableCompinent { get { return movableCompinent; } }

    /// <summary>
    /// 格子对象的颜色(种类)
    /// </summary>
    private ColorPiece colorCompinent;
    public ColorPiece ColorCompinent { get { return colorCompinent; } } 

    /// <summary>
    /// 清除格子对象脚本
    /// </summary>
    private ClearablePiece clearableCompinent;
    public ClearablePiece ClearableCompinent { get { return clearableCompinent; } }



    private void Awake()
    {
        movableCompinent  = GetComponent<MovablePiece>();
        colorCompinent = GetComponent<ColorPiece>();
        clearableCompinent = GetComponent <ClearablePiece>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region 核心函数
     /// <summary>
     /// 初始化数据函数
     /// </summary>
    public void Init(int _x,int _y,Grid.PieceType _pieceType,Grid _grid)
    {
        x = _x;
        y = _y;
        type = _pieceType;
        grid = _grid;
    }
    /// <summary>
    /// 判断该格子对象能不能移动 有移动脚本就可以移动
    /// </summary>
    /// <returns></returns>
    public bool IsMovable()
    {
        return movableCompinent != null;
    }
    /// <summary>
    /// 判断该格子对象能不能设置颜色 有颜色脚本就可以设置
    /// </summary>
    /// <returns></returns>
    public bool IsColored()
    {
        return colorCompinent != null;
    }

    /// <summary>
    /// 判断该格子对象能不能清除
    /// </summary>
    /// <returns></returns>
    public bool IsClearable()
    {
        return clearableCompinent != null;
    }


    /// <summary>
    /// 鼠标进入
    /// </summary>
    private void OnMouseEnter()
    {
       
        grid.EnterPiece(this);
    }
    /// <summary>
    /// 鼠标按下
    /// </summary>
    private void OnMouseDown()
    {
        grid.PressPiece(this);
    }
    /// <summary>
    /// 鼠标抬起
    /// </summary>
    private void OnMouseUp()
    {
        grid.ReleasePiece();
    }


    #endregion
}
