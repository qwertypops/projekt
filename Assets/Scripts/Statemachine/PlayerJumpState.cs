using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/JumpState")]
public class PlayerJumpState : PlayerBaseState
{
    public float jumpHeight = 1;
    // Start is called before the first frame update

    public override void Enter()
    {
        Debug.Log("jumping");
        Player.velocity.y += jumpHeight;
    }
    // Update is called once per frame
    public override void Run()
    {
        Vector3 movement = Player.CollisionDetection();
        Player.transform.position += movement * Time.deltaTime;
        stateMachine.TransitionTo<PlayerFlyState>();
    }
}
