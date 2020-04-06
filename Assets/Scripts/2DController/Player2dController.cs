using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2dController : MonoBehaviour
{
    public float Acceleration = 3;
    public float Deceleration = -2;
    public float minimumDeceleration = 1;
    public float maxSpeed = 5;
    public float staticFfriktionKoefficient = 0.5f;
    public float dynamicFriktionKoefficient = 0.36f;
    public float airResistance = 0.7f;
    public LayerMask GeometryLayer;
    public float skinWidth = 0.4f;
    public Vector2 velocity = new Vector2();
    //private Vector3 magnitudeToRemove = new Vector3();
    //private float magnitudeToRemove;
    public float gravity = 0.5f;
    public float groundCheckDistance = 0.2f;
    public float jumpHeight = 1;
    public PhysicsComponent physicComponent;
    new BoxCollider2D collider;

    void Awake() => collider = GetComponent<BoxCollider2D>();

    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Gravity();
        Jump();
        CheckInput();
        AirResistance();

        Vector3 movement = CollisionDetection();
        //transform.position += movement - movement.normalized * magnitudeToRemove.magnitude;
        //transform.position += movement - (magnitudeToRemove.normalized * magnitudeToRemove.magnitude);
        //transform.position += movement + (movement.normalized *- magnitudeToRemove);
        //magnitudeToRemove = 0;
        //movement -= (movement.normalized * magnitudeToRemove.magnitude);
        //magnitudeToRemove = Vector3.zero;
        transform.position += movement;
    }


    private void AirResistance()
    {
        velocity *= Mathf.Pow(airResistance, Time.deltaTime);
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsPlayerGrounded())
            {
                velocity.y += jumpHeight;
            }
        }
    }
    private void Gravity()
    {
        if (Physics2D.BoxCast(transform.position, collider.size, 0.0f, Vector2.down, skinWidth, GeometryLayer))
        {
            velocity.y = 0;
        }
        else
        {
            velocity += (Vector2.down * gravity * Time.deltaTime);
        }
    }
    private void CheckInput()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if(direction.magnitude > 0)
        {
            Accelerate(direction);
        }
        else
        {
            Decelerate();
        }
    }
    private void Accelerate(Vector2 direction)
    {
        float currentDirection = Vector2.Dot(direction.normalized, velocity.normalized);
        float turnSpeed = 0;
        if(currentDirection < 0)
        {
            float currentSpeed = 0;
            if(velocity.x < 0)
            {
                currentSpeed = -velocity.x * 3;
            }
            else
            {
                currentSpeed = velocity.x * 3;
            }
            turnSpeed = -currentDirection * 10 * currentSpeed;
        }
        velocity += direction * (Acceleration + turnSpeed) * Time.deltaTime;
        
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

    }
    private void Decelerate()
    {
        Vector2 decelerationDirection = velocity;
        decelerationDirection.y = 0;
        if (decelerationDirection.magnitude > minimumDeceleration)
        {
            velocity += decelerationDirection.normalized * Deceleration * Time.deltaTime;
        }
        else
        {
            velocity.x = 0;
        }

    }
    private Vector2 CollisionDetection()
    {
        RaycastHit2D cast = Physics2D.BoxCast(transform.position, collider.size, 0, velocity.normalized, velocity.magnitude + skinWidth, GeometryLayer);
        if (cast.collider != null && velocity.magnitude > 0.001f)
        {
            RaycastHit2D castAgainstNormal = Physics2D.BoxCast(transform.position, collider.size, 0, -cast.normal, velocity.magnitude + skinWidth, GeometryLayer);
            Vector2 normalkraft = StaticFunctions.NormalKraft(velocity, cast.normal);

            velocity += normalkraft;
            friktion(normalkraft.magnitude);
            Vector3 e = -castAgainstNormal.normal * (castAgainstNormal.distance - skinWidth);
            
            transform.position += e;
            //magnitudeToRemove += e;
            //magnitudeToRemove += e.magnitude;
            //velocity -= (velocity.normalized * e.magnitude);
            return CollisionDetection();
        }
        else
        {
            return velocity;
        }
    }
    private void friktion(float direction)
    {
        if(direction < (direction * staticFfriktionKoefficient))
        {
            velocity = Vector2.zero;
        }
        else
        {
            velocity += (-velocity * (direction * dynamicFriktionKoefficient));
        }
    }
    private bool IsPlayerGrounded()
    {
        if (Physics2D.BoxCast(transform.position, collider.size, 0.0f, Vector2.down, groundCheckDistance + skinWidth, GeometryLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}



//float distance = Acceleration * Time.deltaTime;
//Vector2 Movement = direction * distance;
//return Movement.normalized * (cast.distance - skinWidth);