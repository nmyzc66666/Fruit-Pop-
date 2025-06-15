using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���Ӷ�����ƶ��ű�
/// </summary>
public class MovablePiece : MonoBehaviour
{
    /// <summary>
    /// ���Ӷ���
    /// </summary>
    private GamePiece piece;
    /// <summary>
    /// �ƶ�Э�� �� �����ƶ� ���ҿ�����ʱ��ͣ��һ��Э��
    /// </summary>
    private IEnumerator moveCoroutine;
    private void Awake()
    {
        //��ȡ���Ӷ���
        piece = GetComponent<GamePiece>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region ���ĺ���
    
    /// <summary>
    /// �ƶ����� �ڲ��ƶ�����Э��ʵ��
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="fillTime">�ƶ��ȴ�ʱ��</param>
    public void Move(int newX,int newY,float fillTime)
    {
        if(moveCoroutine != null)
        {
            //��ͣЭ��
            StopCoroutine(moveCoroutine);
        }

        //�����µ��ƶ�Э��
        moveCoroutine = MoveCoroutine(newX,newY, fillTime);
        StartCoroutine(moveCoroutine);

    }
    /// <summary>
    /// �����ƶ��߼�
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="time">�ƶ��ȴ�ʱ��</param>
    /// <returns></returns>
    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {

        //��������
        piece.X = newX;
        piece.Y = newY;

        //��ʼλ��
        Vector3 startPos = transform.position;
        //����λ��
        Vector3 endPos = piece.Grid.GetWorldPosition(newX,newY);

        //��ʼ�����ƶ�
        for(float t = 0;t < time;t = t + Time.deltaTime)
        {
            //�ı�����
            piece.transform.position = Vector3.Lerp(startPos, endPos, t / time); 
            yield return 0;

        }

        //ȷ���ܹ�������λ��
        piece.transform.position = endPos;

    }


    #endregion

}
