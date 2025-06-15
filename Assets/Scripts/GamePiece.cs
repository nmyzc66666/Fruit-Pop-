using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���Ӷ��� �� ��¼���Ӷ����������Ϣ 
/// </summary>
public class GamePiece : MonoBehaviour
{
    /// <summary>
    /// ��ǰ������Ϸ���������
    /// </summary>
    private int x;
    private int y;
    public int X { get { return x; } 
        set
        {
            //�ж��Ƿ����ƶ� �ܾ͸�������
            if (IsMovable())
            {
                x = value;
            }

        } }
    public int Y { get { return y; } 
        set
        {
            //�ж��Ƿ����ƶ� �ܾ͸�������
            if (IsMovable())
            {
                y = value;
            }
        }
    }

    /// <summary>
    /// �ø��Ӷ��� ֵ�ķ���
    /// </summary>
    public int score;

    /// <summary>
    /// ���Ӷ�������
    /// </summary>
    private Grid.PieceType type;
    public Grid.PieceType Type { get { return type; } }

    /// <summary>
    /// ���ڵ�����
    /// </summary>
    private Grid grid;
    public Grid Grid { get { return grid; } }

    /// <summary>
    /// ���Ӷ�����ƶ��ű�
    /// </summary>
    private MovablePiece movableCompinent;
    public MovablePiece MovableCompinent { get { return movableCompinent; } }

    /// <summary>
    /// ���Ӷ������ɫ(����)
    /// </summary>
    private ColorPiece colorCompinent;
    public ColorPiece ColorCompinent { get { return colorCompinent; } } 

    /// <summary>
    /// ������Ӷ���ű�
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

    #region ���ĺ���
     /// <summary>
     /// ��ʼ�����ݺ���
     /// </summary>
    public void Init(int _x,int _y,Grid.PieceType _pieceType,Grid _grid)
    {
        x = _x;
        y = _y;
        type = _pieceType;
        grid = _grid;
    }
    /// <summary>
    /// �жϸø��Ӷ����ܲ����ƶ� ���ƶ��ű��Ϳ����ƶ�
    /// </summary>
    /// <returns></returns>
    public bool IsMovable()
    {
        return movableCompinent != null;
    }
    /// <summary>
    /// �жϸø��Ӷ����ܲ���������ɫ ����ɫ�ű��Ϳ�������
    /// </summary>
    /// <returns></returns>
    public bool IsColored()
    {
        return colorCompinent != null;
    }

    /// <summary>
    /// �жϸø��Ӷ����ܲ������
    /// </summary>
    /// <returns></returns>
    public bool IsClearable()
    {
        return clearableCompinent != null;
    }


    /// <summary>
    /// ������
    /// </summary>
    private void OnMouseEnter()
    {
       
        grid.EnterPiece(this);
    }
    /// <summary>
    /// ��갴��
    /// </summary>
    private void OnMouseDown()
    {
        grid.PressPiece(this);
    }
    /// <summary>
    /// ���̧��
    /// </summary>
    private void OnMouseUp()
    {
        grid.ReleasePiece();
    }


    #endregion
}
