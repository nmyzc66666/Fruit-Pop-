using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// ��ʱ��
/// </summary>
public class LevelTimer : Level
{
    /// <summary>
    /// �涨ʱ��
    /// </summary>
    public int timeInSeconds;
    /// <summary>
    /// Ŀ�����
    /// </summary>
    public int targetScore;

    /// <summary>
    /// ��ǰ����ʱ��
    /// </summary>
    private float timer;
    /// <summary>
    /// ��Ϸ�Ƿ����
    /// </summary>
    private bool isGameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.Time;
        //����HUD�еĹؿ�������Ϣ
        hud.SetLevelType(type);
        //���õ�ǰ����
        hud.SetScore(currentScore);

        //����ʣ��ʱ��
        hud.SetRemaining(string.Format("{0}:{1:00}",timeInSeconds / 60, timeInSeconds % 60));
        //����Ŀ�����
        hud.SetTarget(targetScore);


    }

    // Update is called once per frame
    void Update()
    {
        //�Ƿ������Ϸ
        if (isGameOver == false)
        {
            //��ǰʱ��
            timer += Time.deltaTime;

            //����ʣ��ʱ��
            hud.SetRemaining(string.Format("{0}:{1:00}",(int) math.max((timeInSeconds - timer) / 60,0),(int)math.max((timeInSeconds - timer) % 60,0)));

            //��ǰʱ�䳬���涨ʱ��
            if (timer > timeInSeconds)
            {
                //ʱ����� ��ǰ����С��Ŀ����� ʧ��
                if (currentScore < targetScore)
                {
                    GameLose();
                }
                else //��������Ŀ�����ʤ��
                {
                    GameWin();
                }
                //��Ϸ����
                isGameOver = true;

            }
        }


    }
}
