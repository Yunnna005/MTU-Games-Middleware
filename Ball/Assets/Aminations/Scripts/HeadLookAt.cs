using UnityEngine;

public class HeadLookAt : MonoBehaviour
{
    public Transform target;
    public float maxYaw = 55f;   
    public float maxPitch = 30f; 

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        Quaternion localRot = Quaternion.Inverse(transform.parent.rotation) * targetRot;
        Vector3 euler = localRot.eulerAngles;


        euler.x = (euler.x > 180) ? euler.x - 360 : euler.x;
        euler.y = (euler.y > 180) ? euler.y - 360 : euler.y;
        euler.z = 0f;

        euler.x = Mathf.Clamp(euler.x, -maxPitch, maxPitch);
        euler.y = Mathf.Clamp(euler.y, -maxYaw, maxYaw);

        transform.localRotation = Quaternion.Euler(euler);
    }
}
