using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using static Grid;
/// <summary>
/// ������
/// </summary>
public class Grid : MonoBehaviour
{
    public float xoffset;
    public float yoffset;

    /// <summary>
    /// ��������
    /// </summary>
    public enum PieceType
    {
        Empty,//�ո��Ӷ���
        Normal,//�������Ӷ���
        Count,
        Bubble,//�ϰ���
        Row_Clear,//�����������Ӷ���
        Column_Clear,//��������Ӷ���
        Rainbow//���������ͬ��ɫ�ĸ��Ӷ���

    };
    /// <summary>
    /// ���ӽṹ�� �� �洢���Ӷ�������
    /// </summary>
    [Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }


    //���ӳ���
    public int xDim;
    public int yDim;

    /// <summary>
    /// ���ӽṹ������ �ѽṹ�������ת�� ���Ӷ����ֵ��õ� һ��ʼҪ�ṹ������Ϊ�������ü����л�
    /// </summary>
    public PiecePrefab[] piecePrefabs;
    /// <summary>
    /// ���Ӷ���ı�����
    /// </summary>
    public GameObject backgroundPrefab;

    /// <summary>
    /// ���Ӷ����ֵ� �洢���и������͵Ķ���
    /// </summary>
    private Dictionary<PieceType, GameObject> piecePrefabDict;

    /// <summary>
    /// ���ӵ���Ϸ����ű� ����������Ͳ�������ʵ���������ĸ��Ӷ�����Ϸ����
    /// </summary>
    private GamePiece[,] pieces;


    /// <summary>
    /// �ж����һ���Ժ��Ƿ��ܹ��������
    /// </summary>
    private bool movedPiece;
    /// <summary>
    /// �����ʱ��
    /// </summary>
    public float fillTime;
    /// <summary>
    ///  inverse �������ƺ������˳����������򣩣�ģ������ӽǵĽ������Ч����
    /// </summary>
    private bool inverse;

    /// <summary>
    /// ��ǰ����갴�µĸ��Ӷ���
    /// </summary>
    private GamePiece pressPiece;
    /// <summary>
    /// Ҫ�������ĸ��Ӷ���
    /// </summary>
    private GamePiece enterPiece;
    /// <summary>
    /// �ؿ���
    /// </summary>
    public Level level;
    /// <summary>
    /// ��Ϸ�����ж�
    /// </summary>
    private bool gameOver;

    /// <summary>
    /// ������� �� �ϰ�����Ӷ��� 
    /// </summary>
    [Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;

    }
    /// <summary>
    /// ���� ������� �� �ϰ�����Ӷ��� ��������
    /// </summary>
    public PiecePosition[] initialPieces;

    /// <summary>
    /// �Ƿ��������
    /// </summary>
    private bool isFilling = false;
    public bool IsFilling {  get { return isFilling; } }


