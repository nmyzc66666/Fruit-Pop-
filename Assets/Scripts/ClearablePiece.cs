using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 清除格子对象脚本
/// </summary>
public class ClearablePiece : MonoBehaviour
{
    /// <summary>
    /// 动画
    /// </summary>
    public AnimationClip clearAnimation;

    private bool isBeginCleared = false;
    public bool IsBeginCleared {  get { return isBeginCleared; } }

    /// <summary>
    /// 当前所在的格子对象
    /// </summary>
    public GamePiece piece;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
   
    /// <summary>
    /// 清除格子对象
    /// </summary>
    public virtual void Clear()
    {
        //增加分数
        piece.Grid.level.OnPieceCleared(piece);
       
        isBeginCleared = true;
        StartCoroutine(ClearCoroutine());
    }


    /// <summary>
    /// 清除格子对象协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearCoroutine()
    {
        //动画状态机
        Animator animator = GetComponent<Animator>();

        if(animator != null )
        {
            //播放清除动画
            animator.Play(clearAnimation.name);
      
            //等待清除动画播放完成
            yield return new WaitForSeconds(clearAnimation.length);
            //删除该游戏对象  == 格子对象
            Destroy(gameObject);
        }


    }


}
