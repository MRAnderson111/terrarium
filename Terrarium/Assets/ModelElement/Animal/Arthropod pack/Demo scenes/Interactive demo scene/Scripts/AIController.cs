using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    public GameObject target;
    public float walkingSpeed;
    public float runningSpeed;
    public float damageAmount = 5f;
    public AudioSource[] damageSound;
    public AudioSource insectWalk;
    NavMeshAgent agent;
	Animator anim;

    enum STATE {IDLE, WANDER, ATTACK, CHASE, DEAD};
    STATE state= STATE.IDLE;

	void Start () 
    {
		anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
	}

    void TurnOffTriggers()
    {
        anim.SetBool("isWalking",false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isDead", false);
    }

    float DistanceToPlayer()
    {
        if(GameStats.gameOver) return Mathf.Infinity;
        return Vector3.Distance(target.transform.position,this.transform.position);
    }
    bool CanSeePlayer()
    {
        if(DistanceToPlayer() < 10)
            return true;
        return false;
    }

    bool ForgetPlayer()
    {
        if (DistanceToPlayer() > 20)
            return true;
        return false;
    }

    public void KillCritter()
    {
        TurnOffTriggers();
        anim.SetBool("isDead",true);
        state = STATE.DEAD;
    }

    void PlayDamageAudio()
    {
        AudioSource audioSource = new AudioSource();
        int n = Random.Range(1, damageSound.Length);

        audioSource = damageSound[n];
        audioSource.Play();
        damageSound[n] = damageSound[0];
        damageSound[0] = audioSource;
    }

    public void DamagePlayer()
    {
        if(target !=null)
        {
        target.GetComponent<FPController>().TakeHit(damageAmount);
        PlayDamageAudio();
        }
    }
	
	void Update () 
	{
        if (target == null && GameStats.gameOver== false)
        {
            target = GameObject.FindWithTag("Player");
            return;
        }
        switch(state)
        {
            case STATE.IDLE:
                if(CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0,5000) < 5)
                state = STATE.WANDER;
                insectWalk.Stop();
            break;
            case STATE.WANDER:
                if(!agent.hasPath)
                {
                float newX = this.transform.position.x + Random.Range(-10,10);
                float newZ = this.transform.position.z + Random.Range(-10,10);
                float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX,0,newZ));
                Vector3 dest = new Vector3(newX,newY,newX);
                agent.SetDestination(dest);
                agent.stoppingDistance = 0;
                TurnOffTriggers();
                agent.speed = walkingSpeed;
                anim.SetBool("isWalking",true);
                insectWalk.Play();
                }
                if (CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0, 5000) < 5)
                {
                   TurnOffTriggers();
                   state = STATE.IDLE;
                   agent.ResetPath();
                }
                break;
            case STATE.CHASE:
                if(GameStats.gameOver)
                {
                    TurnOffTriggers();
                    state = STATE.WANDER;
                    return;
                }
                TurnOffTriggers();
                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 2;
                agent.speed = runningSpeed;
                anim.SetBool("isRunning",true);
                insectWalk.Play();
                if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    state = STATE.ATTACK;
                }

                if (ForgetPlayer())
                {
                    state = STATE.WANDER;
                    agent.ResetPath();
                }
            break;
            case STATE.ATTACK:
                if (GameStats.gameOver)
                {
                    TurnOffTriggers();
                    state = STATE.WANDER;
                    return;
                }
                insectWalk.Stop();
                TurnOffTriggers();
                anim.SetBool("isAttacking",true);
                this.transform.LookAt(target.transform.position);
                if (DistanceToPlayer() > agent.stoppingDistance + 1)
                  state = STATE.CHASE;
                
            break;
            case STATE.DEAD:

                insectWalk.Stop();
            break;
        }
    }
}
