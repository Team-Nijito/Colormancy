using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    private static AudioScript s_instance;

    public enum SongType { LOBBY, STAGE, BOSS }

    [SerializeField]
    private List<AudioClip> songs;

    private AudioSource _audioSource;
    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            s_instance = this;
        }

        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySong(SongType type)
    {
        switch (type)
        {
            case SongType.LOBBY:
                _audioSource.clip = songs[0];
                break;
            case SongType.STAGE:
                _audioSource.clip = songs[1];
                break;
            case SongType.BOSS:
                _audioSource.clip = songs[2];
                break;
        }

        _audioSource.Play();
    }
}