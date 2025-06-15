using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///格子对象的颜色：相当于格子对象的种类
/// </summary>
public class ColorPiece : MonoBehaviour
{
    /// <summary>
    /// 颜色的类型
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
    /// 颜色的配置结构体
    /// </summary>
    [Serializable]
    public struct ColorSprite
    {
        public ColorType Type;//颜色类型
        public Sprite sprite;//颜色图片
    }
    /// <summary>
    /// 保存配置的颜色结构体数据  用来显示和加载格子对象颜色(种类)
    /// </summary>
    public ColorSprite[] colorSprites;
    /// <summary>
    /// 保存所有配置的颜色数据 和格子对象(GamePiece)的方式差不多 
    /// </summary>
    private Dictionary<ColorType, Sprite> colorSpriteDict;
    /// <summary>
    /// 显示颜色用的
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// 当前有多少种颜色
    /// </summary>
    public int NumColors() {   return colorSprites.Length;  }

    /// <summary>
    /// 当前格子对象是哪种颜色
    /// </summary>
    private ColorType color;
    /// <summary>
    /// 设置颜色就调用函数来操作设置
    /// </summary>
    public ColorType Color { get { return color; } set { SetColor(value); } }

    //注意：虽然格子颜色脚本记录所有颜色数据 但是一个格子对象只能是一种颜色（种类）
    //相当于 一个格子对象都一有具体颜色 并且是唯一的不能改变 蓝莓就是蓝莓 橘子就是橘子 

    private void Awake()
    {
        spriteRenderer = transform.Find("piece").GetComponent<SpriteRenderer>();
        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        //遍历配置颜色数组的数据 加入到字典里面去
        for (int i = 0; i < colorSprites.Length; i++)
        {
            if(colorSpriteDict.ContainsKey(colorSprites[i].Type) == false)
            {
                //字典加入配置好的数据
                colorSpriteDict.Add(colorSprites[i].Type, colorSprites[i].sprite);
            }

        }

    }

    /// <summary>
    /// 给格子对象设置颜色(种类) 并且添加显示相应的图片
    /// </summary>
    /// <param name="newColortype"></param>
    public void SetColor(ColorType newColortype)
    {
        //设置颜色
        color = newColortype;
        //设置相应的显示图片
        if (colorSpriteDict.ContainsKey(newColortype))
        {
           
            spriteRenderer.sprite = colorSpriteDict[newColortype];
        }

    }


}
