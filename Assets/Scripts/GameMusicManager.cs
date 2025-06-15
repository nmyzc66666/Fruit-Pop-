using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��Ϸһ��������
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
    /// ������������
    /// </summary>
    public void PlayClearMusic()
    {
        musicClearSource.clip = clearMusicClip;
        musicClearSource.Play();
       
    }
    /// <summary>
    /// ����������Ӷ�������������
    /// </summary>
    public void PlaySpecialClearMusic()
    {
        musicspecialClearSource.clip = specialClearMusicClip;
        musicspecialClearSource.Play();
    }
    /// <summary>
    /// �������
    /// </summary>
    public void PlayButtonMusic()
    {
        musicButtonSource.clip = buttonMusicClip;
        musicButtonSource.Play();
    }
    /// <summary>
    /// ʤ������
    /// </summary>
    public void PlayWinMusic()
    {
        musicClearSource.clip = winMusicCLip;
        musicClearSource.Play();
    }
    /// <summary>
    /// ʧ������
    /// </summary>
    public void PlayLoseMusic()
    {
        musicClearSource.clip = loseMusicCLip;
        musicClearSource.Play();
    }



    /// <summary>
    /// ������Դ����Э��
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
