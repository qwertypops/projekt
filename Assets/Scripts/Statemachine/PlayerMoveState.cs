using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/MoveState")]
public class PlayerMoveState : PlayerBaseState
{
    public float Speed;
    public float MaxWalkSpeed;
    public float MinWalkSpeed;

    public override void Enter()
    {
        Debug.Log("Enter Move State");
    }
    public override void Run()
    {
        Jump();
        IsPlayerAirborne();
        Gravity();
        CheckInput();

        StaticFunctions.AirResistance(Player.velocity);
        Vector3 movement = Player.CollisionDetection();
        Player.transform.position += movement * Time.deltaTime;
    }
    private void IsPlayerAirborne()
    {
        if(!(Player.IsPlayerGrounded())){
            stateMachine.TransitionTo<PlayerFlyState>();
        }
    }
    private void CheckInput()
    {
        Vector3 p1 = Player.transform.position + Player.collider.center + Vector3.up * (Player.collider.height / 2 - Player.collider.radius);
        Vector3 p2 = Player.transform.position + Player.collider.center + Vector3.down * (Player.collider.height / 2 - Player.collider.radius);
        RaycastHit cast;
        Physics.CapsuleCast(p1, p2, Player.collider.radius, Vector3.down, out cast, 1000, Player.GeometryLayer);

        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        direction = Player.camera.transform.rotation * direction;
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
        float currentDirection = Vector3.Dot(direction.normalized, Player.velocity.normalized);
        float turnSpeed = Mathf.Lerp(0.3f, 0.6f, currentDirection);
        Player.velocity += direction * (Player.Acceleration + turnSpeed) * Time.deltaTime;

        if (Player.velocity.magnitude > Player.maxSpeed)
        {
            Player.velocity = Player.velocity.normalized * Player.maxSpeed;
        }

    }
    private void Decelerate()
    {
        Vector3 decelerationDirection = Player.velocity;
        decelerationDirection.y = 0;
        if (decelerationDirection.magnitude > Player.minimumDeceleration)
        {
            Player.velocity += decelerationDirection.normalized * Player.Deceleration * Time.deltaTime;
        }
        else
        {
            Player.velocity.x = 0;
            Player.velocity.z = 0;
        }

    }
    private void Gravity()
    {
        Vector3 p1 = Player.transform.position + Player.collider.center + Vector3.up * (Player.collider.height / 2 - Player.collider.radius);
        Vector3 p2 = Player.transform.position + Player.collider.center + Vector3.down * (Player.collider.height / 2 - Player.collider.radius);
        if (Physics.CapsuleCast(p1, p2, Player.collider.radius, Vector2.down, Player.skinWidth, Player.GeometryLayer))
        {
            Player.velocity.y = 0;
        }
        else
        {
            Player.velocity += (Vector3.down * Player.gravity * Time.deltaTime);
        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Player.IsPlayerGrounded())
            {
                stateMachine.TransitionTo<PlayerJumpState>();
            }

        }
    }
}
