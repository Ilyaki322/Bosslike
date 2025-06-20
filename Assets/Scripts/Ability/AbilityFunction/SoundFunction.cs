using Unity.Netcode.Components;
using UnityEngine;

public class SoundFunction : AbilityFunction
{
    [SerializeField] SoundData m_data;
    private int m_soundIndex;

    protected override void Use()
    {
        if (m_data.Multiplayer)
        {
            ClipPlayer.Instance().PlayClipAtNetwork(m_soundIndex, transform.parent.position);
        }
        else
        {
            ClipPlayer.Instance().PlayClipAt(m_data.Clip, transform.parent.position);
        }
    }

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_data = data as SoundData;
        m_soundIndex = ClipPlayer.Instance().RegisterClip(m_data.Clip);
    }
}
