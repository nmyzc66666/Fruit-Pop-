using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ͬ��ɫ�ĸ��Ӷ���ű�
/// </summary>
public class ClearColorPiece : ClearablePiece
{
    /// <summary>
    /// Ҫ��ɾ������ɫ
    /// </summary>
    private ColorPiece.ColorType color;
    public ColorPiece.ColorType Color {  get { return color; }set { color = value; } }

    /// <summary>
    /// ���ָ����ͬ��ɫ�ĸ��Ӷ���
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        GameMusicManager.Instance.PlaySpecialClearMusic();
        piece.Grid.ClearColorPiece(color);


    }



}
