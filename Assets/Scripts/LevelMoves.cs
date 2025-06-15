using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 计步关卡
/// </summary>
public class LevelMoves : Level
{
    /// <summary>
    /// 最大移动次数
    /// </summary>
    public int numMoves;
    /// <summary>
    /// 目标分数
    /// </summary>
    public int targetScore;
    /// <summary>
    /// 当前移动次数
    /// </summary>
    private int movesUsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.Moves;
        //更新HUD中的关卡类型信息
        hud.SetLevelType(type);
        //设置当前分数
        hud.SetScore(currentScore);

        //设置剩余步数
        hud.SetRemaining(numMoves);
        //设置目标分数
        hud.SetTarget(targetScore);


    }

    /// <summary>
    /// 格子对象移动了
    /// </summary>
    public override void OnMove()
    {
        //移动步数+1
       movesUsed++;

        //设置剩余步数
        hud.SetRemaining(numMoves - movesUsed);



        //到达最大移动次数
        if (numMoves - movesUsed == 0)
        {
            //当前分数大于目标分数
            if(currentScore > targetScore)
            {
                //获胜
                GameWin();
            }
            else
            {
                //失败
                GameLose();
            }

        }

    }

}
