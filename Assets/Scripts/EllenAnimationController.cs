using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllenAnimationController : MonoBehaviour
{
    Animator animator;
    private Player ellen;
    private int walkingHash, jumpingHash, deathHash, attackHash;
    private bool jumpAnimationRunning = false;

    // Start is called before the first frame update
    void Start(){
        animator = GetComponent<Animator>();
        walkingHash = Animator.StringToHash("isWalking");
        jumpingHash = Animator.StringToHash("isJumping");
        deathHash = Animator.StringToHash("isDying");
        attackHash = Animator.StringToHash("isAttacking");

        ellen = GetComponent<Player>(); // TODO: Make sure right instance of player is used here
    }

    // Update is called once per frame
    void Update(){
        if(jumpAnimationRunning)
            return;

        animator.transform.localScale = ellen.GetScale();   // Would handle flipping the animation when needed
        animator.SetBool(walkingHash, ellen.IsWalking());

        animator.SetBool(jumpingHash, ellen.IsJumping());
    }

    // Animation event for start of a jump
    // Used to stop walking/running
    public void JumpStartEvent(){
        jumpAnimationRunning = true;
    }

    // Animation event for end of a jump
    // Used to resume walking/running
    // TODO: Check why the event is not triggered!
    public void JumpEndEvent(){
        jumpAnimationRunning = false;
    }

    // // TODO:
    // // Animation event for start of an attack
    // // Used to stop walking/running
    // public void AttackStartEvent(){
    //     ellen.SetMoveSpeed(0.0f);
    // }

    // // Animation event for end of an attack
    // // Used to resume walking/running
    // public void AttackEndEvent(){
    //     ellen.SetDefaultMoveSpeed();
    // }
}
