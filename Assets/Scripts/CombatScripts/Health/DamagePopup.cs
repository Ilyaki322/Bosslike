using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DamagePopup : NetworkBehaviour
{
    [SerializeField] TextMeshPro m_text;
    [SerializeField] Color m_textColor;
    float m_disappearTime = 0.15f;

    //public void Config(float lifetime, Vector3 pos, float dmg)
    //{
    //    if (!IsServer) return;

    //    ClientDamagePopupRpc(pos, dmg);
    //    StartCoroutine(ProjectileDestroyCoroutine(lifetime));
    //}


    private void Update()
    {
        float moveSpeed = 2f;
        transform.position += new Vector3(0, moveSpeed) * Time.deltaTime;

        m_disappearTime -= Time.deltaTime;
        if (m_disappearTime < 0)
        {
            float disappearSpeed = 1.5f;
            m_textColor.a -= disappearSpeed * Time.deltaTime;
            m_text.color = m_textColor;
        }

    }

    public void Config(float lifetime, Vector3 pos, float dmg, ulong attackerID)
    {
        //m_disappearTime = 0.15f;
        //m_textColor.a = 255f;
        if (!IsServer) return;

        var damager = new RpcParams
        {
            Send = new RpcSendParams
            {
                Target = RpcTarget.Single(attackerID, RpcTargetUse.Temp)
            }
        };
        var others = new RpcParams
        {
            Send = new RpcSendParams
            {
                Target = RpcTarget.Not(attackerID, RpcTargetUse.Temp)
            }
        };

        HidePopupRpc(others);
        ClientDamagePopupRpc(pos, dmg, damager);
        StartCoroutine(ProjectileDestroyCoroutine(lifetime));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void ClientDamagePopupRpc(Vector3 pos, float dmg, RpcParams clientRpcParams)
    {
        m_disappearTime = 0.15f;
        m_textColor.a = 255f;
        m_text.text = ((int)dmg).ToString();
        transform.position = pos + new Vector3(Random.Range(0, 4), 2f + Random.Range(0, 2), 0);
    }
    [Rpc(SendTo.SpecifiedInParams)]
    public void HidePopupRpc(RpcParams clientRpcParams)
    {
        m_text.text = "";
    }

    IEnumerator ProjectileDestroyCoroutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (!NetworkObject.IsSpawned) return;
        NetworkObject.Despawn(true);
    }
}
