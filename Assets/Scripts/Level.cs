using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �ؿ�����
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
    /// ������
    /// </summary>
    public Grid grid;

    /// <summary>
    /// ����
    /// </summary>
    public int Score1Star;
    public int Score2Star;
    public int Score3Star;

    /// <summary>
    /// ��ǰ����
    /// </summary>
    public int currentScore;

    /// <summary>
    /// �ؿ�����
    /// </summary>
    protected LevelType type;
    public LevelType Type {  get { return type; } }

    /// <summary>
    /// HUD UI
    /// </summary>
    public HUD hud;
    /// <summary>
    /// �Ƿ�ʤ��
    /// </summary>
    protected bool didWin;


    // Start is called before the first frame update
    void Start()
    {
        //���÷���
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
        //�ȴ������� �ж�ʧ�ܻ��߳ɹ�
        StartCoroutine(WaitForGridFill());
    }

    public virtual void GameLose()
    {
        grid.GameOver();
        didWin = false;
        //�ȴ������� �ж�ʧ�ܻ��߳ɹ�
        StartCoroutine(WaitForGridFill());
    }
    /// <summary>
    /// �ƶ�����ʱ����
    /// </summary>
    public virtual void OnMove()
    {


    }
    /// <summary>
    /// ��������ʱ����
    /// </summary>
    /// <param name="piece"></param>
    public virtual void OnPieceCleared(GamePiece piece)
    {
        //�ӷ�
        currentScore += piece.score;
        //����HUD
        hud.SetScore(currentScore);

    }

    /// <summary>
    /// �ȴ������� �ж�ʤ������ʧ��
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator WaitForGridFill()
    {
        //�ȴ�������
        while (grid.IsFilling)
        {
            yield return null;
        }

        //�ж�ʤ������ʧ��
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