    // Start is called before the first frame update
    void Awake()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        //�Ѹ��ӽṹ����������� ת���ֵ�
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (piecePrefabDict.ContainsKey(piecePrefabs[i].type) == false)
            {
                //��ͬ���͸��Ӷ�������ֵ�
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }

        }

        //����ÿһ�����ӵı�����
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                //���ɵ������ӱ���
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                //���ø�����
                background.transform.parent = transform;
            }
        }

        //��ʼ�����Ӷ���ű����� 
        pieces = new GamePiece[xDim, yDim];


        //�����ϰ���
        for(int i = 0 ; i < initialPieces.Length; i++)
        {

            //�ϰ��������е��ϰ���λ��Ҫ��ȷ
            if (initialPieces[i].x >= 0 && initialPieces[i].x < xDim && initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                //�����ϰ���
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);

            }
        }



        //����ÿһ��������Ϸ���� Ĭ�������ɿյĸ�����Ϸ����
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                //���ϰ���ĵط��������ɿո��Ӷ���
                if (pieces[x, y] == null)
                {
                    //���ɲ��ұ��浥��������Ϸ���� Ĭ�������ɿյĸ�����Ϸ����
                    SpawnNewPiece(x, y, PieceType.Empty);
                }

            }
        }

        


        //�����Ϸ���Ӷ���
        StartCoroutine(Fill());

    }



    // Update is called once per frame
    void Update()
    {

    }

    #region ���ĺ���

    #region �����ƶ�������Ӷ���
    /// <summary>
    /// �жϲ��ҵ������ ���Э��
    /// </summary>
    public IEnumerator Fill()
    {
        //�Ƿ���Ҫ�������
        bool needRefill = true;
        //�������
        isFilling  = true; 


        while (needRefill)
        {

            yield return new WaitForSeconds(fillTime+0.5f);

            //һֱ��䵽�������Ϊֹ
            while (FillStep())
            {
                // inverse �������ƺ������˳����������򣩣�ģ������ӽǵĽ������Ч����
                inverse = !inverse;
                //���ȴ�
                yield return new WaitForSeconds(fillTime);
            }

            //����������������ϵ���ͬ���Ӷ��� ������������ ����Ժ�����������
            needRefill = ClearAllValidMatches();
        }
        //������
        isFilling = false;




    }
    /// <summary>
    /// ��������߼�
    /// </summary>
    public bool FillStep()
    {
        //Ĭ���´β������
        movedPiece = false;

        //��һ��ѭ����������������ÿһ�����Ӷ��� ���Ƿ��ܹ�����
        //���ǰ���Y��ʼ���� ��һ��һ�� ���¶��� ���Ҵӵ����ڶ��п�ʼ ��Ϊ���һ�в������� 
        //�����±���㿪ʼ ���� 0 - 8 �����ڶ��о��� yDim -2
        for (int y = yDim - 2;y >= 0; y--)
        {
            for(int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                // inverse �������ƺ������˳����������򣩣�ģ������ӽǵĽ������Ч����
                if (inverse) 
                {
                    x = xDim - 1 - loopX;
                }

                //��ȡ��ǰ���Ӷ���
                GamePiece piece = pieces[x,y];

                //�жϵ�ǰ���Ӷ����ܲ����ƶ�
                if (piece.IsMovable())//���ƶ�˵���ø��Ӷ����ǿյ�
                {

                    //����λ�õ���һ�еĶ�Ӧλ�� �ǲ��ǿո��Ӷ��� �Ǿ����ڵĸ��Ӷ�������
                    //ȡ��һ�еĸ�λ��
                    GamePiece latePiece = pieces[x, y + 1];
                    //�·�Ϊ��ֱ�����
                    if (latePiece.Type == PieceType.Empty)
                    {
                        //ɾ����λ����Ϸ���Ӷ���
                        Destroy(latePiece.gameObject);

                        //���� �ƶ��� ��һ���Ŀ�λ��
                        piece.MovableCompinent.Move(x, y+1,fillTime);
                        //��������
                        pieces[x, y + 1] = piece;
                        //�ѵ�ǰλ���ÿ�
                        SpawnNewPiece(x,y,PieceType.Empty);
                        //��һ�ο��Լ����������
                        movedPiece = true;

                    }
                    else//�·���Ϊ�� �жϸ�λ���Ƿ��ܹ������������
                    {
                        for(int diag = -1;diag <= 1; diag++)
                        {
                            //���������½ǻ������½����� ���м�=0ʱ���ù� =0������Ǹ�λ������Ϊ��λ�õ���� ��Ȼ�����������ȷ����Ϊ����Ǿ��������ifֱ��������
                            if(diag != 0)
                            {
                                //��ʼʱ������������ ����������
                                int diagX = x + diag;

                                if (inverse) //����Ƿ������ �Ǿ�//��ʼʱ������������ ����������
                                {
                                    diagX = x - diag;
                                }

                                if(diagX >= 0 &&  diagX < xDim) //���ܳ�������x�����·�λ�ò��ܳ�������Χ
                                {
                                    //��ȡ���·� ���� ���·���λ�ø��Ӷ���
                                    GamePiece diagonalPiece  = pieces[diagX, y + 1]; 

                                    if(diagonalPiece.Type == PieceType.Empty)//���·� ���� ���·���λ�ø��Ӷ���Ϊ��
                                    {
                                        //�Ƿ����ƶ������·��������·�
                                        bool hasPieceAbove = true; //��ʼʱĬ�ϲ������ƶ�����λ��

                                        for(int aboveY = y;aboveY >= 0;aboveY--)
                                        {
                                            //���·��������·�λ�õĸ��Ӷ��� ��������Ӷ���
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            ////���½ǻ������½�λ�õĸ��Ӷ��� �����������������Ӷ��� �������� 
                                            ///��Ϊ���λ���� ��û���ü�����ĸ��Ӷ��������λ�� 
                                            ///���Ǳ����ǰ���һ���������������·������жϿ��������Ա߻�û���ü������λ��
                                            if (pieceAbove.IsMovable()) 
                                            {
                                                break;
                                            }else if(!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.Empty)//�����½ǻ������½��������ϰ��� �������䵽��λ��
                                            {
                                                //�����ƶ�����λ��
                                                hasPieceAbove = false;
                                                break;
                                            }

                                        }

                                        //�����ƶ������½ǻ������½�
                                        //ִ��һ�ξ�break�� �����ж���һ�����½ǻ������½���
                                        if (hasPieceAbove == false)
                                        {
                                            Destroy(diagonalPiece.gameObject);//�������½ǻ������½ǵĿն���
                                            piece.MovableCompinent.Move(diagX, y + 1, fillTime);//��ǰλ���ƶ������½ǻ������½�λ��
                                            pieces[diagX, y + 1] = piece;//���¸��������еĸ��Ӷ�����Ϣ
                                            SpawnNewPiece(x, y, PieceType.Empty);//��ǰλ�ñ�ɿ�
                                            movedPiece = true;//���Լ���������һ���������
                                            break;//����ѭ����ǰ�����½ǵ��ƶ�ѭ�� ����������ǰ�е���һ��λ��

                                        }

                                    }

                                }


                            }




                        }


                    }

                }

            }
        }

        //�ڶ���ѭ�����ڵ�һ�д����ǿյ� ���Ӷ��� ����ÿһ�д��� ��Ϊ��������Ժ� ��һ�ε�������ֱ�����䵽��һ��
        for(int x = 0;x < xDim; x++)
        {

            //�ȿ���һ�е�ÿһ��Ԫ���Ƿ��ǿ� �пյ�λ�ò��ܴ���
            GamePiece pieceBelow = pieces[x, 0];

            if(pieces[x, 0].Type == PieceType.Empty)
            {
                //ɾ����λ����Ϸ���Ӷ���
                Destroy(pieceBelow.gameObject);

                //����������Ϸ����
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.Normal], GetWorldPosition(x, -1), Quaternion.identity);
                //���ø�����
                newPiece.transform.parent = transform;
     
                //������Ӷ���ű�
                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                //��ʼ�����Ӷ���ű��е�����
                pieces[x, 0].Init(x, -1, PieceType.Normal, this);

                //�ƶ����Ӷ���
                pieces[x, 0].MovableCompinent.Move(x, 0, fillTime);
                
                //���ø��Ӷ�����ɫ(����) 
                //������� �������һ��������ɫ �������Ӷ���������ɫ
                //��ɫ������ö�ٵ� ���Կ��԰����� �����Ӧ��ö��
                //����ö�ٵ���ɫ���� ��������ɫ
                ColorPiece.ColorType colorType = (ColorPiece.ColorType)UnityEngine.Random.Range(0, pieces[x, 0].ColorCompinent.NumColors());
                pieces[x, 0].ColorCompinent.SetColor(colorType);
                //�´ο��Լ������
                movedPiece = true;
            }


        }



        //�ж���һ���ܲ��ܼ������
        return movedPiece;

    }

    #endregion

    #region �������Ӷ������
    /// <summary>
    /// ���Ӷ���������� �����������
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
    /// ���Ӷ��󴴽�����
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="pieceType"></param>
    /// <returns></returns>
    public GamePiece SpawnNewPiece(int x,int y,PieceType pieceType)
    {
        //����������Ϸ����
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[pieceType],GetWorldPosition(x,y), Quaternion.identity);
        //��������
        newPiece.name = "Piece(" + x + ", " + y + ")";
        //���ø�����
        newPiece.transform.parent = transform;

        //������Ӷ���ű�
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        //��ʼ�����Ӷ���ű��е�����
        pieces[x, y].Init(x, y, pieceType, this);

        return pieces[x, y];

    }

    /// <summary>
    /// �ж��������Ӷ����Ƿ����һ�����������
    /// </summary>
    /// <param name="piece1"></param>
    /// <param name="piece2"></param>
    /// <returns></returns>
    public bool IsAdjacent(GamePiece piece1,GamePiece piece2)
    {
        //�����XY������Ϸ�е�Position����,���������������λ������ ���Լ�¼�Ļ�����ϢXY���ڵĻ��������1
        return (piece1.X == piece2.X && (int)Math.Abs(piece1.Y - piece2.Y) == 1) ||
            (piece1.Y == piece2.Y && (int)Math.Abs(piece1.X - piece2.X) == 1);
    }
    /// <summary>
    /// �����������Ӷ���
    /// </summary>
    /// <param name="piece1"></param>
    /// <param name="piece2"></param>
    public void SwapPiece(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver)
        {
            return;
        }

        //�������������е�λ��
        pieces[piece1.X, piece1.Y] = piece2;
        pieces[piece2.X, piece2.Y] = piece1;

        //�ж��ܲ��ܽ��� ֻҪ��������һ�� �����Ժ��ܹ���������ͬԪ�� �ſ��Խ��� ���߽������� ��ͬ��ɫ������������Ӷ���
        if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
            || piece1.Type == PieceType.Rainbow || piece2.Type == PieceType.Rainbow
            )
        {
            //�������Ӷ�����Ϸλ��
            int pressX = piece1.X;
            int pressY = piece1.Y;

            piece1.MovableCompinent.Move(piece2.X, piece2.Y, fillTime);
            piece2.MovableCompinent.Move(pressX, pressY, fillTime);

            //����������Ӷ����� ������ͬ��ɫ��������Ӷ���
            if(piece1.Type == PieceType.Rainbow && piece1.IsClearable())
            {
                //��ȡ�����ű�
                ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                //����Ҫ��������ɫ
                if(clearColor != null)
                {
                    clearColor.Color = piece2.ColorCompinent.Color;
                }
                //������������
                ClearPiece(piece1.X, piece1.Y);
            }

            //����������Ӷ����� ������ͬ��ɫ��������Ӷ���
            if (piece2.Type == PieceType.Rainbow && piece2.IsClearable())
            {
                //��ȡ�����ű�
                ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

                //����Ҫ��������ɫ
                if (clearColor != null)
                {
                    clearColor.Color = piece1.ColorCompinent.Color;
                }
                //������������
                ClearPiece(piece2.X, piece2.Y);
            }

            //������������ϵ���ͬ���Ӷ���
            ClearAllValidMatches();

            //�����ĸ��Ӷ�����������Ӷ��� ���ܸ�������Ӷ����ܲ��ܱ�ƥ������ ���Ƕ����� ����
            //�ǰ������� ���߰�������
            ////���� ֻҪ�����ɹ�������Ӷ��� �ʹ����л�������
            if (piece1.Type == PieceType.Row_Clear || piece1.Type == PieceType.Column_Clear)
            {
                ClearPiece(piece1.X, piece1.Y);
            }

            if (piece2.Type == PieceType.Row_Clear || piece2.Type == PieceType.Column_Clear)
            {
                ClearPiece(piece2.X, piece2.Y);
            }

            //�����Ժ����������Ӷ����ɿ�
            pressPiece = null;
            enterPiece = null;

            //���Ƿ���� �Ǿ����
            StartCoroutine(Fill());

            //�ؿ��ĸ��Ӷ����ƶ���������
            level.OnMove();

        }
        else //��ֹ����
        {
            //�������������е�λ��
            pieces[piece1.X, piece1.Y] = piece1;
            pieces[piece2.X, piece2.Y] = piece2;
        }

  


    }
    /// <summary>
    /// �������ʱ���õķ��� ��¼��ȡ�����µĸ��Ӷ���
    /// </summary>
    /// <param name="piece"></param>
    public void PressPiece(GamePiece piece)
    {
        pressPiece = piece;
    }
    /// <summary>
    /// ��¼Ҫ�������ĸ��Ӷ���
    /// </summary>
    /// <param name="piece"></param>
    public void EnterPiece(GamePiece piece)
    {
        enterPiece = piece;
    }
    /// <summary>
    /// �ɿ����ʱ���õķ���
    /// </summary>
    public void ReleasePiece()
    {
        //�жϰ��º���Ҫ�����ĸ��Ӷ����Ƿ�����
        if(IsAdjacent(pressPiece, enterPiece)) //���ھͽ���λ��
        {
            SwapPiece(pressPiece, enterPiece);
        }


    }

    #endregion

    #region ƥ�����

    /// <summary>
    /// ֱ��ƥ����Ӷ��� ������������Ӷ��󽻻��Ժ�������һ���ǲ�������������������Ԫ�� �ǾͿ��Խ����������Ӷ��� ���ǾͲ����Խ���
    /// </summary>
    /// <param name="piece">�����ĸ��Ӷ��� ƥ����������</param>
    /// <param name="newX">�����Ժ��λ��X</param>
    /// <param name="newY">�����Ժ��λ��Y</param>
    /// <returns></returns>
    public List<GamePiece> GetMatch(GamePiece piece,int newX,int newY)
    {
        //�жϴ���Ķ����ǲ����������Ӷ���
        if (piece.IsColored())
        {
            //��¼��ǰ�����Ӷ������ɫ
            ColorPiece.ColorType color = piece.ColorCompinent.Color;

            //�洢����ˮƽ����ĸ��Ӷ���
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            //�洢������ֱ����ĸ��Ӷ���
            List<GamePiece> verticalPieces = new List<GamePiece>();
            //�洢���Ա������ĸ��Ӷ���
            List<GamePiece> matchPieces = new List<GamePiece>();



            //��ʼ�ӵ�ǰ���Ӷ���Ϊ��� ˮƽ���������������Ӷ���

            horizontalPieces.Add(piece);//�Ȱ��������ȥ

            //��ʼˮƽ��������
            for (int dir = 0;dir <= 1; dir++) //0 1����ˮƽ�����ķ��� �������� ��һ�������� �ڶ���������
            {
                for(int xOffset = 1;xOffset < xDim; xOffset++) //��ʼ������ұ���ͬ�еĸ��Ӷ��� ֱ�������뵱ǰ���Ӷ�����ɫ����ͬ�ͽ���
                {
                    //xOffset �������Ӷ����ƫ��λ�� ���������Ӷ����λ��
                    int x;//ͬ�е�ǰ�������ĸ��Ӷ���Xλ�� 

                    if(dir == 0) //������
                    {
                        x = newX - xOffset; //���λ�� - ƫ��λ��
                    }
                    else//������
                    {
                        x = newX + xOffset;//���λ�� +ƫ��λ��
                    }

                    if(x < 0 || x >= xDim) //X ����Խ��
                    {
                        break;
                    }
                    //ˮƽ˳��������ĸ��Ӷ�������������� �����������Ӷ�����ͬ��ɫ�ͼ��뼯�� ����ø��Ӷ�����ͬ
                    if (pieces[x,newY].IsColored() && pieces[x,newY].ColorCompinent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else //���Ǿ�ֱ�ӽ����÷����ˮƽ����
                    {
                        break;
                    }

                }

                //һ��ˮƽ������������Ժ� ˮƽ�������� �����Ԫ���ǲ��Ǵ������� �ǾͰ���Щ���Ӷ��� �������������
                if(horizontalPieces.Count >= 3)
                {
                    for(int i = 0;i< horizontalPieces.Count;i++)
                    {
                        matchPieces.Add(horizontalPieces[i]);
                    }

                    //L��ƥ��
                    LTypeMatch(horizontalPieces, verticalPieces, matchPieces, newX, newY, color);
                }


                if(matchPieces.Count >= 3)//���������������������ϸ��Ӷ��� ˵���˴ν����������Ӷ���Ĳ������Խ��� ֱ�ӽ��� ���ҷ��� ���������� ���ü�����һ��������ߴ�ֱ����������
                {
                    return matchPieces;
                }

            }


            //��ֱ����ƥ��
            horizontalPieces.Clear();
            verticalPieces.Clear();
   
            verticalPieces.Add(piece);//�Ȱ��������ȥ


            //��ʼ��ֱ��������
            for (int dir = 0; dir <= 1; dir++) //0 1����ֱ�����ķ��� �������� ��һ�������� �ڶ���������
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++) //��ʼ�ϻ����±���ͬ�еĸ��Ӷ��� ֱ�������뵱ǰ���Ӷ�����ɫ����ͬ�ͽ���
                {
                    //yOffset �������Ӷ����ƫ��λ�� ���������Ӷ����λ��
                    int y;//ͬ�е�ǰ�������ĸ��Ӷ���yλ�� 

                    if (dir == 0) //������
                    {
                        y = newY - yOffset; //���λ�� - ƫ��λ��
                    }
                    else//������
                    {
                        y = newY + yOffset;//���λ�� +ƫ��λ��
                    }

                    if (y < 0 || y >= yDim) //y ����Խ��
                    {
                        break;
                    }
                    //����˳��������ĸ��Ӷ�������������� �����������Ӷ�����ͬ��ɫ�ͼ��뼯�� ����ø��Ӷ�����ͬ
                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorCompinent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else //���Ǿ�ֱ�ӽ����÷���Ĵ�ֱ����
                    {
                        break;
                    }

                }

                //һ���ϻ����·�����������Ժ� ��ֱ�������� �����Ԫ���ǲ��Ǵ������� �ǾͰ���Щ���Ӷ��� �������������
                if (verticalPieces.Count >= 3)
                {
                    for (int i = 0; i < verticalPieces.Count; i++)
                    {
                        matchPieces.Add(verticalPieces[i]);
                    }

                    //L��ƥ��
                    LTypeMatch(horizontalPieces, verticalPieces, matchPieces, newX, newY, color);
                }

                if (matchPieces.Count >= 3)//���������������������ϸ��Ӷ��� ˵���˴ν����������Ӷ���Ĳ������Խ��� ֱ�ӽ��� ���ҷ��� ���������� ���ü�����һ������������
                {
                    return matchPieces;
                }

            }
            //Ĭ����������ͬԪ������
            return null;
        }

        return null;

    }

    /// <summary>
    /// L��ƥ�� ������ֱ��ƥ��ɹ��Ժ� �ڸ�ֱ�߷�����û���������������ӵĸ��Ӷ���
    /// ˮƽ�ɹ�ƥ���Ժ� ��ÿһ��ˮƽ���Ӷ�������������д�ֱ�������������Ѹø��ӵ����������ӵĸ��Ӷ�������������������
    /// ���ߴ�ֱ�ɹ�ƥ���Ժ� ��ÿһ����ֱ���Ӷ��������������ˮƽ�������������Ѹø��ӵ����������ӵĸ��Ӷ�������������������
    /// </summary>
    /// <param name="horizontalPieces"></param>
    /// <param name="verticalPieces"></param>
    /// <param name="matchPieces"></param>
    public void LTypeMatch(List<GamePiece> horizontalPieces,List<GamePiece> verticalPieces, List<GamePiece> matchPieces,int newX,int newY,ColorPiece.ColorType color)
    {
        //ˮƽƥ��ɹ�ʱ��L��
        if (horizontalPieces.Count >= 3)
        {
            for (int i = 0; i < horizontalPieces.Count; i++)
            {
                //��ƥ��õ�ֱ�߸��Ӷ��󼯺ϵ�ÿһ�����Ӷ���ʼ ���д�ֱ��������

                //��ʼ��ֱ��������
                for (int dir = 0; dir <= 1; dir++) //0 1����ֱ�����ķ��� �������� ��һ�������� �ڶ���������
                {
                    for (int yOffset = 1; yOffset < yDim; yOffset++) //��ʼ�ϻ����±���ͬ�еĸ��Ӷ��� ֱ�������뵱ǰ���Ӷ�����ɫ����ͬ�ͽ���
                    {
                        //yOffset �������Ӷ����ƫ��λ�� ���������Ӷ����λ��
                        int y;//ͬ�е�ǰ�������ĸ��Ӷ���yλ�� 

                        if (dir == 0) //������
                        {
                            y = newY - yOffset; //���λ�� - ƫ��λ��
                        }
                        else//������
                        {
                            y = newY + yOffset;//���λ�� +ƫ��λ��
                        }

                        if (y < 0 || y >= yDim) //y ����Խ��
                        {
                            break;
                        }
                        //����˳��������ĸ��Ӷ�������������� �����������Ӷ�����ͬ��ɫ�ͼ��뼯�� ����ø��Ӷ�����ͬ
                        if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorCompinent.Color == color)
                        {
                            verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                        }
                        else //���Ǿ�ֱ�ӽ����÷���Ĵ�ֱ����
                        {
                            break;
                        }

                    }


                }
                //����һ�η��ָø��Ӷ���Ĵ�ֱ���򲻹����� �������ֱ�����е�Ԫ��
                if (verticalPieces.Count < 2)
                {
                    verticalPieces.Clear();
                }
                else //������ͼ��뵽���������� Ȼ��ֱ�ӽ��� ����Ҫ��������
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

        //��ֱƥ��ɹ�ʱ��L��

        if(verticalPieces.Count > 3)
        {
            for(int i =  0; i < verticalPieces.Count; i++)
            {

                //��ʼˮƽ��������
                for (int dir = 0; dir <= 1; dir++) //0 1����ˮƽ�����ķ��� �������� ��һ�������� �ڶ���������
                {
                    for (int xOffset = 1; xOffset < xDim; xOffset++) //��ʼ������ұ���ͬ�еĸ��Ӷ��� ֱ�������뵱ǰ���Ӷ�����ɫ����ͬ�ͽ���
                    {
                        //xOffset �������Ӷ����ƫ��λ�� ���������Ӷ����λ��
                        int x;//ͬ�е�ǰ�������ĸ��Ӷ���Xλ�� 

                        if (dir == 0) //������
                        {
                            x = newX - xOffset; //���λ�� - ƫ��λ��
                        }
                        else//������
                        {
                            x = newX + xOffset;//���λ�� +ƫ��λ��
                        }

                        if (x < 0 || x >= xDim) //X ����Խ��
                        {
                            break;
                        }
                        //ˮƽ˳��������ĸ��Ӷ�������������� �����������Ӷ�����ͬ��ɫ�ͼ��뼯�� ����ø��Ӷ�����ͬ
                        if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorCompinent.Color == color)
                        {
                            horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                        }
                        else //���Ǿ�ֱ�ӽ����÷����ˮƽ����
                        {
                            break;
                        }

                    }

                }

                //����һ�η��ָø��Ӷ����ˮƽ���򲻹����� �����ˮƽ�����е�Ԫ��
                if (horizontalPieces.Count < 2)
                {
                    horizontalPieces.Clear();
                }
                else //������ͼ��뵽���������� Ȼ��ֱ�ӽ��� ����Ҫ��������
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

    #region �������
    /// <summary>
    /// �������и��Ӷ��� ������ÿһ�����Ӷ������ƥ����� ���ƥ��ɹ������
    /// </summary>
    /// <returns></returns>

    public bool ClearAllValidMatches()
    {
        //�Ƿ���Ҫ�������
        bool needRefill = false;

        for(int y = 0;y < yDim;y++)//���б���ÿһ�����Ӷ���
        {

            for(int x = 0;x < xDim;x++)
            {
                if (pieces[x,y].IsClearable()) //�Ƿ��ǿ�����ĸ��Ӷ���
                {
                    //�Ե�ǰ���Ӷ���Ϊ���ƥ����Ӷ���
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);

                    if(match != null)//ƥ��ɹ���������
                    {
                        GameMusicManager.Instance.PlayClearMusic();

                        //����һ�γɹ��Ժ� ����һ���������ͣ�����һ������4����������������Ӷ���
                        //Ĭ�ϲ�������������Ӷ�������
                        PieceType specialPieceType = PieceType.Count;
                        //�������������� ���ѡ��һ��������Ԫ�� ������λ��������������Ӷ���
                        GamePiece randomPiece = match[UnityEngine.Random.Range(0, match.Count)];
                        //������Ӷ������ɵ�λ��
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;

                        //����һ������4����������������Ӷ���
                        if(match.Count == 4)
                        {
                            //�������Ȼ������� ʱ�����Ǿ��������һ��������Ӷ����л����еĸ��Ӷ���
                            if(pressPiece == null || enterPiece == null)
                            {
                                specialPieceType = (PieceType)UnityEngine.Random.Range((int)PieceType.Row_Clear, (int)PieceType.Column_Clear);
                            }
                            else if(pressPiece.Y == enterPiece.Y)//��ҽ������������ ������ˮƽ�����ЩԪ�� ��������������Ӷ���
                            {
                                specialPieceType = PieceType.Row_Clear;
                            }
                            else//��ҽ������������ �����Ǵ�ֱ�����ЩԪ�� ��������������Ӷ���
                            {
                                specialPieceType = PieceType.Column_Clear;
                            }

                        }else if(match.Count == 5) //����������ͬ��ɫ��������Ӷ���
                        {
                            specialPieceType = PieceType.Rainbow;
                        }


                        for (int i = 0; i < match.Count; i++)
                        {
                            //������ű��ĸ��Ӷ��� ���� ���������״̬�ĸ��Ӷ���ſ������
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needRefill = true;

                                //����ǽ����������ǾͲ������ ������Ӷ���λ�� �ý������Ǹ�λ�þ���
                                //������ܷ������λ�� ����Ϊ Destor��������һ֡���� ���ǻ��ܷ��ʵ���ǰ���Ӷ�������
                                if (match[i] == pressPiece || match[i] == enterPiece)
                                {
                                    //������Ӷ����λ�ø���
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }

                            }
                        }

                        //����������Ժ� ��������Ӷ������ͱ�ʶ���� ˵�����Է�������������Ӷ���
                        if(specialPieceType != PieceType.Count)
                        {
                            //ɾ��������Ӷ���λ���ϵĸ��Ӷ���
                            Destroy(pieces[specialPieceX,specialPieceY]);
                            //���ɸ����������
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY,specialPieceType);



                            //������Ӷ��� �������ɵ�������Ӷ�����������ɫ �����������ʱ�ĸ��Ӷ������������ɫ
                            if((specialPieceType == PieceType.Row_Clear || specialPieceType == PieceType.Column_Clear)
                                && newPiece.IsColored() && match[0].IsColored())
                            {

                                //���ǰ����ɵ�������Ӷ��� ��ɫ(����) ��ɺ� �ո������� �������Ӷ���һ������
                                newPiece.ColorCompinent.SetColor(match[0].ColorCompinent.Color);

                            }
                            else if(specialPieceType == PieceType.Rainbow && newPiece.IsClearable()) //��ͬ��ɫ������������Ӷ���
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
    /// �������ִ�к���
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool ClearPiece(int x,int y)
    {
        //�ж��Ƿ������� ���Ҳ����������

        if (pieces[x,y].IsColored() && pieces[x,y].ClearableCompinent.IsBeginCleared == false)
        {
            //��������ű��е����
            pieces[x,y].ClearableCompinent.Clear();
            //��λ�ñ�ɿ�
            SpawnNewPiece(x, y, PieceType.Empty);

            //�жϸ�����ĸ��Ӷ�������������û���ϰ��� �о����
            ClearObstacles(x, y);

            return true;
        }

        return false;
    }

    /// <summary>
    /// �������Ӷ���ʱ���� ��Χ�Ƿ����ϰ��� �о�����
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ClearObstacles(int x,int y)
    {
        //��ǰ�������ĸ��Ӷ��� ѭ����������һ��ĸ��Ӷ����ǲ����ϰ���
        for(int adjacentX = x -1;adjacentX <= x + 1;adjacentX++) 
        {
            //����Խ��
            if(adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                //�����ĸ������ϰ��� ���ҿ��Ա�����
                if (pieces[adjacentX,y].Type == PieceType.Bubble && pieces[adjacentX, y].IsClearable())
                {
                    //�����ϰ���
                    pieces[adjacentX, y].ClearableCompinent.Clear();
                    //���ɿո������ϰ���λ��
                    SpawnNewPiece(adjacentX, y, PieceType.Empty);
                }
            }
        }
        //��ǰ�������ĸ��Ӷ��� ѭ����������һ��ĸ��Ӷ����ǲ����ϰ���
        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            //����Խ��
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                //�����ĸ������ϰ��� ���ҿ��Ա�����
                if (pieces[x, adjacentY].Type == PieceType.Bubble && pieces[x, adjacentY].IsClearable())
                {
                    //�����ϰ���
                    pieces[x, adjacentY].ClearableCompinent.Clear();
                    //���ɿո������ϰ���λ��
                    SpawnNewPiece(x, adjacentY, PieceType.Empty);
                }
            }
        }


    }

    /// <summary>
    /// ������и��Ӷ���
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
    /// ������и��Ӷ���
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
    /// ���ָ����ͬ��ɫ����ĸ��Ӷ���
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

    #region �ؿ���ز���




    /// <summary>
    /// ��Ϸ��������
    /// </summary>
    public void GameOver()
    {
        gameOver = true;
    }
    /// <summary>
    /// �������������ж��ٸ� ��������� �ĸ��Ӷ��� ���Ǹ��������ж��ٸ��ϰ���
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
