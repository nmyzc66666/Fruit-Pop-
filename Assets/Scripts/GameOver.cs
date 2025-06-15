using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 游戏结束UI脚本
/// </summary>
public class GameOver : MonoBehaviour
{

    /// <summary>
    /// 游戏结束UI
    /// </summary>
    public GameObject screenParent;
    /// <summary>
    /// 分数显示UI
    /// </summary>
    public GameObject scoreParent;
    /// <summary>
    /// 失败提示
    /// </summary>
    public Text loseText;
    /// <summary>
    /// 分数
    /// </summary>
    public Text scoreText;
    /// <summary>
    /// 游戏结束面板的星星显示个数
    /// </summary>
    public GameObject[] stars;

    // Start is called before the first frame update
    void Start()
    {
        //先把游戏结束UI关闭
        screenParent.SetActive(false);

        //星数全部设置为空
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 显示失败UI
    /// </summary>
    public void ShowLose()
    {
        //打开游戏失败面板
        screenParent.SetActive(true);
        //关闭得分面板
        scoreParent.SetActive(false);
        //播放面板动画
        Animator animator = GetComponent<Animator>();

        if(animator != null )
        {
            animator.Play("GameOver");
        }

    }
    /// <summary>
    /// 显示成功面板
    /// </summary>
    /// <param name="score"></param>
    /// <param name="starCount"></param>
    public void ShowWin(int score,int starCount)
    {
        //游戏结束面板显示
        screenParent.SetActive(true);
        //游戏失败提示关闭
        loseText.enabled = false;
        //更新得分
        scoreText.text = score.ToString();
        //先关闭显示得分
        scoreText.enabled = false;

        //播放游戏结束面板动画
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play("GameOver");
        }
        //开一个协程一个一个显示星星
        StartCoroutine(ShowWinCoroutine(starCount));
    }

    /// <summary>
    /// 显示星星协程
    /// </summary>
    /// <param name="starsCount"></param>
    /// <returns></returns>
    private IEnumerator ShowWinCoroutine(int starsCount)
    {
        //等待0.5f
        yield return new WaitForSeconds(0.5f);

        //看星星有多少个 就更新显示多少
        if(starsCount < stars.Length)
        {
            //循环显示星星并且0.5f显示一个
            for (int i = 0; i <= starsCount; i++)
            {
                //显示当前星数
                stars[i].SetActive(true);
                //显示当前星数以后 隐藏低星的
                if(i > 0)
                {
                    stars[i - 1].SetActive(false);
                }

                yield return new WaitForSeconds(0.5f);

            }

        }
        //显示分数
        scoreText.enabled = true;
     }

    /// <summary>
    /// 重新游戏按钮
    /// </summary>
    public void OnReplayClicked()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneName);

        GameMusicManager.Instance.PlayButtonMusic();

    }
    /// <summary>
    /// 下一关按钮 ：回到选择关卡场景
    /// </summary>
    public void OnDoneClicked()
    {
        SceneManager.LoadScene("LevelSelect");
        GameMusicManager.Instance.PlayButtonMusic();
    }


    }



