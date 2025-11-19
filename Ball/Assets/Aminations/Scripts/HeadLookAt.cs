using UnityEngine;

public class HeadLookAt : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        transform.LookAt(transform.position + dir);
    }
}
