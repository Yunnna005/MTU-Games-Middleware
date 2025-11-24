using UnityEngine;
using UnityEngine.InputSystem;

public class BallThrower : MonoBehaviour
{
    public PhysicsSphere spherePrefab; 
    public Transform cameraTransform;  
    public float throwForce = 10f;
    public float distance = 1f;

    public void Throw(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PhysicsSphere newSphere = Instantiate(
                spherePrefab,
                cameraTransform.position + cameraTransform.forward * distance,
                Quaternion.identity
            );
            newSphere.overrideAfterCollision(newSphere.transform.position, Vector3.zero);
            newSphere.velocity = cameraTransform.forward * throwForce;
        }
    }
}
