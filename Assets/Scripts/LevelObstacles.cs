using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 障碍赛
/// </summary>
public class LevelObstacles : Level
{
    //最大移动次数
    public int numMoves;
    /// <summary>
    /// 要消除的障碍物类型
    /// </summary>
    public Grid.PieceType[] obstacleTypes;

    /// <summary>
    /// 当前移动次数
    /// </summary>
    private int movesUsed = 0;
    /// <summary>
    /// 一共有多少个障碍物
    /// </summary>
    private int numObstaclesLeft;

    private void Start()
    {
        //关卡类型
        type = LevelType.Obstacle;

        //看一下障碍物数组里面有多少种障碍物 然后去网格里面找有多少个相同类型障碍物
        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            numObstaclesLeft += grid.GetPieceOfType(obstacleTypes[i]).Count;
        }

        //更新HUD中的关卡类型信息
        hud.SetLevelType(type);
        //设置当前分数
        hud.SetScore(currentScore);

        //设置剩余步数
        hud.SetRemaining(numMoves);
        //设置剩余的障碍物
        hud.SetTarget(numObstaclesLeft);


    }

    /// <summary>
    /// 格子对象移动时调用
    /// </summary>
    public override void OnMove()
    {
        //移动次数+1
        movesUsed++;

        //设置剩余步数
        hud.SetRemaining(numMoves - movesUsed);

        if (movesUsed - numMoves == 0 && numObstaclesLeft > 0)
        {
            //游戏失败
           GameLose();
        }

    }

    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);

        //消除的时候去 障碍物类型数组 里面看 这个被消除的格子对象是不是障碍物
        for(int i = 0;i < obstacleTypes.Length; i++)
        {
            //判断被消除的格子对象是不是障碍物
            if (obstacleTypes[i] == piece.Type)
            {
                numObstaclesLeft--;
                //更新剩余的障碍物
                hud.SetTarget(numObstaclesLeft);
                //消除所有障碍物 结束游戏
                if (numObstaclesLeft == 0)
                {
                    //额外加分 ：剩余步数 * 1000
                    currentScore += 1000 * (numMoves - movesUsed);

                    //更新分数
                    hud.SetScore(currentScore);

                    //游戏胜利
                    GameWin();
                }
            }


        }


    }

}
