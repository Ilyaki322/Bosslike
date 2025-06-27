using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClipPlayer : NetworkBehaviour
{
    static ClipPlayer m_instance;

    private Dictionary<int, AudioClip> m_clipDict = new();

    [SerializeField] private AudioListener m_listener;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }

    public static ClipPlayer Instance()
    {
        return m_instance;
    }

    public int RegisterClip(AudioClip clip)
    {
        int i = m_clipDict.Count;
        m_clipDict[i] = clip;
        return i;
    }

    public void PlayClipAt(AudioClip clip, Vector3 loc)
    {
        AudioSource.PlayClipAtPoint(clip, loc);
    }

    public void PlayClipAtNetwork(int clip, Vector3 loc)
    {
        PlayClipAtPointRpc(clip, loc);
    }


    [Rpc(SendTo.Everyone)]
    public void PlayClipAtPointRpc(int clip, Vector3 loc)
    {
        if (!m_clipDict.ContainsKey(clip))
        {
            print("Missing audio clip");
            return;
        }

        AudioSource.PlayClipAtPoint(m_clipDict[clip], loc);
    }

    public void OffListener()
    {
        m_listener.enabled = false;
    }
}
