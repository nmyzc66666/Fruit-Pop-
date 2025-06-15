using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �Ʋ��ؿ�
/// </summary>
public class LevelMoves : Level
{
    /// <summary>
    /// ����ƶ�����
    /// </summary>
    public int numMoves;
    /// <summary>
    /// Ŀ�����
    /// </summary>
    public int targetScore;
    /// <summary>
    /// ��ǰ�ƶ�����
    /// </summary>
    private int movesUsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.Moves;
        //����HUD�еĹؿ�������Ϣ
        hud.SetLevelType(type);
        //���õ�ǰ����
        hud.SetScore(currentScore);

        //����ʣ�ಽ��
        hud.SetRemaining(numMoves);
        //����Ŀ�����
        hud.SetTarget(targetScore);


    }

    /// <summary>
    /// ���Ӷ����ƶ���
    /// </summary>
    public override void OnMove()
    {
        //�ƶ�����+1
       movesUsed++;

        //����ʣ�ಽ��
        hud.SetRemaining(numMoves - movesUsed);



        //��������ƶ�����
        if (numMoves - movesUsed == 0)
        {
            //��ǰ��������Ŀ�����
            if(currentScore > targetScore)
            {
                //��ʤ
                GameWin();
            }
            else
            {
                //ʧ��
                GameLose();
            }

        }

    }

}
