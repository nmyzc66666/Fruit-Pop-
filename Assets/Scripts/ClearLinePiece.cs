using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 清除整行或列
/// </summary>
public class ClearLinePiece : ClearablePiece
{
    /// <summary>
    /// 是否清除整行
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
    /// 清除整行或者列
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        GameMusicManager.Instance.PlaySpecialClearMusic();
        if(IsRow)//是否是按行清
        {
            //调用网格类中的清除方法 一个一个清除
            piece.Grid.ClearRow(piece.Y);
        }
        else
        {
            //调用网格类中的清除方法 一个一个清除
            piece.Grid.ClearColumn(piece.X);
        }



    }

}
