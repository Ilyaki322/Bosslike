using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DamagePopup : NetworkBehaviour
{
    [SerializeField] TextMeshPro m_text;

    //public void Config(float lifetime, Vector3 pos, float dmg)
    //{
    //    if (!IsServer) return;

    //    ClientDamagePopupRpc(pos, dmg);
    //    StartCoroutine(ProjectileDestroyCoroutine(lifetime));
    //}
    public void Config(float lifetime, Vector3 pos, float dmg, ulong attackerID)
    {
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
