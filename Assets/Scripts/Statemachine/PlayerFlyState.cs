using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/FlyState")]
public class PlayerFlyState : PlayerBaseState
{
    public float Speed;
    public float gravity = 0.5f;

    public override void Enter()
    {
        Debug.Log("flystate");
    }

    public override void Run()
    {
        Gravity();
        StaticFunctions.AirResistance(Player.velocity);
        Vector3 movement = Player.CollisionDetection();
        Player.transform.position += movement * Time.deltaTime;
        if (Player.IsPlayerGrounded())
        {
            stateMachine.TransitionTo<PlayerMoveState>();
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
            Player.velocity += (Vector3.down * gravity * Time.deltaTime);
        }
    }
}
