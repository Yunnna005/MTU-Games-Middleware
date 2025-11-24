using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    List<IPhysical> allObjects = new List<IPhysical>();

    void Start()
    {
        /*allObjects.Clear();

        var allMonobehaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (var mb in allMonobehaviours)
        {
            if (mb is IPhysical physical)
            {
                allObjects.Add(physical);
            }
        }*/
    }

    private void Update()
    {
        var allMonobehaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (var mb in allMonobehaviours)
        {
            if (mb is IPhysical physical && !allObjects.Contains(physical))
            {
                allObjects.Add(physical);
            }
        }

        for (int i = 0; i< allObjects.Count-1; i++)
        {
            for (int j = i+1; j< allObjects.Count; j++)
            {
                Vector3 pos = Vector3.zero, vel = Vector3.zero;
                if (allObjects[i].rank >= allObjects[j].rank)
                {
                 
                    if (allObjects[i].isColliding(allObjects[j]))
                    {
                        allObjects[i].resolvedVelosityForCollisionWith(allObjects[j], ref pos, ref vel);
                        allObjects[j].overrideAfterCollision(pos, vel);
                    }
                }
                else
                {
                    if (allObjects[j].isColliding(allObjects[i]))
                    {
                        allObjects[j].resolvedVelosityForCollisionWith(allObjects[i], ref pos, ref vel);
                        allObjects[i].overrideAfterCollision(pos, vel);
                    }
                }

                
            }
        }
    }
}
