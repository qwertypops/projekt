using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticFunctions 
{
    public static float airResistance = 0.7f;
    public static Vector3 NormalKraft(Vector3 velocity, Vector3 normal)
    {
        float projection = Vector3.Dot(velocity, normal);
        if(projection > 0)
        {
            projection = 0;
        }
        Vector3 value = projection * normal;
        return -value;
    }
    public static Vector3 AirResistance(Vector3 velocity)
    {
        velocity *= Mathf.Pow(airResistance, Time.deltaTime);
        return velocity;
    }
    
}
