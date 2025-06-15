using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///���Ӷ������ɫ���൱�ڸ��Ӷ��������
/// </summary>
public class ColorPiece : MonoBehaviour
{
    /// <summary>
    /// ��ɫ������
    /// </summary>
  public enum ColorType
    {
        Red,
        Green, 
        Blue,
        PurpleItem,
        OrangeItem,
        Any
    }
    /// <summary>
    /// ��ɫ�����ýṹ��
    /// </summary>
    [Serializable]
    public struct ColorSprite
    {
        public ColorType Type;//��ɫ����
        public Sprite sprite;//��ɫͼƬ
    }
    /// <summary>
    /// �������õ���ɫ�ṹ������  ������ʾ�ͼ��ظ��Ӷ�����ɫ(����)
    /// </summary>
    public ColorSprite[] colorSprites;
    /// <summary>
    /// �����������õ���ɫ���� �͸��Ӷ���(GamePiece)�ķ�ʽ��� 
    /// </summary>
    private Dictionary<ColorType, Sprite> colorSpriteDict;
    /// <summary>
    /// ��ʾ��ɫ�õ�
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// ��ǰ�ж�������ɫ
    /// </summary>
    public int NumColors() {   return colorSprites.Length;  }

    /// <summary>
    /// ��ǰ���Ӷ�����������ɫ
    /// </summary>
    private ColorType color;
    /// <summary>
    /// ������ɫ�͵��ú�������������
    /// </summary>
    public ColorType Color { get { return color; } set { SetColor(value); } }

    //ע�⣺��Ȼ������ɫ�ű���¼������ɫ���� ����һ�����Ӷ���ֻ����һ����ɫ�����ࣩ
    //�൱�� һ�����Ӷ���һ�о�����ɫ ������Ψһ�Ĳ��ܸı� ��ݮ������ݮ ���Ӿ������� 

    private void Awake()
    {
        spriteRenderer = transform.Find("piece").GetComponent<SpriteRenderer>();
        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        //����������ɫ��������� ���뵽�ֵ�����ȥ
        for (int i = 0; i < colorSprites.Length; i++)
        {
            if(colorSpriteDict.ContainsKey(colorSprites[i].Type) == false)
            {
                //�ֵ�������úõ�����
                colorSpriteDict.Add(colorSprites[i].Type, colorSprites[i].sprite);
            }

        }

    }

    /// <summary>
    /// �����Ӷ���������ɫ(����) ���������ʾ��Ӧ��ͼƬ
    /// </summary>
    /// <param name="newColortype"></param>
    public void SetColor(ColorType newColortype)
    {
        //������ɫ
        color = newColortype;
        //������Ӧ����ʾͼƬ
        if (colorSpriteDict.ContainsKey(newColortype))
        {
           
            spriteRenderer.sprite = colorSpriteDict[newColortype];
        }

    }


}
