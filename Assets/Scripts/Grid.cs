using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using static Grid;
/// <summary>
/// 格子类
/// </summary>
public class Grid : MonoBehaviour
{
    public float xoffset;
    public float yoffset;

    /// <summary>
    /// 格子类型
    /// </summary>
    public enum PieceType
    {
        Empty,//空格子对象
        Normal,//正常格子对象
        Count,
        Bubble,//障碍物
        Row_Clear,//行清除特殊格子对象
        Column_Clear,//列清除格子对象
        Rainbow//清除所有相同颜色的格子对象

    };
    /// <summary>
    /// 格子结构体 ： 存储格子对象及类型
    /// </summary>
    [Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }


    //格子长宽
    public int xDim;
    public int yDim;

    /// <summary>
    /// 格子结构体数组 把结构体的数据转成 格子对象字典用的 一开始要结构体是因为方便配置及序列化
    /// </summary>
    public PiecePrefab[] piecePrefabs;
    /// <summary>
    /// 格子对象的背景框
    /// </summary>
    public GameObject backgroundPrefab;

    /// <summary>
    /// 格子对象字典 存储所有格子类型的对象
    /// </summary>
    private Dictionary<PieceType, GameObject> piecePrefabDict;

    /// <summary>
    /// 格子的游戏对象脚本 ：用来保存和操作所有实例化出来的格子对象游戏物体
    /// </summary>
    private GamePiece[,] pieces;


    /// <summary>
    /// 判断填充一次以后是否能够继续填充
    /// </summary>
    private bool movedPiece;
    /// <summary>
    /// 填充间隔时间
    /// </summary>
    public float fillTime;
    /// <summary>
    ///  inverse 变量控制横向遍历顺序（正序或逆序），模拟玩家视角的交替填充效果。
    /// </summary>
    private bool inverse;

    /// <summary>
    /// 当前被鼠标按下的格子对象
    /// </summary>
    private GamePiece pressPiece;
    /// <summary>
    /// 要被交换的格子对象
    /// </summary>
    private GamePiece enterPiece;
    /// <summary>
    /// 关卡类
    /// </summary>
    public Level level;
    /// <summary>
    /// 游戏结束判断
    /// </summary>
    private bool gameOver;

    /// <summary>
    /// 特殊格子 及 障碍物格子对象 
    /// </summary>
    [Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;

    }
    /// <summary>
    /// 配置 特殊格子 及 障碍物格子对象 生成数组
    /// </summary>
    public PiecePosition[] initialPieces;

    /// <summary>
    /// 是否正在填充
    /// </summary>
    private bool isFilling = false;
    public bool IsFilling {  get { return isFilling; } }


