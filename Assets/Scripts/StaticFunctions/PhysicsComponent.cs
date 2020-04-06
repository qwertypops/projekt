using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsComponent : MonoBehaviour
{
    public float Acceleration = 3;
    public float Deceleration = -2;
    public float minimumDeceleration = 1;
    public float maxSpeed = 5;
    public float staticFfriktionKoefficient = 0.5f;
    public float dynamicFriktionKoefficient = 0.36f;
    public float airResistance = 0.7f;
    public Vector2 velocity = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void friktion(float direction)
    {
        if (direction < (direction * staticFfriktionKoefficient))
        {
            velocity = Vector2.zero;
        }
        else
        {
            velocity += (-velocity * (direction * dynamicFriktionKoefficient));
        }
    }
    private void AirResistance()
    {
        velocity *= Mathf.Pow(airResistance, Time.deltaTime);
    }
}
