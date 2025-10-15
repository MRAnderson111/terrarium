using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackGardenAnt_AnimDemo : MonoBehaviour
{
    Animator anim;
    bool walkForward;
    bool walkBackward;
    bool inAir;
    bool isDead;
    public Button WF;
    public Button WB;
    public Button Id;
    public Button IdL;
    public Button A1;
    public Button A2;
    public Button A3;
    public Button J;
    public Button TD;
    public Slider DP;
    public Toggle Dead;
    public Toggle Air;


    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        
        WF.onClick.AddListener(WalkForward);
        WB.onClick.AddListener(WalkBackward);
        Id.onClick.AddListener(Idle);
        IdL.onClick.AddListener(IdleLookaround);
        A1.onClick.AddListener(Attack1);
        A2.onClick.AddListener(Attack2);
        A3.onClick.AddListener(Attack3);
        J.onClick.AddListener(Jump);
        TD.onClick.AddListener(TakeDamage);
        Air.onValueChanged.AddListener(delegate
        {
            isinAir(Air);
        });
        Dead.onValueChanged.AddListener(delegate
        {
            DeadState(Dead);
        });

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("damagePosition", DP.value);
        if (walkForward == true)
            anim.SetBool("walkForward", true);
        else
            anim.SetBool("walkForward", false);

        if (walkBackward == true)
            anim.SetBool("walkBackward", true);
        else
            anim.SetBool("walkBackward", false);

        if (inAir == true)
            anim.SetBool("isInAir", true);
        else
            anim.SetBool("isInAir", false);

        if (isDead == true)
            anim.SetBool("isDead", true);
        else
            anim.SetBool("isDead", false);
    }
    void WalkForward()
    {
        walkForward = !walkForward;
        walkBackward = false;

    }
    void WalkBackward()
    {
        walkBackward = !walkBackward;
        walkForward = false;
    }
    void Idle()
    {
        walkBackward = false;
        walkForward = false;
    }
    void IdleLookaround()
    {
        anim.SetTrigger("lookaround");

    }
    void Attack1()
    {
        anim.SetInteger("attackType", 0);
        anim.SetTrigger("attack");

    }
    void Attack2()
    {
        anim.SetInteger("attackType", 1);
        anim.SetTrigger("attack");

    }
    void Attack3()
    {
        anim.SetInteger("attackType", 2);
        anim.SetTrigger("attack");

    }
    void Jump()
    {
        anim.SetTrigger("jump");
        
    }
    void TakeDamage()
    {
        anim.SetTrigger("takeDamage");
    }
    void isinAir(Toggle change)
    {
        inAir = !inAir;
    }

    void DeadState(Toggle change)
    {
        isDead = !isDead;
    }
}
