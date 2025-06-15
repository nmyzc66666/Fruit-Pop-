using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 清除相同颜色的格子对象脚本
/// </summary>
public class ClearColorPiece : ClearablePiece
{
    /// <summary>
    /// 要吧删除的颜色
    /// </summary>
    private ColorPiece.ColorType color;
    public ColorPiece.ColorType Color {  get { return color; }set { color = value; } }

    /// <summary>
    /// 清除指定相同颜色的格子对象
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        GameMusicManager.Instance.PlaySpecialClearMusic();
        piece.Grid.ClearColorPiece(color);


    }



}
