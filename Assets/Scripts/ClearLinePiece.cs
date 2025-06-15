using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������л���
/// </summary>
public class ClearLinePiece : ClearablePiece
{
    /// <summary>
    /// �Ƿ��������
    /// </summary>
    public bool IsRow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// ������л�����
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        GameMusicManager.Instance.PlaySpecialClearMusic();
        if(IsRow)//�Ƿ��ǰ�����
        {
            //�����������е�������� һ��һ�����
            piece.Grid.ClearRow(piece.Y);
        }
        else
        {
            //�����������е�������� һ��һ�����
            piece.Grid.ClearColumn(piece.X);
        }



    }

}
