using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 关卡基类
/// </summary>
public class Level : MonoBehaviour
{
    public enum LevelType
    {
        Time,
        Obstacle,
        Moves,
    }
    /// <summary>
    /// 网格类
    /// </summary>
    public Grid grid;

    /// <summary>
    /// 分数
    /// </summary>
    public int Score1Star;
    public int Score2Star;
    public int Score3Star;

    /// <summary>
    /// 当前分数
    /// </summary>
    public int currentScore;

    /// <summary>
    /// 关卡类型
    /// </summary>
    protected LevelType type;
    public LevelType Type {  get { return type; } }

    /// <summary>
    /// HUD UI
    /// </summary>
    public HUD hud;
    /// <summary>
    /// 是否胜利
    /// </summary>
    protected bool didWin;


    // Start is called before the first frame update
    void Start()
    {
        //设置分数
        hud.SetScore(currentScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void GameWin()
    {
        grid.GameOver();
        didWin = true;
        //等待填充完成 判断失败或者成功
        StartCoroutine(WaitForGridFill());
    }

    public virtual void GameLose()
    {
        grid.GameOver();
        didWin = false;
        //等待填充完成 判断失败或者成功
        StartCoroutine(WaitForGridFill());
    }
    /// <summary>
    /// 移动格子时调用
    /// </summary>
    public virtual void OnMove()
    {


    }
    /// <summary>
    /// 消除格子时调用
    /// </summary>
    /// <param name="piece"></param>
    public virtual void OnPieceCleared(GamePiece piece)
    {
        //加分
        currentScore += piece.score;
        //更新HUD
        hud.SetScore(currentScore);

    }

    /// <summary>
    /// 等待填充完成 判断胜利或者失败
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator WaitForGridFill()
    {
        //等待填充完成
        while (grid.IsFilling)
        {
            yield return null;
        }

        //判断胜利或者失败
        if (didWin)
        {
            hud.OnGameWin(currentScore);
        }
        else
        {
            hud.OnGameLose();
        }



    }

}
