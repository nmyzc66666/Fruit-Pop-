using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 格子对象的移动脚本
/// </summary>
public class MovablePiece : MonoBehaviour
{
    /// <summary>
    /// 格子对象
    /// </summary>
    private GamePiece piece;
    /// <summary>
    /// 移动协程 ： 缓慢移动 并且可以随时暂停上一个协程
    /// </summary>
    private IEnumerator moveCoroutine;
    private void Awake()
    {
        //获取格子对象
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

    #region 核心函数
    
    /// <summary>
    /// 移动函数 内部移动：用协程实现
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="fillTime">移动等待时间</param>
    public void Move(int newX,int newY,float fillTime)
    {
        if(moveCoroutine != null)
        {
            //暂停协程
            StopCoroutine(moveCoroutine);
        }

        //启动新的移动协程
        moveCoroutine = MoveCoroutine(newX,newY, fillTime);
        StartCoroutine(moveCoroutine);

    }
    /// <summary>
    /// 具体移动逻辑
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="time">移动等待时间</param>
    /// <returns></returns>
    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {

        //更新坐标
        piece.X = newX;
        piece.Y = newY;

        //起始位置
        Vector3 startPos = transform.position;
        //结束位置
        Vector3 endPos = piece.Grid.GetWorldPosition(newX,newY);

        //开始缓慢移动
        for(float t = 0;t < time;t = t + Time.deltaTime)
        {
            //改变坐标
            piece.transform.position = Vector3.Lerp(startPos, endPos, t / time); 
            yield return 0;

        }

        //确保能够到结束位置
        piece.transform.position = endPos;

    }


    #endregion

}
