using Assets.Scripts;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;

public class PhysicsSphere : MonoBehaviour, IPhysical
{

    public Vector3 acceleration, velocity;
    private const float gravity = 9.81f;
    float CoR = 0.5f;
    private Vector3 previousPosition;
    private Vector3 previousVelocity;
    private float timeInterval;

    public float Radius 
    { get 
        { 
            return transform.localScale.x / 2f; 
        } 
     internal set
        {
            transform.localScale = value * 2 * Vector3.one;
        }
    }

    public int rank  => 1; 

    public float mass = 1.0f;
    

    void Start()
    {
        acceleration = gravity * Vector3.down;   
    }

    void Update()

    { previousPosition = transform.position;
        previousVelocity = velocity;
        velocity += acceleration * Time.deltaTime; 
        transform.position += velocity * Time.deltaTime;
        /*
        transform.position -= velocity * Time.deltaTime;
        Vector3 vel_parallel = Utils.parallelTo(velocity, plane.normal);
        Vector3 vel_perpendicullar = Utils.perpendicularTo(velocity, plane.normal);
        velocity = -CoR * vel_parallel + vel_perpendicullar;
        */
    }

    public bool isColliding(IPhysical other)
    {
        if (other is PhysicsPlane)
        {
            return (other as PhysicsPlane).isCollidingWith(this);
        }

        if (other is PhysicsSphere sphere)
        {
            Vector3 v = sphere.transform.position - this.transform.position;
            return v.magnitude < (sphere.Radius + this.Radius);
        }

        return true;
    }

    public void resolvedVelosityForCollisionWith(IPhysical other, ref Vector3 positionOut, ref Vector3 velocityOut)
    {
        timeInterval = Time.deltaTime;

        if (other is PhysicsPlane)
        {   // Calculate Time of Impact (ToI) 
            PhysicsPlane plane = other as PhysicsPlane;

            float D0 = Utils.distanceToPlane(previousPosition, plane) - Radius;
            float D1 = Utils.distanceToPlane(transform.position, plane) - Radius;
            float totalDistance = D1 - D0;
            float speed = (totalDistance) / timeInterval;
            float ToI = -D0 / speed;



            // Resollve vel at ToI
            Vector3 velAtToI = previousVelocity + acceleration * ToI;
            Vector3 posAtToI = previousPosition + velAtToI * ToI;
            Vector3 parVel = Utils.parallelTo(velAtToI, plane.normal);
            Vector3 perpVel = Utils.perpendicularTo(velAtToI, plane.normal);
            Vector3 newVel = -CoR * parVel + perpVel;

            // resolve pos at end of interval
            float remainingTime = timeInterval - ToI;
            velocity = newVel + acceleration * remainingTime;
            transform.position = posAtToI + velocity * remainingTime;
            float d = Utils.distanceToPlane(transform.position, plane) - Radius;

            if (d < 0)
            {
                transform.position -= d * plane.normal;
            }
        }

        if (other is PhysicsSphere)
        {
            //calculate ToI
            PhysicsSphere sphere = other as PhysicsSphere;
            float D0 = Vector3.Distance(previousPosition, sphere.previousPosition) - Radius - sphere.Radius; //distance betwwen 2 spheres
            float D1 = Vector3.Distance(transform.position, sphere.transform.position) - Radius - sphere.Radius;
            
            float totalDistance = D1 - D0;
            float speed = (totalDistance) / timeInterval;
            float ToI = -D0 / speed;
            
            Vector3 velAtToI = previousVelocity + acceleration * ToI;
            Vector3 posAtToI = previousPosition + velAtToI * ToI;

            Vector3 velAtToIOther = sphere.previousVelocity + sphere.acceleration * ToI;
            Vector3 posAtToIOther = sphere.previousPosition + velAtToIOther * ToI;
            
            Vector3 normal = (posAtToI - posAtToIOther).normalized;

            Vector3 parVel = Utils.parallelTo(velAtToI, normal);
            Vector3 perpVel = Utils.perpendicularTo(velAtToI, normal);

            Vector3 parVelOther = Utils.parallelTo(velAtToIOther, normal);
            Vector3 perpVelOther = Utils.perpendicularTo(velAtToIOther, normal);

            Vector3 velPerpAfter = ElasticCollision(parVel, parVelOther, mass, sphere.mass);
            Vector3 velPerpAfterOther = ElasticCollision(parVelOther, parVel, sphere.mass, mass);
            Vector3 velAfter = -CoR * parVel + velPerpAfter;
            Vector3 velAfterOther = -CoR * parVelOther + velPerpAfterOther;

            float remainingTime = timeInterval -ToI;
            velocity = velAfter + acceleration * remainingTime;
            transform.position = posAtToI + velocity * remainingTime;

            velocityOut = velAfterOther + sphere.acceleration * remainingTime;
            positionOut = posAtToIOther + velocityOut * remainingTime;


            //Vector3 deltaPos = position - sphere.transform.position;
            //float dist = deltaPos.magnitude;
            //float minDist = Radius + sphere.Radius;

            //if (dist < minDist)
            //{
            //    Vector3 normal2 = deltaPos.normalized;

            //    Vector3 relVel = velocity - sphere.velocity;
            //    float relVelAlongNormal = Vector3.Dot(relVel, normal2);

            //}
            

        }
    }

    private Vector3 ElasticCollision(Vector3 parVel, Vector3 parVelOther, float mass1, float mass2)
    {
        float calc1 = ((mass1- mass2)/(mass1 + mass2));
        Vector3 x = calc1 * parVel;
        float calc2 = ((2 * mass2)/(mass1 + mass2));
        Vector3 y = calc2 * parVelOther;

        return x + y;
    }

    public void overrideAfterCollision(Vector3 pos, Vector3 vel)
    {
        transform.position = pos;
        velocity = vel;


    }
}
