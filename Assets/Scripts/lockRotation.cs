using UnityEngine;

public class lockRotation : MonoBehaviour
{
    private Quaternion rot;
    private void Awake()
    {
        rot = transform.rotation;
    }

    private void Update()
    {
        transform.rotation = rot;
    }

}
