using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// HUD UI
/// </summary>
public class HUD : MonoBehaviour
{
    /// <summary>
    /// 当前关卡
    /// </summary>
    public Level level;

    /// <summary>
    /// 游戏结束脚本
    /// </summary>
    public GameOver gameOver;

    //一些基础配置
    public Text remainingText;//剩余
    public Text remainingSubtext;//根据类型判断是剩余哪种类型的关卡标题
    public Text targetText;//目标
    public Text targetSubtext;//根据类型判断是哪种类型的目标关卡标题
    public Text scoreText;//得分
    public GameObject[] stars; //星星数量UI数组
    /// <summary>
    /// 当前关卡的最大星数
    /// </summary>
    private int starIndex = 0;




    // Start is called before the first frame update
    void Start()
    {
        
        //默认设置成0颗星
        for(int i = 0; i < stars.Length; i++)
        {
           if(i == starIndex)
            {
                stars[i].SetActive(true);
            }
            else
            {
                stars[i].SetActive(false);
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    #region 核心函数
    /// <summary>
    /// 设置分数
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();

        //星数
        int visibleStar = 0;

        //设置星数
        if (score >= level.Score1Star && score < level.Score2Star)
        {
            visibleStar = 1;
        }
        else if (score >= level.Score2Star && score < level.Score3Star)
        {
            visibleStar = 2;
        }
        else if (score >= level.Score3Star)
        {
            visibleStar = 3;
        }

        //设置星星显示
        for(int i = 0;i < stars.Length;i++)
        {
            if(i == visibleStar)
            {
                stars[i].SetActive(true);
            }
            else
            {
                stars[i].SetActive(false);
            }


        }

        //保存星数
        starIndex = math.max( visibleStar,starIndex);
    
    }
    
    /// <summary>
    /// 设置目标分数
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(int target)
    {
        targetText.text = target.ToString();

    }
    /// <summary>
    /// 设置剩余步数
    /// </summary>
    /// <param name="remaining"></param>
    public void SetRemaining(int remaining)
    {
        remainingText.text = remaining.ToString();
    }

    /// <summary>
    /// 设置剩余时间
    /// </summary>
    /// <param name="remaining"></param>
    public void SetRemaining(string remaining)
    {
        remainingText.text = remaining;
    }
    /// <summary>
    /// 根据关卡类型 显示不同内容
    /// </summary>
    /// <param name="type"></param>
    public void SetLevelType(Level.LevelType type)
    {
        if(type == Level.LevelType.Moves)
        {
            remainingSubtext.text = "剩余步数";
            targetSubtext.text = "目标分数";

        }else if(type == Level.LevelType.Obstacle)
        {
            remainingSubtext.text = "剩余步数";
            targetSubtext.text = "障碍物数量";
        }
        else if(type == Level.LevelType.Time)
        {
            remainingSubtext.text = "剩余时间";
            targetSubtext.text = "目标分数";
        }


    }

    /// <summary>
    /// 游戏胜利
    /// </summary>
    /// <param name="score"></param>
    public void OnGameWin(int score)
    {
        //调用结束脚本的胜利函数
        gameOver.ShowWin(score,starIndex);
        GameMusicManager.Instance.PlayWinMusic();        
        //当前场景名称
        string levelName = SceneManager.GetActiveScene().name;

        //保存关卡数据 场景名字是Key 星是Value
        if(starIndex > PlayerPrefs.GetInt(levelName))
        {
            PlayerPrefs.SetInt(levelName,starIndex);
        }


    }
    /// <summary>
    /// 游戏失败
    /// </summary>
    public void OnGameLose()
    {
        GameMusicManager.Instance.PlayLoseMusic();
        //调用结束脚本的失败函数
        gameOver.ShowLose();

    }
    /// <summary>
    /// 退出当前关卡 到 选择场景关卡
    /// </summary>
    public void QuitToSelect()
    {
        GameMusicManager.Instance.PlayButtonMusic();
        SceneManager.LoadScene("LevelSelect");
      
    }

    #endregion


}
