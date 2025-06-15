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
    /// ��ǰ�ؿ�
    /// </summary>
    public Level level;

    /// <summary>
    /// ��Ϸ�����ű�
    /// </summary>
    public GameOver gameOver;

    //һЩ��������
    public Text remainingText;//ʣ��
    public Text remainingSubtext;//���������ж���ʣ���������͵Ĺؿ�����
    public Text targetText;//Ŀ��
    public Text targetSubtext;//���������ж����������͵�Ŀ��ؿ�����
    public Text scoreText;//�÷�
    public GameObject[] stars; //��������UI����
    /// <summary>
    /// ��ǰ�ؿ����������
    /// </summary>
    private int starIndex = 0;




    // Start is called before the first frame update
    void Start()
    {
        
        //Ĭ�����ó�0����
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


    #region ���ĺ���
    /// <summary>
    /// ���÷���
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();

        //����
        int visibleStar = 0;

        //��������
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

        //����������ʾ
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

        //��������
        starIndex = math.max( visibleStar,starIndex);
    
    }
    
    /// <summary>
    /// ����Ŀ�����
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(int target)
    {
        targetText.text = target.ToString();

    }
    /// <summary>
    /// ����ʣ�ಽ��
    /// </summary>
    /// <param name="remaining"></param>
    public void SetRemaining(int remaining)
    {
        remainingText.text = remaining.ToString();
    }

    /// <summary>
    /// ����ʣ��ʱ��
    /// </summary>
    /// <param name="remaining"></param>
    public void SetRemaining(string remaining)
    {
        remainingText.text = remaining;
    }
    /// <summary>
    /// ���ݹؿ����� ��ʾ��ͬ����
    /// </summary>
    /// <param name="type"></param>
    public void SetLevelType(Level.LevelType type)
    {
        if(type == Level.LevelType.Moves)
        {
            remainingSubtext.text = "ʣ�ಽ��";
            targetSubtext.text = "Ŀ�����";

        }else if(type == Level.LevelType.Obstacle)
        {
            remainingSubtext.text = "ʣ�ಽ��";
            targetSubtext.text = "�ϰ�������";
        }
        else if(type == Level.LevelType.Time)
        {
            remainingSubtext.text = "ʣ��ʱ��";
            targetSubtext.text = "Ŀ�����";
        }


    }

    /// <summary>
    /// ��Ϸʤ��
    /// </summary>
    /// <param name="score"></param>
    public void OnGameWin(int score)
    {
        //���ý����ű���ʤ������
        gameOver.ShowWin(score,starIndex);
        GameMusicManager.Instance.PlayWinMusic();        
        //��ǰ��������
        string levelName = SceneManager.GetActiveScene().name;

        //����ؿ����� ����������Key ����Value
        if(starIndex > PlayerPrefs.GetInt(levelName))
        {
            PlayerPrefs.SetInt(levelName,starIndex);
        }


    }
    /// <summary>
    /// ��Ϸʧ��
    /// </summary>
    public void OnGameLose()
    {
        GameMusicManager.Instance.PlayLoseMusic();
        //���ý����ű���ʧ�ܺ���
        gameOver.ShowLose();

    }
    /// <summary>
    /// �˳���ǰ�ؿ� �� ѡ�񳡾��ؿ�
    /// </summary>
    public void QuitToSelect()
    {
        GameMusicManager.Instance.PlayButtonMusic();
        SceneManager.LoadScene("LevelSelect");
      
    }

    #endregion


}
