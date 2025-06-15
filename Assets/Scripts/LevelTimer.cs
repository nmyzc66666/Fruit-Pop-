using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 计时赛
/// </summary>
public class LevelTimer : Level
{
    /// <summary>
    /// 规定时间
    /// </summary>
    public int timeInSeconds;
    /// <summary>
    /// 目标分数
    /// </summary>
    public int targetScore;

    /// <summary>
    /// 当前所用时间
    /// </summary>
    private float timer;
    /// <summary>
    /// 游戏是否结束
    /// </summary>
    private bool isGameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.Time;
        //更新HUD中的关卡类型信息
        hud.SetLevelType(type);
        //设置当前分数
        hud.SetScore(currentScore);

        //设置剩余时间
        hud.SetRemaining(string.Format("{0}:{1:00}",timeInSeconds / 60, timeInSeconds % 60));
        //设置目标分数
        hud.SetTarget(targetScore);


    }

    // Update is called once per frame
    void Update()
    {
        //是否结束游戏
        if (isGameOver == false)
        {
            //当前时间
            timer += Time.deltaTime;

            //更新剩余时间
            hud.SetRemaining(string.Format("{0}:{1:00}",(int) math.max((timeInSeconds - timer) / 60,0),(int)math.max((timeInSeconds - timer) % 60,0)));

            //当前时间超过规定时间
            if (timer > timeInSeconds)
            {
                //时间结束 当前分数小于目标分数 失败
                if (currentScore < targetScore)
                {
                    GameLose();
                }
                else //分数大于目标分数胜利
                {
                    GameWin();
                }
                //游戏结束
                isGameOver = true;

            }
        }


    }
}
