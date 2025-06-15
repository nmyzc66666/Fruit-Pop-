using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������Ӷ���ű�
/// </summary>
public class ClearablePiece : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    public AnimationClip clearAnimation;

    private bool isBeginCleared = false;
    public bool IsBeginCleared {  get { return isBeginCleared; } }

    /// <summary>
    /// ��ǰ���ڵĸ��Ӷ���
    /// </summary>
    public GamePiece piece;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
   
    /// <summary>
    /// ������Ӷ���
    /// </summary>
    public virtual void Clear()
    {
        //���ӷ���
        piece.Grid.level.OnPieceCleared(piece);
       
        isBeginCleared = true;
        StartCoroutine(ClearCoroutine());
    }


    /// <summary>
    /// ������Ӷ���Э��
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearCoroutine()
    {
        //����״̬��
        Animator animator = GetComponent<Animator>();

        if(animator != null )
        {
            //�����������
            animator.Play(clearAnimation.name);
      
            //�ȴ���������������
            yield return new WaitForSeconds(clearAnimation.length);
            //ɾ������Ϸ����  == ���Ӷ���
            Destroy(gameObject);
        }


    }


}
