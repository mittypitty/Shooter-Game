using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FPSPlayeAnimation : MonoBehaviour
{
    private Animator anim;
    private string move = "Move";
    private string velocity_y = "VelocityY";
    private string crouch = "Crouch";
    private string crouch_walk = "CrouchWalk";

    private string standShoot = "StandShoot";
    private string crouchShoot = "CrouchShoot";
    private string reload = "Reload";
    void Awake() { anim = GetComponent<Animator>(); }


    public void Movement(float magnitude)
    {
        anim.SetFloat(move, magnitude);
    }

    public void PlayerJump(float velocity)
    {
        anim.SetFloat (velocity_y, velocity);
    }

    public void PlayerCrouch(bool isCrouching)
    {
        anim.SetBool(crouch, isCrouching);
    }

    public void PlayerCrouchWalk(float magnitude)
    {
        anim.SetFloat(crouch_walk, magnitude);
    }

    public void Shoot(bool isStanding)
    {
        if(isStanding)
        {
            anim.SetTrigger(standShoot);
        }
        else
        {
            anim.SetTrigger(crouchShoot);
        }
    }
    public void ReloadGun()
    {
        anim.SetTrigger(reload);
    }
}
