using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3dController : MonoBehaviour
{
    public float Acceleration = 3;
    public float Deceleration = -2;
    public float minimumDeceleration = 1;
    public float maxSpeed = 5;
    public float staticFfriktionKoefficient = 0.5f;
    public float dynamicFriktionKoefficient = 0.36f;
    public float airResistance = 0.7f;
    public float mouseSenesitivity = 10;
    public LayerMask GeometryLayer;
    public float skinWidth = 0.4f;
    public Vector3 velocity = new Vector3();
    public float gravity = 0.5f;
    public float groundCheckDistance = 0.2f;
    public float jumpHeight = 1;
    public float minClamp = -90;
    public float maxClamp = 90;
    public Vector3 threedcameraposition;
    private float rotationX;
    private float rotationY;
   
    new public Camera camera;
    new CapsuleCollider collider;

    void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        
        
        CheckInput();
        Gravity();
        //FirststPersonCameraMovement();
        ThirdPersonCameraMovement();
        Jump();
        AirResistance();

        Vector3 movement = CollisionDetection();
        transform.position += movement * Time.deltaTime;
    }
    private void FirststPersonCameraMovement()
    {
        rotationY += Input.GetAxisRaw("Mouse X") * mouseSenesitivity;
        rotationX -= Input.GetAxisRaw("Mouse Y") * mouseSenesitivity;
        rotationX = Mathf.Clamp(rotationX, minClamp, maxClamp);
        camera.transform.rotation = Quaternion.Euler(rotationX,rotationY,0);
    }
    private void ThirdPersonCameraMovement()
    {
        rotationY += Input.GetAxisRaw("Mouse X") * mouseSenesitivity;
        rotationX -= Input.GetAxisRaw("Mouse Y") * mouseSenesitivity;
        rotationX = Mathf.Clamp(rotationX, minClamp, maxClamp);
        camera.transform.rotation = Quaternion.Euler(rotationX,rotationY,0);
        
        Vector3 cameraposition = camera.transform.rotation * threedcameraposition;
        cameraposition += transform.position;

        RaycastHit cast;
        Physics.SphereCast(transform.position, collider.radius, cameraposition, out cast, threedcameraposition.magnitude, GeometryLayer);
        Debug.DrawRay(transform.position, cast.point);
        
        if (cast.collider != null)
        {
            camera.transform.position =  cast.point;
        }
        else
        {
            camera.transform.position = cameraposition;
        }
        //camera.transform.position = cameradirection;
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
        Vector3 p1 = transform.position + collider.center + Vector3.up * (collider.height / 2 - collider.radius);
        Vector3 p2 = transform.position + collider.center + Vector3.down * (collider.height / 2 - collider.radius);
        if (Physics.CapsuleCast(p1, p2, collider.radius, Vector2.down, skinWidth, GeometryLayer))
        {
            velocity.y = 0;
        }
        else
        {
            velocity += (Vector3.down * gravity * Time.deltaTime);
        }
    }
    private void CheckInput()
    {
        Vector3 p1 = transform.position + collider.center + Vector3.up * (collider.height / 2 - collider.radius);
        Vector3 p2 = transform.position + collider.center + Vector3.down * (collider.height / 2 - collider.radius);
        RaycastHit cast;
        Physics.CapsuleCast(p1, p2, collider.radius, Vector3.down, out cast, 1000, GeometryLayer);

        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        direction = camera.transform.rotation * direction;
        Vector3 projection = Vector3.ProjectOnPlane(direction, cast.normal);
        direction = projection;


        if (direction.magnitude > 0)
        {
        if (direction.magnitude > 1)
        {
            direction = direction.normalized;
        }
            Accelerate(direction);
        }
        else
        {
            Decelerate();
        }
    }
    private void Accelerate(Vector3 direction)
    {
        float currentDirection = Vector3.Dot(direction.normalized, velocity.normalized);
        float turnSpeed = Mathf.Lerp(0.3f, 0.6f, currentDirection);
        velocity += direction * (Acceleration + turnSpeed) * Time.deltaTime;

        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

    }
    private void Decelerate()
    {
        Vector3 decelerationDirection = velocity;
        decelerationDirection.y = 0;
        if (decelerationDirection.magnitude > minimumDeceleration)
        {
            velocity += decelerationDirection.normalized * Deceleration * Time.deltaTime;
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }

    }
    private Vector3 CollisionDetection()
    {
        if(velocity.magnitude < 0.001f)
        {
            return Vector3.zero;
        }
        Vector3 p1 = transform.position + collider.center + Vector3.up * (collider.height / 2 - collider.radius);
        Vector3 p2 = transform.position + collider.center + Vector3.down * (collider.height / 2 - collider.radius);
        RaycastHit cast;
        Physics.CapsuleCast(p1, p2, collider.radius, velocity.normalized, out cast, velocity.magnitude * Time.deltaTime + skinWidth , GeometryLayer);
        
        if (cast.collider != null)
        {
            RaycastHit normalCast;
            Physics.CapsuleCast(p1, p2, collider.radius, -cast.normal, out normalCast, velocity.magnitude * Time.deltaTime + skinWidth, GeometryLayer);
            Vector3 normalkraft = StaticFunctions.NormalKraft(velocity, cast.normal);

            velocity += normalkraft;
            friction(normalkraft.magnitude);
            Vector3 e = -normalCast.normal * (normalCast.distance - skinWidth);
            if(Vector3.Dot(e, normalCast.normal) < 0)
            {
                transform.position += e;
            }
            
            return CollisionDetection();
        }
        else
        {
            return velocity;
        }
    }
    private void friction(float direction)
    {
        if (velocity.magnitude < (direction * staticFfriktionKoefficient))
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity += (-velocity.normalized * (direction * dynamicFriktionKoefficient));
        }
    }
    public bool IsPlayerGrounded()
    {
        Vector3 p1 = transform.position + collider.center + Vector3.up * (collider.height / 2 - collider.radius);
        Vector3 p2 = transform.position + collider.center + Vector3.down * (collider.height / 2 - collider.radius);
        if (Physics.CapsuleCast(p1, p2, collider.radius, Vector3.down, groundCheckDistance + skinWidth, GeometryLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
