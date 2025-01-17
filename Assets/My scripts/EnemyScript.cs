using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : Humanoid
{
    public float ballCatchDistance=2f;
    public bool randomCatch;
    public NavMeshAgent enemyAgent;
    public Camera cam;
    public bool isClosestToBall;
    void Start()
    {
        DoAtStart();
        enemyAgent=GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //Manage the mov't system for Enemy
        Picking_TargetMechanic();
        if(randomCatch==true)
        {
            //random catch recieved is true,so can try to catch
            if(Vector3.Distance(gameObject.transform.position,FoundBall.transform.position)<ballCatchDistance && FoundBall.GetComponent<BallScript>().isHarmful==true)
            {
                CatchInteraction();
                randomCatch=false;
            }
        }
        SavedNotPicked();
        AfterShooting();

        
        if(FoundBall.GetComponent<BallScript>()._onEnemyFloor==true)
        {
            if(isClosestToBall==true)
            {
                //make enemy walk towards ball
                //need to disable character controller first
                GetComponent<CharacterController>().enabled=false;
                enemyAgent.SetDestination(FoundBall.transform.position);
                PlayerAnim.SetBool("walk",true);
                PlayerAnim.SetBool("idle",false);
            }
        }
        else 
        {
            //re-enable character controller 
            GetComponent<CharacterController>().enabled=true;
            PlayerAnim.SetBool("walk",false);
            PlayerAnim.SetBool("idle",true);
        }
        
        if(CurrentHealth<=0)
        {
            //dead
            //can't follow the ball anymore
            //capsule collider already disable in Humanoid
            enemyAgent.enabled=false;
        }
        
    }//End of Update

    

}
