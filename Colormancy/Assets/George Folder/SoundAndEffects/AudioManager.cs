using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    /*
     * This will instantiatiate different sound types.
     * There will be environmental and spatial sound.
     * There will also be a background music calculation.
     * 
     * Master
     * Effects
     * 
     */
    [System.Serializable]
    public class AudioType
    {
        public string audioTypeName;
        public float volume = 1;
        public bool mute = false;
        public AudioType[] _audioChildren;
        public Dictionary<string, AudioType> audioChildren;
        public Dictionary<int, GameObject> audioGameObjects;
        public float inheritedVolume;
        public float outputVolume;

        public void Pause()
        {
            if (audioGameObjects == null)
            {
                return;
            }
            foreach (var gm in audioGameObjects)
            {
                gm.Value.GetComponent<AudioSource>().Pause();
            }
        }

        public void Play()
        {
            if (audioGameObjects == null)
            {
                return;
            }
            foreach (var gm in audioGameObjects)
            {
                gm.Value.GetComponent<AudioSource>().Play();
            }
        }

        public void SetVolume(float volume)
        {
            if (audioGameObjects == null)
            {
                return;
            }
            foreach (var gm in audioGameObjects)
            {
                gm.Value.GetComponent<AudioSource>().volume = volume;
            }
        }

        public void SetUpAudioChildren()
        {
            foreach (var a in _audioChildren)
            {
                audioChildren = new Dictionary<string, AudioType>();

                foreach (var child in _audioChildren)
                {
                    audioChildren.Add(child.audioTypeName, child);
                }

                audioGameObjects = new Dictionary<int, GameObject>();
            }
        }

        public void Add(GameObject gm)
        {
            if (audioGameObjects == null)
            {
                audioGameObjects = new Dictionary<int, GameObject>();
            }
            audioGameObjects.Add(gm.GetInstanceID(), gm);
        }

        public GameObject Get(int id)
        {
            return audioGameObjects[id];
        }

        public void Remove(int id)
        {
            audioGameObjects.Remove(id);
        }

        public AudioType this[string path]
        {
            get
            {
                if (audioChildren.ContainsKey(path))
                {
                    return audioChildren[path];
                }
                return null;
            }
        }
    }

    AudioType audioTypes;
    public float maxVolume = 0;
    public float minVolume = 1;
    public AudioType loadFromDefaultAudioType;
    public bool loadFromDefault = false;
    public string calculateOnUpdatePath = "";
    public bool calculateOnUpdate = false;
    public bool removeSoundObjectsOnPath = false;

    // Cache System
    public GameObject audioSoucePrefab;
    public Transform cacheParent;
    public Queue<GameObject> cacheAudioSources;
    public AudioClip cacheTestClip;
    public bool cachePlayClip;
    public GameObject cacheGeneratedClip;
    public bool cacheRemoveClip;

    public delegate void OnDFS(AudioType audioType);

    private void Start()
    {
        cacheAudioSources = new Queue<GameObject>();
        if (loadFromDefault)
        {
            SetAudioTypeIntoManager(loadFromDefaultAudioType);
        }
    }

    private void Update()
    {
        if (calculateOnUpdate)
        {
            var a = GetAudioTypeFromPath(calculateOnUpdatePath);
            if (a == null)
            {
                return;
            }
            UpdateVolumeDFS(a);
            UpdateSoundObjectVolume(a);
        }

        if (removeSoundObjectsOnPath)
        {
            removeSoundObjectsOnPath = false;
            RemoveSoundObjects(GetAudioTypeFromPath(calculateOnUpdatePath));
        }

        if (cachePlayClip)
        {
            cachePlayClip = false;
            cacheGeneratedClip = PlaySound(calculateOnUpdatePath, cacheTestClip, audioSoucePrefab, Vector3.one);
        }

        if (cacheRemoveClip)
        {
            cacheRemoveClip = false;
            RemoveSound(cacheGeneratedClip);
        }
    }

    public void UpdateVolumes()
    {
        UpdateVolumeDFS(audioTypes);
        UpdateSoundObjectVolume(audioTypes);
    }

    public GameObject PlaySound(string path, AudioClip audio, GameObject prefab, Vector3 position, Transform parent = null)
    {
        GameObject gm = null;

        if (cacheAudioSources.Count > 0)
        {
            gm = cacheAudioSources.Dequeue();
        } else
        {
            gm = Instantiate(prefab);
        }
        gm.name = path + "-" + audio.name + "-" + gm.GetInstanceID();

        gm.SetActive(true);

        gm.transform.position = position;
        gm.transform.parent = parent;

        AudioType at = GetAudioTypeFromPath(path);
        AudioSource s = gm.GetComponent<AudioSource>();
        s.volume = at.outputVolume;
        s.clip = audio;
        s.Play();

        at.Add(gm);
        return gm;
    }

    public void RemoveSound(GameObject soundObject)
    {
        var n = soundObject.name;
        var ns = n.Split('-');
        var path = ns[0];

        soundObject.transform.parent = cacheParent;
        AudioSource s = soundObject.GetComponent<AudioSource>();
        s.Stop();
        AudioType at = GetAudioTypeFromPath(path);
        soundObject.SetActive(false);
        at.Remove(soundObject.GetInstanceID());

        cacheAudioSources.Enqueue(soundObject);
    }

    public bool RemoveSoundObjects(AudioType audioType)
    {
        return DFS(audioType, (AudioType audioType) =>
        {
            if (audioType.audioGameObjects == null)
                return;
            List<GameObject> l = new List<GameObject>(audioType.audioGameObjects.Values);
            foreach (var a in l)
            {
                RemoveSound(a);
            }
            audioType.audioGameObjects.Clear();
        });
    }

    // Eventually modularize for traversal to save code.
    public bool UpdateVolumeDFS(AudioType audioType, float setInheritedVolume = 1f)
    {
        audioType.inheritedVolume = setInheritedVolume;
        audioType.outputVolume = GetCalculatedVolume(audioType.volume, audioType.inheritedVolume, audioType.mute);
        
        if (audioType.audioChildren == null)
        {
            return true;
        }

        foreach (var audioChild in audioType.audioChildren)
        {
            UpdateVolumeDFS(audioChild.Value, audioType.outputVolume);
        }
        return true;
    }

    public bool UpdateSoundObjectVolume(AudioType audioType)
    {
        return DFS(audioType, (AudioType audioType) =>
        {
            audioType.SetVolume(audioType.outputVolume);
        });
    }

    public bool PauseSoundObjects(AudioType audioType)
    {
        return DFS(audioType, (AudioType audioType) =>
        {
            audioType.Pause();
        });
    }

    public bool PlaySoundObjects(AudioType audioType)
    {
        return DFS(audioType, (AudioType audioType) =>
        {
            audioType.Play();
        });
    }

    bool DFS(AudioType audioType, OnDFS runScript)
    {
        runScript(audioType);

        if (audioType.audioChildren == null)
        {
            return true;
        }

        foreach (var audioChild in audioType.audioChildren)
        {
            DFS(audioChild.Value, runScript);
        }
        return true;
    }

    public AudioType GetAudioTypeFromPath(string path)
    {
        string[] paths = path.Split('/');
        var currDir = audioTypes;
        foreach (var s in paths)
        {
            Debug.Log(s);
            if (s == "")
                return currDir;
            currDir = currDir[s];
        }
        return currDir;
    }

    public void SetAudioTypeIntoManager(AudioType audioType)
    {
        audioTypes = audioType;
        SetAudioType(audioTypes);
    }

    void SetAudioType(AudioType audioType)
    {
        DFS(audioType, (AudioType audioType) =>
        {
            audioType.SetUpAudioChildren();
        });
    }

    float GetCalculatedVolume(float volume, float inheritedVolume, bool mute = false)
    {
        var returnVolume = volume * inheritedVolume; //Mathf.Clamp(volume * inheritedVolume, minVolume, maxVolume); Does not work...
        returnVolume = Mathf.Max(returnVolume, 0);
        returnVolume = Mathf.Min(1, returnVolume);
        return (mute ? 0 : returnVolume);
    }

    public string SaveSettings()
    {
        return "";
    }

    public string LoadSettings()
    {
        return "";
    }

}
