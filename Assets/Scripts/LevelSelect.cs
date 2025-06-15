using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// �ؿ�ѡ��ű�
/// </summary>
public class LevelSelect : MonoBehaviour
{
    /// <summary>
    /// �ؿ�����ṹ��
    /// </summary>
    [Serializable]
    public struct ButtonPlayerPrefs
    {
        public GameObject gameObject;
        public string playerprefKey;
    }

    /// <summary>
    /// ��ǰ���йؿ����� == �ؿ����
    /// </summary>
    public ButtonPlayerPrefs[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        //�����ؿ����� ���ݹؿ�����ṹ��ĳ������Ƽ� Key ��ȡÿһ���������������
        for(int i = 0; i < buttons.Length; i++)
        {
            //��ǰ�ؿ��������
            int maxStar = PlayerPrefs.GetInt(buttons[i].playerprefKey);

            //�������� ͬһ���ؿ���ť ��һ��Ҫ��Ҫ��ʾ���Ӧλ�õ�����
            for(int starIndex = 1;starIndex<=3;starIndex++)
            {
                //��ȡ���Ӧ��λ�õ�UI����
                Transform star = buttons[i].gameObject.transform.Find("star"+starIndex);

                //��ǰ���� �Ƿ�С�� �������
                if(starIndex <= maxStar)
                {
                    star.gameObject.SetActive(true);
                }
                else
                {
                    star.gameObject.SetActive(false);
                }

            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ����ؿ���ť �������ּ��س���
    /// </summary>
    /// <param name="levelName"></param>
    public void OnButtonPress(string levelName)
    {

        SceneManager.LoadScene(levelName);
        GameMusicManager.Instance.PlayButtonMusic();

    }


}
