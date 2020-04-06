using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerBaseState : State
{
    private PlayerController player;
    public PlayerController Player => player = player ?? (PlayerController)owner;
}
