using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 关卡选择脚本
/// </summary>
public class LevelSelect : MonoBehaviour
{
    /// <summary>
    /// 关卡保存结构体
    /// </summary>
    [Serializable]
    public struct ButtonPlayerPrefs
    {
        public GameObject gameObject;
        public string playerprefKey;
    }

    /// <summary>
    /// 当前所有关卡数组 == 关卡面板
    /// </summary>
    public ButtonPlayerPrefs[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        //遍历关卡数组 根据关卡数组结构体的场景名称即 Key 获取每一个场景保存的星数
        for(int i = 0; i < buttons.Length; i++)
        {
            //当前关卡最大星数
            int maxStar = PlayerPrefs.GetInt(buttons[i].playerprefKey);

            //遍历三次 同一个关卡按钮 看一看要不要显示相对应位置的星星
            for(int starIndex = 1;starIndex<=3;starIndex++)
            {
                //获取相对应星位置的UI对象
                Transform star = buttons[i].gameObject.transform.Find("star"+starIndex);

                //当前星数 是否小于 最大星数
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
    /// 点击关卡按钮 根据名字加载场景
    /// </summary>
    /// <param name="levelName"></param>
    public void OnButtonPress(string levelName)
    {

        SceneManager.LoadScene(levelName);
        GameMusicManager.Instance.PlayButtonMusic();

    }


}
