using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏一样管理器
/// </summary>
public class GameMusicManager : MonoBehaviour
{
    private static GameMusicManager instance;
    public static GameMusicManager Instance { get { return instance; } }

    private AudioSource musicSource;
    private AudioSource musicButtonSource;
    private AudioSource musicClearSource;
    private AudioSource musicspecialClearSource;

    private AudioClip bgMusicClip;
    private AudioClip clearMusicClip;
    private AudioClip specialClearMusicClip;
    private AudioClip buttonMusicClip;
    private AudioClip winMusicCLip;
    private AudioClip loseMusicCLip;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            musicSource = gameObject.AddComponent<AudioSource>();
            musicButtonSource = gameObject.AddComponent <AudioSource>();
            musicClearSource = gameObject.AddComponent<AudioSource>();
            musicspecialClearSource = gameObject.AddComponent<AudioSource>();
            musicButtonSource.volume = 0.5f;
            StartCoroutine(LoadMusicClip());
        }

    }
    /// <summary>
    /// 播放消除音乐
    /// </summary>
    public void PlayClearMusic()
    {
        musicClearSource.clip = clearMusicClip;
        musicClearSource.Play();
       
    }
    /// <summary>
    /// 播放特殊格子对象消除的音乐
    /// </summary>
    public void PlaySpecialClearMusic()
    {
        musicspecialClearSource.clip = specialClearMusicClip;
        musicspecialClearSource.Play();
    }
    /// <summary>
    /// 点击音乐
    /// </summary>
    public void PlayButtonMusic()
    {
        musicButtonSource.clip = buttonMusicClip;
        musicButtonSource.Play();
    }
    /// <summary>
    /// 胜利音乐
    /// </summary>
    public void PlayWinMusic()
    {
        musicClearSource.clip = winMusicCLip;
        musicClearSource.Play();
    }
    /// <summary>
    /// 失败音乐
    /// </summary>
    public void PlayLoseMusic()
    {
        musicClearSource.clip = loseMusicCLip;
        musicClearSource.Play();
    }



    /// <summary>
    /// 音乐资源加载协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadMusicClip()
    {

        ResourceRequest bgClip = Resources.LoadAsync<AudioClip>("Sounds/With love from Vertex Studio (8)");
        yield return bgClip;
        bgMusicClip = (AudioClip)bgClip.asset;
        musicSource.clip = bgMusicClip;
        musicSource.volume = 0.05f;
        musicSource.Play();
        musicSource.loop = true;

        ResourceRequest clearClip = Resources.LoadAsync<AudioClip>("Sounds/Clear");
        yield return clearClip;
        clearMusicClip = (AudioClip)clearClip.asset;

        ResourceRequest winClip = Resources.LoadAsync<AudioClip>("Sounds/Win");
        yield return winClip;
        winMusicCLip = (AudioClip)winClip.asset;

        ResourceRequest loseClip = Resources.LoadAsync<AudioClip>("Sounds/Lose");
        yield return loseClip;
        loseMusicCLip = (AudioClip)loseClip.asset;

        ResourceRequest spClearClip = Resources.LoadAsync<AudioClip>("Sounds/BuyButton");
        yield return spClearClip;
        specialClearMusicClip = (AudioClip)spClearClip.asset;

        ResourceRequest buttonClip = Resources.LoadAsync<AudioClip>("Sounds/Button");
        yield return buttonClip;
        buttonMusicClip = (AudioClip)buttonClip.asset;

    }


}
