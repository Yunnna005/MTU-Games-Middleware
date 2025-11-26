using UnityEngine;
using UnityEngine.InputSystem;

public class BallThrower : MonoBehaviour
{
    public PhysicsSphere spherePrefab; 
    public Transform cameraTransform;  
    public float throwForce = 10f;
    public float distance = 1f;

    public float minScale = 0.2f;
    public float maxScale = 1f;
    public float minMass = 0.2f;
    public float maxMass = 2f;

    public void Throw(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PhysicsSphere newSphere = Instantiate(
                spherePrefab,
                cameraTransform.position + cameraTransform.forward * distance,
                Quaternion.identity
            );

            float scaleValue = Random.Range(minScale, maxScale);
            float massValue = Random.Range(minMass, maxMass);

            newSphere.transform.localScale = Vector3.one * scaleValue;
            newSphere.mass = massValue;

            newSphere.overrideAfterCollision(newSphere.transform.position, Vector3.zero);
            newSphere.velocity = cameraTransform.forward * throwForce;
        }
    }
}
