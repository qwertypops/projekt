using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private StateMachine stateMachine;
    public State[] states;
    public float skinWidth = 0.4f;
    public float groundCheckDistance = 0.2f;
    public LayerMask GeometryLayer;
    public Vector3 velocity;
    new public CapsuleCollider collider;
    public float Acceleration = 3;
    public float Deceleration = -2;
    public float minimumDeceleration = 1;
    public float maxSpeed = 5;
    public float staticFfriktionKoefficient = 0.5f;
    public float dynamicFriktionKoefficient = 0.36f;
    public float mouseSenesitivity = 10;
    public float gravity = 0.5f;
    public float jumpHeight = 1;
    public float minClamp = -90;
    public float maxClamp = 90;
    public Vector3 threedcameraposition;
    private float rotationX;
    private float rotationY;

    new public Camera camera;

    private void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
    }
    void Start()
    {
        stateMachine = new StateMachine(this, states);
    }

    void Update()
    {
        stateMachine.Run();
        PlayerMovement();
    }
    private void PlayerMovement()
    {
        FirststPersonCameraMovement();
        //ThirdPersonCameraMovement();
    }
    private void FirststPersonCameraMovement()
    {
        rotationY += Input.GetAxisRaw("Mouse X") * mouseSenesitivity;
        rotationX -= Input.GetAxisRaw("Mouse Y") * mouseSenesitivity;
        rotationX = Mathf.Clamp(rotationX, minClamp, maxClamp);
        camera.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }
    private void ThirdPersonCameraMovement()
    {
        rotationY += Input.GetAxisRaw("Mouse X") * mouseSenesitivity;
        rotationX -= Input.GetAxisRaw("Mouse Y") * mouseSenesitivity;
        rotationX = Mathf.Clamp(rotationX, minClamp, maxClamp);
        camera.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);

        Vector3 cameraposition = camera.transform.rotation * threedcameraposition;
        cameraposition += transform.position;

        RaycastHit cast;
        Physics.SphereCast(transform.position, collider.radius, cameraposition, out cast, threedcameraposition.magnitude, GeometryLayer);
        Debug.DrawRay(transform.position, cast.point);

        if (cast.collider != null)
        {
            camera.transform.position = cast.point;
        }
        else
        {
            camera.transform.position = cameraposition;
        }
        
    }
    public Vector3 CollisionDetection()
    {
        if (velocity.magnitude < 0.001f)
        {
            return Vector3.zero;
        }
        Vector3 p1 = transform.position + collider.center + Vector3.up * (collider.height / 2 - collider.radius);
        Vector3 p2 = transform.position + collider.center + Vector3.down * (collider.height / 2 - collider.radius);
        RaycastHit cast;
        Physics.CapsuleCast(p1, p2, collider.radius, velocity.normalized, out cast, velocity.magnitude * Time.deltaTime + skinWidth, GeometryLayer);

        if (cast.collider != null)
        {
            RaycastHit normalCast;
            Physics.CapsuleCast(p1, p2, collider.radius, -cast.normal, out normalCast, velocity.magnitude * Time.deltaTime + skinWidth, GeometryLayer);
            Vector3 normalkraft = StaticFunctions.NormalKraft(velocity, cast.normal);

            velocity += normalkraft;
            friction(normalkraft.magnitude);
            Vector3 e = -normalCast.normal * (normalCast.distance - skinWidth);
            if (Vector3.Dot(e, normalCast.normal) < 0)
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
    public void friction(float direction)
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