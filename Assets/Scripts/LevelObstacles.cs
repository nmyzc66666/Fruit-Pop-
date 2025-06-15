using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �ϰ���
/// </summary>
public class LevelObstacles : Level
{
    //����ƶ�����
    public int numMoves;
    /// <summary>
    /// Ҫ�������ϰ�������
    /// </summary>
    public Grid.PieceType[] obstacleTypes;

    /// <summary>
    /// ��ǰ�ƶ�����
    /// </summary>
    private int movesUsed = 0;
    /// <summary>
    /// һ���ж��ٸ��ϰ���
    /// </summary>
    private int numObstaclesLeft;

    private void Start()
    {
        //�ؿ�����
        type = LevelType.Obstacle;

        //��һ���ϰ������������ж������ϰ��� Ȼ��ȥ�����������ж��ٸ���ͬ�����ϰ���
        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            numObstaclesLeft += grid.GetPieceOfType(obstacleTypes[i]).Count;
        }

        //����HUD�еĹؿ�������Ϣ
        hud.SetLevelType(type);
        //���õ�ǰ����
        hud.SetScore(currentScore);

        //����ʣ�ಽ��
        hud.SetRemaining(numMoves);
        //����ʣ����ϰ���
        hud.SetTarget(numObstaclesLeft);


    }

    /// <summary>
    /// ���Ӷ����ƶ�ʱ����
    /// </summary>
    public override void OnMove()
    {
        //�ƶ�����+1
        movesUsed++;

        //����ʣ�ಽ��
        hud.SetRemaining(numMoves - movesUsed);

        if (movesUsed - numMoves == 0 && numObstaclesLeft > 0)
        {
            //��Ϸʧ��
           GameLose();
        }

    }

    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);

        //������ʱ��ȥ �ϰ����������� ���濴 ����������ĸ��Ӷ����ǲ����ϰ���
        for(int i = 0;i < obstacleTypes.Length; i++)
        {
            //�жϱ������ĸ��Ӷ����ǲ����ϰ���
            if (obstacleTypes[i] == piece.Type)
            {
                numObstaclesLeft--;
                //����ʣ����ϰ���
                hud.SetTarget(numObstaclesLeft);
                //���������ϰ��� ������Ϸ
                if (numObstaclesLeft == 0)
                {
                    //����ӷ� ��ʣ�ಽ�� * 1000
                    currentScore += 1000 * (numMoves - movesUsed);

                    //���·���
                    hud.SetScore(currentScore);

                    //��Ϸʤ��
                    GameWin();
                }
            }


        }


    }

}