    // Start is called before the first frame update
    void Awake()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        //把格子结构体数组的数据 转成字典
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (piecePrefabDict.ContainsKey(piecePrefabs[i].type) == false)
            {
                //不同类型格子对象加入字典
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }

        }

        //创建每一个格子的背景框
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                //生成单个格子背景
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                //设置父对象
                background.transform.parent = transform;
            }
        }

        //初始化格子对象脚本数组 
        pieces = new GamePiece[xDim, yDim];


        //设置障碍物
        for(int i = 0 ; i < initialPieces.Length; i++)
        {

            //障碍物数组中的障碍物位置要正确
            if (initialPieces[i].x >= 0 && initialPieces[i].x < xDim && initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                //生成障碍物
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);

            }
        }



        //创建每一个格子游戏对象 默认先生成空的格子游戏对象
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                //有障碍物的地方不能生成空格子对象
                if (pieces[x, y] == null)
                {
                    //生成并且保存单个格子游戏对象 默认先生成空的格子游戏对象
                    SpawnNewPiece(x, y, PieceType.Empty);
                }

            }
        }

        


        //填充游戏格子对象
        StartCoroutine(Fill());

    }



    // Update is called once per frame
    void Update()
    {

    }

    #region 核心函数

    #region 填充和移动下落格子对象
    /// <summary>
    /// 判断并且调用填充 填充协程
    /// </summary>
    public IEnumerator Fill()
    {
        //是否需要重新填充
        bool needRefill = true;
        //正在填充
        isFilling  = true; 


        while (needRefill)
        {

            yield return new WaitForSeconds(fillTime+0.5f);

            //一直填充到不能填充为止
            while (FillStep())
            {
                // inverse 变量控制横向遍历顺序（正序或逆序），模拟玩家视角的交替填充效果。
                inverse = !inverse;
                //填充等待
                yield return new WaitForSeconds(fillTime);
            }

            //清除所有三个及以上的相同格子对象 如果发生清除就 清除以后重新填充格子
            needRefill = ClearAllValidMatches();
        }
        //填充完成
        isFilling = false;




    }
    /// <summary>
    /// 具体填充逻辑
    /// </summary>
    public bool FillStep()
    {
        //默认下次不能填充
        movedPiece = false;

        //第一次循环：遍历网格数组每一个格子对象 看是否能够下落
        //我们按照Y开始遍历 即一行一行 自下而上 并且从倒数第二行开始 因为最后一行不会下落 
        //我们下标从零开始 就是 0 - 8 倒数第二行就是 yDim -2
        for (int y = yDim - 2;y >= 0; y--)
        {
            for(int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                // inverse 变量控制横向遍历顺序（正序或逆序），模拟玩家视角的交替填充效果。
                if (inverse) 
                {
                    x = xDim - 1 - loopX;
                }

                //获取当前格子对象
                GamePiece piece = pieces[x,y];

                //判断当前格子对象能不能移动
                if (piece.IsMovable())//能移动说明该格子对象不是空的
                {

                    //看该位置的下一行的对应位置 是不是空格子对象 是就现在的格子对象下落
                    //取下一行的该位置
                    GamePiece latePiece = pieces[x, y + 1];
                    //下方为空直接填充
                    if (latePiece.Type == PieceType.Empty)
                    {
                        //删除空位置游戏格子对象
                        Destroy(latePiece.gameObject);

                        //下落 移动到 下一个的空位置
                        piece.MovableCompinent.Move(x, y+1,fillTime);
                        //更新数组
                        pieces[x, y + 1] = piece;
                        //把当前位置置空
                        SpawnNewPiece(x,y,PieceType.Empty);
                        //下一次可以继续调用填充
                        movedPiece = true;

                    }
                    else//下方不为空 判断该位置是否能够左右下落填充
                    {
                        for(int diag = -1;diag <= 1; diag++)
                        {
                            //尝试往左下角或者右下角下落 。中间=0时不用管 =0的情况是该位置下面为空位置的情况 显然这种情况不正确，因为如果是就走上面的if直接下落了
                            if(diag != 0)
                            {
                                //开始时尝试往左下落 再往右下落
                                int diagX = x + diag;

                                if (inverse) //如果是方向遍历 那就//开始时尝试往右下落 再往左下落
                                {
                                    diagX = x - diag;
                                }

                                if(diagX >= 0 &&  diagX < xDim) //不能超过网格x左右下方位置不能超过网格范围
                                {
                                    //获取左下方 或者 右下方的位置格子对象
                                    GamePiece diagonalPiece  = pieces[diagX, y + 1]; 

                                    if(diagonalPiece.Type == PieceType.Empty)//左下方 或者 右下方的位置格子对象为空
                                    {
                                        //是否能移动到左下方或者右下方
                                        bool hasPieceAbove = true; //开始时默认不可以移动到该位置

                                        for(int aboveY = y;aboveY >= 0;aboveY--)
                                        {
                                            //左下方或者右下方位置的格子对象 的上面格子对象
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            ////左下角或者右下角位置的格子对象 上面是其他正常格子对象 不能下落 
                                            ///因为这个位置是 还没来得及下落的格子对象下落的位置 
                                            ///我们遍历是按行一个个遍历的左右下方下落判断可能遇到旁边还没来得及下落的位置
                                            if (pieceAbove.IsMovable()) 
                                            {
                                                break;
                                            }else if(!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.Empty)//该左下角或者右下角上面是障碍物 可以下落到该位置
                                            {
                                                //可以移动到该位置
                                                hasPieceAbove = false;
                                                break;
                                            }

                                        }

                                        //可以移动到左下角或者右下角
                                        //执行一次就break了 不会判断另一边左下角或者右下角了
                                        if (hasPieceAbove == false)
                                        {
                                            Destroy(diagonalPiece.gameObject);//消除左下角或者右下角的空对象
                                            piece.MovableCompinent.Move(diagX, y + 1, fillTime);//当前位置移动到左下角或者右下角位置
                                            pieces[diagX, y + 1] = piece;//重新覆盖网格中的格子对象信息
                                            SpawnNewPiece(x, y, PieceType.Empty);//当前位置变成空
                                            movedPiece = true;//可以继续进行下一次填充网格
                                            break;//结束循环当前左右下角的移动循环 继续遍历当前行的下一个位置

                                        }

                                    }

                                }


                            }




                        }


                    }

                }

            }
        }

        //第二次循环：在第一行创建非空的 格子对象 不用每一行创建 因为创建完成以后 下一次调用填充会直接下落到下一行
        for(int x = 0;x < xDim; x++)
        {

            //先看第一行的每一个元素是否是空 有空的位置才能创建
            GamePiece pieceBelow = pieces[x, 0];

            if(pieces[x, 0].Type == PieceType.Empty)
            {
                //删除空位置游戏格子对象
                Destroy(pieceBelow.gameObject);

                //创建格子游戏对象
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.Normal], GetWorldPosition(x, -1), Quaternion.identity);
                //设置父对象
                newPiece.transform.parent = transform;
     
                //保存格子对象脚本
                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                //初始化格子对象脚本中的数据
                pieces[x, 0].Init(x, -1, PieceType.Normal, this);

                //移动格子对象
                pieces[x, 0].MovableCompinent.Move(x, 0, fillTime);
                
                //设置格子对象颜色(种类) 
                //用随机数 随机生成一个格子颜色 来给格子对象设置颜色
                //颜色类型是枚举的 所以可以把整形 变成相应的枚举
                //根据枚举的颜色类型 来设置颜色
                ColorPiece.ColorType colorType = (ColorPiece.ColorType)UnityEngine.Random.Range(0, pieces[x, 0].ColorCompinent.NumColors());
                pieces[x, 0].ColorCompinent.SetColor(colorType);
                //下次可以继续填充
                movedPiece = true;
            }


        }



        //判断下一次能不能继续填充
        return movedPiece;

    }

    #endregion

    #region 操作格子对象相关
    /// <summary>
    /// 格子对象坐标调整 变成世界坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x + x * xoffset
            , transform.position.y + yDim / 2.0f - y - y * yoffset);
    }

    /// <summary>
    /// 格子对象创建函数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="pieceType"></param>
    /// <returns></returns>
    public GamePiece SpawnNewPiece(int x,int y,PieceType pieceType)
    {
        //创建格子游戏对象
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[pieceType],GetWorldPosition(x,y), Quaternion.identity);
        //设置名字
        newPiece.name = "Piece(" + x + ", " + y + ")";
        //设置父对象
        newPiece.transform.parent = transform;

        //保存格子对象脚本
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        //初始化格子对象脚本中的数据
        pieces[x, y].Init(x, y, pieceType, this);

        return pieces[x, y];

    }

    /// <summary>
    /// 判断两个格子对象是否左右或者上下相邻
    /// </summary>
    /// <param name="piece1"></param>
    /// <param name="piece2"></param>
    /// <returns></returns>
    public bool IsAdjacent(GamePiece piece1,GamePiece piece2)
    {
        //这里的XY不是游戏中的Position坐标,是网格数组里面的位置坐标 所以记录的基本信息XY相邻的换相减就是1
        return (piece1.X == piece2.X && (int)Math.Abs(piece1.Y - piece2.Y) == 1) ||
            (piece1.Y == piece2.Y && (int)Math.Abs(piece1.X - piece2.X) == 1);
    }
    /// <summary>
    /// 交换两个格子对象
    /// </summary>
    /// <param name="piece1"></param>
    /// <param name="piece2"></param>
    public void SwapPiece(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver)
        {
            return;
        }

        //交换网格数组中的位置
        pieces[piece1.X, piece1.Y] = piece2;
        pieces[piece2.X, piece2.Y] = piece1;

        //判断能不能交换 只要两个其中一个 交换以后能够有三个相同元素 才可以交换 或者交换的是 相同颜色消除的特殊格子对象
        if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
            || piece1.Type == PieceType.Rainbow || piece2.Type == PieceType.Rainbow
            )
        {
            //交换格子对象游戏位置
            int pressX = piece1.X;
            int pressY = piece1.Y;

            piece1.MovableCompinent.Move(piece2.X, piece2.Y, fillTime);
            piece2.MovableCompinent.Move(pressX, pressY, fillTime);

            //如果交换格子对象是 消除相同颜色的特殊格子对象
            if(piece1.Type == PieceType.Rainbow && piece1.IsClearable())
            {
                //获取消除脚本
                ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                //设置要消除的颜色
                if(clearColor != null)
                {
                    clearColor.Color = piece2.ColorCompinent.Color;
                }
                //调用消除函数
                ClearPiece(piece1.X, piece1.Y);
            }

            //如果交换格子对象是 消除相同颜色的特殊格子对象
            if (piece2.Type == PieceType.Rainbow && piece2.IsClearable())
            {
                //获取消除脚本
                ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

                //设置要消除的颜色
                if (clearColor != null)
                {
                    clearColor.Color = piece1.ColorCompinent.Color;
                }
                //调用消除函数
                ClearPiece(piece2.X, piece2.Y);
            }

            //清除三个及以上的相同格子对象
            ClearAllValidMatches();

            //交换的格子对象是特殊格子对象 不管该特殊格子对象能不能被匹配消除 我们都消除 并且
            //是按行消除 或者按列消除
            ////即： 只要交换成功特殊格子对象 就触发行或列消除
            if (piece1.Type == PieceType.Row_Clear || piece1.Type == PieceType.Column_Clear)
            {
                ClearPiece(piece1.X, piece1.Y);
            }

            if (piece2.Type == PieceType.Row_Clear || piece2.Type == PieceType.Column_Clear)
            {
                ClearPiece(piece2.X, piece2.Y);
            }

            //交换以后这两个格子对象变成空
            pressPiece = null;
            enterPiece = null;

            //看是否填充 是就填充
            StartCoroutine(Fill());

            //关卡的格子对象移动函数调用
            level.OnMove();

        }
        else //禁止交换
        {
            //交换网格数组中的位置
            pieces[piece1.X, piece1.Y] = piece1;
            pieces[piece2.X, piece2.Y] = piece2;
        }

  


    }
    /// <summary>
    /// 按下鼠标时调用的方法 记录获取被按下的格子对象
    /// </summary>
    /// <param name="piece"></param>
    public void PressPiece(GamePiece piece)
    {
        pressPiece = piece;
    }
    /// <summary>
    /// 记录要被交换的格子对象
    /// </summary>
    /// <param name="piece"></param>
    public void EnterPiece(GamePiece piece)
    {
        enterPiece = piece;
    }
    /// <summary>
    /// 松开鼠标时调用的方法
    /// </summary>
    public void ReleasePiece()
    {
        //判断按下和需要交换的格子对象是否相邻
        if(IsAdjacent(pressPiece, enterPiece)) //相邻就交换位置
        {
            SwapPiece(pressPiece, enterPiece);
        }


    }

    #endregion

    #region 匹配操作

    /// <summary>
    /// 直线匹配格子对象 ：检查两个格子对象交换以后其中有一方是不是三个或者三个以上元素 是就可以交换两个格子对象 不是就不可以交换
    /// </summary>
    /// <param name="piece">交换的格子对象 匹配遍历的起点</param>
    /// <param name="newX">交换以后的位置X</param>
    /// <param name="newY">交换以后的位置Y</param>
    /// <returns></returns>
    public List<GamePiece> GetMatch(GamePiece piece,int newX,int newY)
    {
        //判断传入的对象是不是正常格子对象
        if (piece.IsColored())
        {
            //记录当前起点格子对象的颜色
            ColorPiece.ColorType color = piece.ColorCompinent.Color;

            //存储搜索水平方向的格子对象
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            //存储搜索垂直方向的格子对象
            List<GamePiece> verticalPieces = new List<GamePiece>();
            //存储可以被三消的格子对象
            List<GamePiece> matchPieces = new List<GamePiece>();



            //开始从当前格子对象为起点 水平左右搜索其他格子对象

            horizontalPieces.Add(piece);//先把起点加入进去

            //开始水平左右搜索
            for (int dir = 0;dir <= 1; dir++) //0 1代表水平搜索的方向 搜索两次 第一次左搜索 第二次右搜索
            {
                for(int xOffset = 1;xOffset < xDim; xOffset++) //开始左或者右遍历同行的格子对象 直到遇到与当前格子对象颜色不相同就结束
                {
                    //xOffset 是起点格子对象的偏移位置 即其他格子对象的位置
                    int x;//同行当前遍历到的格子对象X位置 

                    if(dir == 0) //左搜索
                    {
                        x = newX - xOffset; //起点位置 - 偏移位置
                    }
                    else//右搜索
                    {
                        x = newX + xOffset;//起点位置 +偏移位置
                    }

                    if(x < 0 || x >= xDim) //X 不能越界
                    {
                        break;
                    }
                    //水平顺序遍历到的格子对象是正常格对象 并且与起点格子对象相同颜色就加入集合 代表该格子对象相同
                    if (pieces[x,newY].IsColored() && pieces[x,newY].ColorCompinent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else //不是就直接结束该方向的水平搜索
                    {
                        break;
                    }

                }

                //一次水平方向搜索完成以后看 水平搜索集合 里面的元素是不是大于三个 是就把这些格子对象 加入待消除集合
                if(horizontalPieces.Count >= 3)
                {
                    for(int i = 0;i< horizontalPieces.Count;i++)
                    {
                        matchPieces.Add(horizontalPieces[i]);
                    }

                    //L型匹配
                    LTypeMatch(horizontalPieces, verticalPieces, matchPieces, newX, newY, color);
                }


                if(matchPieces.Count >= 3)//待消除集合中有三个以上格子对象 说明此次交换两个格子对象的操作可以进行 直接交换 并且返回 待消除集合 不用继续另一个方向或者垂直方向搜索了
                {
                    return matchPieces;
                }

            }


            //垂直搜索匹配
            horizontalPieces.Clear();
            verticalPieces.Clear();
   
            verticalPieces.Add(piece);//先把起点加入进去


            //开始垂直上下搜索
            for (int dir = 0; dir <= 1; dir++) //0 1代表垂直搜索的方向 搜索两次 第一次上搜索 第二次下搜索
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++) //开始上或者下遍历同行的格子对象 直到遇到与当前格子对象颜色不相同就结束
                {
                    //yOffset 是起点格子对象的偏移位置 即其他格子对象的位置
                    int y;//同行当前遍历到的格子对象y位置 

                    if (dir == 0) //上搜索
                    {
                        y = newY - yOffset; //起点位置 - 偏移位置
                    }
                    else//下搜索
                    {
                        y = newY + yOffset;//起点位置 +偏移位置
                    }

                    if (y < 0 || y >= yDim) //y 不能越界
                    {
                        break;
                    }
                    //上下顺序遍历到的格子对象是正常格对象 并且与起点格子对象相同颜色就加入集合 代表该格子对象相同
                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorCompinent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else //不是就直接结束该方向的垂直搜索
                    {
                        break;
                    }

                }

                //一次上或者下方向搜索完成以后看 垂直搜索集合 里面的元素是不是大于三个 是就把这些格子对象 加入待消除集合
                if (verticalPieces.Count >= 3)
                {
                    for (int i = 0; i < verticalPieces.Count; i++)
                    {
                        matchPieces.Add(verticalPieces[i]);
                    }

                    //L型匹配
                    LTypeMatch(horizontalPieces, verticalPieces, matchPieces, newX, newY, color);
                }

                if (matchPieces.Count >= 3)//待消除集合中有三个以上格子对象 说明此次交换两个格子对象的操作可以进行 直接交换 并且返回 待消除集合 不用继续另一个方向搜索了
                {
                    return matchPieces;
                }

            }
            //默认无三个相同元素相邻
            return null;
        }

        return null;

    }

    /// <summary>
    /// L型匹配 ：列如直线匹配成功以后 在该直线方向看有没有其他三个相连接的格子对象
    /// 水平成功匹配以后 在每一个水平格子对象上面继续进行垂直上下搜索操作把该格子的三个相连接的格子对象继续加入待消除集合
    /// 或者垂直成功匹配以后 在每一个垂直格子对象上面继续进行水平左右搜索操作把该格子的三个相连接的格子对象继续加入待消除集合
    /// </summary>
    /// <param name="horizontalPieces"></param>
    /// <param name="verticalPieces"></param>
    /// <param name="matchPieces"></param>
    public void LTypeMatch(List<GamePiece> horizontalPieces,List<GamePiece> verticalPieces, List<GamePiece> matchPieces,int newX,int newY,ColorPiece.ColorType color)
    {
        //水平匹配成功时找L型
        if (horizontalPieces.Count >= 3)
        {
            for (int i = 0; i < horizontalPieces.Count; i++)
            {
                //从匹配好的直线格子对象集合的每一个格子对象开始 进行垂直搜索操作

                //开始垂直上下搜索
                for (int dir = 0; dir <= 1; dir++) //0 1代表垂直搜索的方向 搜索两次 第一次上搜索 第二次下搜索
                {
                    for (int yOffset = 1; yOffset < yDim; yOffset++) //开始上或者下遍历同行的格子对象 直到遇到与当前格子对象颜色不相同就结束
                    {
                        //yOffset 是起点格子对象的偏移位置 即其他格子对象的位置
                        int y;//同行当前遍历到的格子对象y位置 

                        if (dir == 0) //上搜索
                        {
                            y = newY - yOffset; //起点位置 - 偏移位置
                        }
                        else//下搜索
                        {
                            y = newY + yOffset;//起点位置 +偏移位置
                        }

                        if (y < 0 || y >= yDim) //y 不能越界
                        {
                            break;
                        }
                        //上下顺序遍历到的格子对象是正常格对象 并且与起点格子对象相同颜色就加入集合 代表该格子对象相同
                        if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorCompinent.Color == color)
                        {
                            verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                        }
                        else //不是就直接结束该方向的垂直搜索
                        {
                            break;
                        }

                    }


                }
                //搜索一次发现该格子对象的垂直方向不够两个 就清除垂直集合中的元素
                if (verticalPieces.Count < 2)
                {
                    verticalPieces.Clear();
                }
                else //如果够就加入到待消除集合 然后直接结束 不需要继续找了
                {
                    for (int k = 0; k < verticalPieces.Count; k++)
                    {
                        matchPieces.Add(verticalPieces[k]);
                    }

                    break;
                }


            }
            return;
        }

        //垂直匹配成功时找L型

        if(verticalPieces.Count > 3)
        {
            for(int i =  0; i < verticalPieces.Count; i++)
            {

                //开始水平左右搜索
                for (int dir = 0; dir <= 1; dir++) //0 1代表水平搜索的方向 搜索两次 第一次左搜索 第二次右搜索
                {
                    for (int xOffset = 1; xOffset < xDim; xOffset++) //开始左或者右遍历同行的格子对象 直到遇到与当前格子对象颜色不相同就结束
                    {
                        //xOffset 是起点格子对象的偏移位置 即其他格子对象的位置
                        int x;//同行当前遍历到的格子对象X位置 

                        if (dir == 0) //左搜索
                        {
                            x = newX - xOffset; //起点位置 - 偏移位置
                        }
                        else//右搜索
                        {
                            x = newX + xOffset;//起点位置 +偏移位置
                        }

                        if (x < 0 || x >= xDim) //X 不能越界
                        {
                            break;
                        }
                        //水平顺序遍历到的格子对象是正常格对象 并且与起点格子对象相同颜色就加入集合 代表该格子对象相同
                        if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorCompinent.Color == color)
                        {
                            horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                        }
                        else //不是就直接结束该方向的水平搜索
                        {
                            break;
                        }

                    }

                }

                //搜索一次发现该格子对象的水平方向不够两个 就清除水平集合中的元素
                if (horizontalPieces.Count < 2)
                {
                    horizontalPieces.Clear();
                }
                else //如果够就加入到待消除集合 然后直接结束 不需要继续找了
                {
                    for (int k = 0; k < horizontalPieces.Count; k++)
                    {
                        matchPieces.Add(horizontalPieces[k]);
                    }

                    break;
                }

            }


        }


    }


    #endregion

    #region 清除操作
    /// <summary>
    /// 遍历所有格子对象 并且以每一个格子对象进行匹配操作 如果匹配成功就清除
    /// </summary>
    /// <returns></returns>

    public bool ClearAllValidMatches()
    {
        //是否需要重新填充
        bool needRefill = false;

        for(int y = 0;y < yDim;y++)//按行遍历每一个格子对象
        {

            for(int x = 0;x < xDim;x++)
            {
                if (pieces[x,y].IsClearable()) //是否是可清除的格子对象
                {
                    //以当前格子对象为起点匹配格子对象
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);

                    if(match != null)//匹配成功开启三消
                    {
                        GameMusicManager.Instance.PlayClearMusic();

                        //消除一次成功以后 生成一个特殊类型（必须一下消除4个才能生成特殊格子对象）
                        //默认不能生成特殊格子对象类型
                        PieceType specialPieceType = PieceType.Count;
                        //在消除集合里面 随机选择一个被消除元素 用它的位置来生成特殊格子对象
                        GamePiece randomPiece = match[UnityEngine.Random.Range(0, match.Count)];
                        //特殊格子对象生成的位置
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;

                        //必须一下消除4个才能生成特殊格子对象
                        if(match.Count == 4)
                        {
                            //如果是自然填充消除 时那我们就随机生成一个特殊格子对象行或者列的格子对象
                            if(pressPiece == null || enterPiece == null)
                            {
                                specialPieceType = (PieceType)UnityEngine.Random.Range((int)PieceType.Row_Clear, (int)PieceType.Column_Clear);
                            }
                            else if(pressPiece.Y == enterPiece.Y)//玩家交换发生的清除 并且是水平清除这些元素 就生成行特殊格子对象
                            {
                                specialPieceType = PieceType.Row_Clear;
                            }
                            else//玩家交换发生的清除 并且是垂直清除这些元素 就生成列特殊格子对象
                            {
                                specialPieceType = PieceType.Column_Clear;
                            }

                        }else if(match.Count == 5) //生成消除相同颜色的特殊格子对象
                        {
                            specialPieceType = PieceType.Rainbow;
                        }


                        for (int i = 0; i < match.Count; i++)
                        {
                            //有清除脚本的格子对象 并且 无正在清除状态的格子对象才可以清除
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needRefill = true;

                                //如果是交换产生的那就不用随机 特殊格子对象位置 用交换的那个位置就行
                                //清除还能访问这个位置 是因为 Destor函数是下一帧调用 我们还能访问到当前格子对象数据
                                if (match[i] == pressPiece || match[i] == enterPiece)
                                {
                                    //特殊格子对象的位置更新
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }

                            }
                        }

                        //如果清除完成以后 该特殊格子对象类型标识变了 说明可以发生生成特殊格子对象
                        if(specialPieceType != PieceType.Count)
                        {
                            //删除特殊格子对象位置上的格子对象
                            Destroy(pieces[specialPieceX,specialPieceY]);
                            //生成格子特殊对象
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY,specialPieceType);



                            //特殊格子对象 并且生成的特殊格子对象能设置颜色 并且我们清除时的格子对象可以设置颜色
                            if((specialPieceType == PieceType.Row_Clear || specialPieceType == PieceType.Column_Clear)
                                && newPiece.IsColored() && match[0].IsColored())
                            {

                                //我们把生成的特殊格子对象 颜色(种类) 变成和 刚刚消除的 正常格子对象一样就行
                                newPiece.ColorCompinent.SetColor(match[0].ColorCompinent.Color);

                            }
                            else if(specialPieceType == PieceType.Rainbow && newPiece.IsClearable()) //相同颜色消除的特殊格子对象
                            {
                                newPiece.ColorCompinent.SetColor(ColorPiece.ColorType.Any);
                            }
                   

                        }

                    }
                }
            }


        }

        return needRefill;  

    }

    /// <summary>
    /// 清除格子执行函数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool ClearPiece(int x,int y)
    {
        //判断是否可以清除 并且不是正在清除

        if (pieces[x,y].IsColored() && pieces[x,y].ClearableCompinent.IsBeginCleared == false)
        {
            //调用清除脚本中的清除
            pieces[x,y].ClearableCompinent.Clear();
            //该位置变成空
            SpawnNewPiece(x, y, PieceType.Empty);

            //判断该清除的格子对象上下左右有没有障碍物 有就清除
            ClearObstacles(x, y);

            return true;
        }

        return false;
    }

    /// <summary>
    /// 消除格子对象时搜索 周围是否有障碍物 有就消除
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ClearObstacles(int x,int y)
    {
        //当前被消除的格子对象 循环遍历左右一格的格子对象看是不是障碍物
        for(int adjacentX = x -1;adjacentX <= x + 1;adjacentX++) 
        {
            //不能越界
            if(adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                //遍历的格子是障碍物 并且可以被消除
                if (pieces[adjacentX,y].Type == PieceType.Bubble && pieces[adjacentX, y].IsClearable())
                {
                    //消除障碍物
                    pieces[adjacentX, y].ClearableCompinent.Clear();
                    //生成空格子在障碍物位置
                    SpawnNewPiece(adjacentX, y, PieceType.Empty);
                }
            }
        }
        //当前被消除的格子对象 循环遍历上下一格的格子对象看是不是障碍物
        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            //不能越界
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                //遍历的格子是障碍物 并且可以被消除
                if (pieces[x, adjacentY].Type == PieceType.Bubble && pieces[x, adjacentY].IsClearable())
                {
                    //消除障碍物
                    pieces[x, adjacentY].ClearableCompinent.Clear();
                    //生成空格子在障碍物位置
                    SpawnNewPiece(x, adjacentY, PieceType.Empty);
                }
            }
        }


    }

    /// <summary>
    /// 清除整行格子对象
    /// </summary>
    /// <param name="row"></param>
   public void ClearRow(int row)
    {
        for(int x = 0;x < xDim; x++)
        {
            ClearPiece(x, row);
        }

    }
    /// <summary>
    /// 清除整列格子对象
    /// </summary>
    /// <param name="column"></param>
    public void ClearColumn(int column) 
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y);
        }

    }

    /// <summary>
    /// 清除指定相同颜色种类的格子对象
    /// </summary>
    public void ClearColorPiece(ColorPiece.ColorType color)
    {
        for(int x = 0;x < xDim; x++)
        {

            for(int  y = 0;y < yDim; y++)
            {

                if (pieces[x,y].IsColored() && pieces[x, y].ColorCompinent.Color == color
                    || color == ColorPiece.ColorType.Any)
                {
                    ClearPiece(x, y);

                }


            }

        }



    }


    #endregion

    #region 关卡相关操作




    /// <summary>
    /// 游戏结束调用
    /// </summary>
    public void GameOver()
    {
        gameOver = true;
    }
    /// <summary>
    /// 查找网格里面有多少个 传入的类型 的格子对象 就是格子里面有多少个障碍物
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<GamePiece> GetPieceOfType(PieceType type)
    {
        List<GamePiece> pieceOfType = new List<GamePiece>();

        for(int x = 0; x < xDim; x++)
        {

            for(int y =0; y < yDim; y++)
            {
                if(pieces[x,y].Type == type)
                {
                    pieceOfType.Add(pieces[x,y]);
                }

            }

        }
        return pieceOfType;


    }



    #endregion

    #endregion


}
