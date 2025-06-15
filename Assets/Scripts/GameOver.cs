using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// ��Ϸ����UI�ű�
/// </summary>
public class GameOver : MonoBehaviour
{

    /// <summary>
    /// ��Ϸ����UI
    /// </summary>
    public GameObject screenParent;
    /// <summary>
    /// ������ʾUI
    /// </summary>
    public GameObject scoreParent;
    /// <summary>
    /// ʧ����ʾ
    /// </summary>
    public Text loseText;
    /// <summary>
    /// ����
    /// </summary>
    public Text scoreText;
    /// <summary>
    /// ��Ϸ��������������ʾ����
    /// </summary>
    public GameObject[] stars;

    // Start is called before the first frame update
    void Start()
    {
        //�Ȱ���Ϸ����UI�ر�
        screenParent.SetActive(false);

        //����ȫ������Ϊ��
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
    /// ��ʾʧ��UI
    /// </summary>
    public void ShowLose()
    {
        //����Ϸʧ�����
        screenParent.SetActive(true);
        //�رյ÷����
        scoreParent.SetActive(false);
        //������嶯��
        Animator animator = GetComponent<Animator>();

        if(animator != null )
        {
            animator.Play("GameOver");
        }

    }
    /// <summary>
    /// ��ʾ�ɹ����
    /// </summary>
    /// <param name="score"></param>
    /// <param name="starCount"></param>
    public void ShowWin(int score,int starCount)
    {
        //��Ϸ���������ʾ
        screenParent.SetActive(true);
        //��Ϸʧ����ʾ�ر�
        loseText.enabled = false;
        //���µ÷�
        scoreText.text = score.ToString();
        //�ȹر���ʾ�÷�
        scoreText.enabled = false;

        //������Ϸ������嶯��
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play("GameOver");
        }
        //��һ��Э��һ��һ����ʾ����
        StartCoroutine(ShowWinCoroutine(starCount));
    }

    /// <summary>
    /// ��ʾ����Э��
    /// </summary>
    /// <param name="starsCount"></param>
    /// <returns></returns>
    private IEnumerator ShowWinCoroutine(int starsCount)
    {
        //�ȴ�0.5f
        yield return new WaitForSeconds(0.5f);

        //�������ж��ٸ� �͸�����ʾ����
        if(starsCount < stars.Length)
        {
            //ѭ����ʾ���ǲ���0.5f��ʾһ��
            for (int i = 0; i <= starsCount; i++)
            {
                //��ʾ��ǰ����
                stars[i].SetActive(true);
                //��ʾ��ǰ�����Ժ� ���ص��ǵ�
                if(i > 0)
                {
                    stars[i - 1].SetActive(false);
                }

                yield return new WaitForSeconds(0.5f);

            }

        }
        //��ʾ����
        scoreText.enabled = true;
     }

    /// <summary>
    /// ������Ϸ��ť
    /// </summary>
    public void OnReplayClicked()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneName);

        GameMusicManager.Instance.PlayButtonMusic();

    }
    /// <summary>
    /// ��һ�ذ�ť ���ص�ѡ��ؿ�����
    /// </summary>
    public void OnDoneClicked()
    {
        SceneManager.LoadScene("LevelSelect");
        GameMusicManager.Instance.PlayButtonMusic();
    }


    }



