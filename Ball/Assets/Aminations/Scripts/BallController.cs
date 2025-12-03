using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{

    public bool IsHeld { get; private set; }
    public Transform Holder { get; private set; }
    public float thrownReleaseDelay = 0.15f;
    public float landReleaseDelay = 0.1f;

    Rigidbody rb;
    Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public bool TryPickUp(Transform newHolder, Transform handPoint)
    {
        if (IsHeld) return false;

        IsHeld = true;
        Holder = newHolder;

        rb.isKinematic = true;
        col.isTrigger = true;

        transform.SetParent(handPoint, true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        return true;
    }

    public void Throw(Vector3 direction, float force)
    {
        if (!IsHeld) return;

        transform.SetParent(null);
        IsHeld = false;
        Holder = null;

        StartCoroutine(ReleaseAfterEndOneFrame(direction.normalized * force));
    }

    IEnumerator ReleaseAfterEndOneFrame(Vector3 impulse)
    {
        // small delay to avoid immediate pickup by the thrower
        yield return new WaitForSeconds(thrownReleaseDelay);

        rb.isKinematic = false;
        col.isTrigger = false;

        rb.AddForce(impulse, ForceMode.Impulse);

        // optional: allow a short grace period before another pickup
        yield return new WaitForSeconds(landReleaseDelay);
    } 
}

