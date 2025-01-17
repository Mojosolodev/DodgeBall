using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class Humanoid : MonoBehaviour
{
    protected static float CollisionCoolDown=0.3f;
    protected static float moveSpeed=2f;
    public static Camera mainCam;
    protected static float WaitMoveTime=1f;
    public  static GameObject FoundBall;//make sure static but visible
    public GameObject HitVFX;//make sure static but visible
    protected static float rotationSpeed=5f;
    protected static float maxHealth=40f;
    protected static float WaitTimeBeforeRecatch=3f;
    protected static GameObject TeamManager;
    
    //end of static vars

    //start of variables for instances of children classes
    protected Animator PlayerAnim;
    [SerializeField] protected Transform HandPos;
    [SerializeField] public GameObject PickedBall;
    public float shootForce=1f;
    protected  Rigidbody Ballrb;
    protected CharacterController characterController;
    [SerializeField] protected GameObject Target;
    [SerializeField] protected GameObject BallSave;
    public float CurrentHealth;//can be affected later if add Power-Ups
    public float HitDamage=10f;//can be affected by state of ball
    public HealthBar healthbarScript;
    public float waitTimeBeforeSearchOpponent=2f;//used only by enemy
    protected bool _alreadyShot;
    protected bool _alreadyCatching=false;
    

    void Start()
    {
        FoundBall=GameObject.FindGameObjectWithTag("ball");
        mainCam=Camera.main;
        Debug.Log("Humanoid script");
    }
    
    void Update()
    {

    }
    protected void DoAtStart()
    {
        PlayerAnim=gameObject.GetComponent<Animator>();
        characterController=GetComponent<CharacterController>();
        Target=null;
        CurrentHealth=maxHealth;
        Debug.Log(gameObject);
        _alreadyShot=false;
        _alreadyCatching=false;
        TeamManager=GameObject.Find("TeamManagerEmpty");
        healthbarScript.UpdateHealthBar(maxHealth,CurrentHealth);
    }
    
    protected void Picking_TargetMechanic()
    {
        if(PickedBall)
        {
            Ballrb=PickedBall.GetComponent<Rigidbody>();
            Ballrb.linearVelocity=new Vector3(0f,0f,0f);
            Ballrb.angularVelocity=new Vector3(0f,0f,0f);
            PickedBall.GetComponent<SphereCollider>().enabled=false;
            PickedBall.transform.position=HandPos.position;
            PickedBall.transform.rotation=HandPos.rotation;
            PickedBall.transform.SetParent(HandPos.transform);
            
            //If player,shooting is enbaled by user Input
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray= mainCam.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray,out RaycastHit hit))
                {
                        if(hit.transform.gameObject.CompareTag("enemy"))
                        {
                            Target=hit.transform.gameObject;
                            //Give the target a random catch value
                            if(Random.value>=0.5)
                            {
                                Target.GetComponent<EnemyScript>().randomCatch=true;
                            }
                            else{
                                Target.GetComponent<EnemyScript>().randomCatch=false;
                            }
                        }
                }
            }
            //if this is enemy,shooting enabled by this below
            if(gameObject.CompareTag("enemy"))
            {
                Invoke("SetPlayerTarget",waitTimeBeforeSearchOpponent);
            }
            if(Target)
            {
                PlayerAnim.SetLayerWeight(1,0f);
                Vector3 TargetPos=Target.transform.position;
                TargetPos.y=2f;
                transform.LookAt(TargetPos);
                //disable players collision bound before throwing
                GetComponent<CapsuleCollider>().enabled=false;
                if(_alreadyShot==false)
                {//To prevent shooting twice
                    PlayerAnim.SetTrigger("Throw");
                    _alreadyShot=true;
                }
                
                //disable character controller so can't move when start throw
                characterController.enabled=false;
            }
            
            //if picked ball but not yet shooting a target,look forward
            if(!Target)
            {
                Vector3 frontDir=new Vector3(0,0,0);//random assignation to remove error
                //check if it is a player or enemy,to decide if turn forward or backward
                if(gameObject.CompareTag("Player"))
                {
                    frontDir=Vector3.forward;
                }
                else if(gameObject.CompareTag("enemy"))
                {
                    frontDir=Vector3.forward*-1;
                }
                frontDir.y=0;
                Quaternion targetRot=Quaternion.LookRotation(frontDir);
                transform.rotation=Quaternion.Slerp(transform.rotation,targetRot,rotationSpeed*Time.deltaTime);
            }
            
        }
        //to manage a rotation error somewhere
        if(PickedBall==null)
        {
            Target=null;
        }
        
    }
    //End of Update


    public void CatchInteraction()
    {
        if(!PickedBall)
        {
            //catches the ball so doesn't get hurt(in OnCollisionEnter below)
            if(_alreadyCatching==false)
            {//manage double catching error
                PlayerAnim.SetTrigger("catch");
                _alreadyCatching=true;
                //some time after can catch again
                Invoke("CanCatch",WaitTimeBeforeRecatch);
            }
            
        }
        
    }
    protected void CanCatch()
    {
        _alreadyCatching=false;
    }
    

    public void SavedNotPicked()
    {
        if(BallSave && !PickedBall)
        {
            //turn towards ball
            Vector3 targetDir=FoundBall.transform.position-transform.position;
            targetDir.y=0;
            Quaternion targetRot=Quaternion.LookRotation(targetDir);
            transform.rotation=Quaternion.Slerp(transform.rotation,targetRot,rotationSpeed*Time.deltaTime);
        }
    }

    public void AfterShooting()
    {
        if(!BallSave && !PickedBall && !Target)
        {
            Vector3 frontDir=new Vector3(0,0,0);//random assignation to remove error
            //check if it is a player or enemy,to decide if turn forward or backward
            if(gameObject.CompareTag("Player"))
            {
                frontDir=Vector3.forward;
            }
            else if(gameObject.CompareTag("enemy"))
            {
                frontDir=Vector3.forward*-1;
            }
            frontDir.y=0;
            Quaternion targetRot=Quaternion.LookRotation(frontDir);
            transform.rotation=Quaternion.Slerp(transform.rotation,targetRot,rotationSpeed*Time.deltaTime);
        }
    }
    public void Throw()
    {
        Ballrb.useGravity=false;
        FoundBall.GetComponent<BallScript>().isHarmful=true;
        PickedBall.transform.SetParent(null,true);
        Ballrb.AddForce(transform.forward*shootForce);
        PickedBall.GetComponent<SphereCollider>().enabled=true;
        PickedBall=null;
        Target=null;
        BallSave=null;
        StartCoroutine(WaitThrow());
        StartCoroutine(WaitMove());
    }
    
    IEnumerator WaitThrow()
    {
        //after throwing wait before re-enabling player collision bound
        yield return new WaitForSeconds(CollisionCoolDown);
        GetComponent<CapsuleCollider>().enabled=true;
        
        
    }
    IEnumerator WaitMove()
    {
        yield return new WaitForSeconds(WaitMoveTime);
        PlayerAnim.SetLayerWeight(1,Mathf.Lerp(PlayerAnim.GetLayerWeight(1),1f,1.5f));
        characterController.enabled=true;
        _alreadyShot=false;//reset this,so sometime after throwing,can shoot again (when necessary)
    }
    private void CanMove()
    {
        characterController.enabled=true;
        PlayerAnim.SetLayerWeight(1,Mathf.Lerp(PlayerAnim.GetLayerWeight(1),1f,1f));
    }
    public void Pick()
    {
        PickedBall=BallSave;//set the pickedBall reference
    }
    
    protected void SetPlayerTarget()
    {
        //check if not yet having a target,to avoid changing targets several times
        if(Target==null)
        {
            int randomTargetIndex=Random.Range(0,3);
            if(PickedBall && TeamManager.GetComponent<TeamManager>().PlayerTeam[randomTargetIndex]==null)
            {
                //Reverifying if pickedBall Still exists(because method is called with a delay)
                
                if(TeamManager!=null)
                {
                    //find index of player who hasn't lost yet in playerTeam                
                    do{
                    randomTargetIndex=Random.Range(0,3);
                    Debug.Log("random index:"+randomTargetIndex);
                    }while(TeamManager.GetComponent<TeamManager>().PlayerTeam[randomTargetIndex]==null);
                    
                }
                else{
                    Debug.Log("TeamManager reference is not set");
                }
            }
            Target=TeamManager.GetComponent<TeamManager>().PlayerTeam[randomTargetIndex];
        }
    }

    protected void OnCollisionEnter(Collision other)
    {
        //if ball collides with player
        if(other.gameObject.CompareTag("ball"))
        {
            //check if ball cannot hurt player
            if(FoundBall.GetComponent<BallScript>().isHarmful==false)
            {
                
                //picks the ball
                if(gameObject.CompareTag("Player"))
                {
                    //just check if not selected already,to not spawn vfx twice
                    if(gameObject.GetComponent<PlayerScript>().isSelectedPlayer==false)
                    {
                        //set this player active(selected) and unselect all the others(Can't pick if not selected)
                        //first unselect all the others,before selecting this player
                        GameObject.Find("TeamManagerEmpty").GetComponent<TeamManager>().UnselectAllPlayers();
                        gameObject.GetComponent<PlayerScript>().isSelectedPlayer=true;
                        Destroy(Instantiate(GameObject.Find("TeamManagerEmpty").GetComponent<TeamManager>().selectionVFX,transform.position,transform.rotation),1f);
                    }
                }
                characterController.enabled=false;//stops moving
                PlayerAnim.SetLayerWeight(1,Mathf.Lerp(PlayerAnim.GetLayerWeight(1),0f,1f));
                PlayerAnim.SetTrigger("pick");
                GetComponent<CapsuleCollider>().enabled=false;
                BallSave=other.gameObject;//save the ref to ball for after collision
                BallSave.GetComponent<Rigidbody>().linearVelocity=new Vector3(0f,0f,0f);
                BallSave.GetComponent<Rigidbody>().angularVelocity=new Vector3(0f,0f,0f);
                Invoke("CanMove",1f);
                
            }
            //check if ball can hurt
            else if(FoundBall.GetComponent<BallScript>().isHarmful==true)
            {
                //if playing catch anim,catch the ball
                if(PlayerAnim.GetCurrentAnimatorStateInfo(0).IsName("catch"))
                {
                    //catches the ball so doesn't get hurt
                    PickedBall=other.gameObject;
                }
                //if not playing catch animation,gets hurt
                else if(!PlayerAnim.GetCurrentAnimatorStateInfo(0).IsName("catch"))
                {
                    //gets hurt
                    
                    ContactPoint contact=other.contacts[0];
                    Instantiate(HitVFX,contact.point,FoundBall.transform.rotation);
                    //reduce Health
                    CurrentHealth-=HitDamage;
                    healthbarScript.UpdateHealthBar(maxHealth,CurrentHealth);
                    if(CurrentHealth>0)
                    {//plays hit animation
                        PlayerAnim.SetTrigger("Hit");
                    }
                    else if(CurrentHealth<=0)
                    {
                        //dead   so disable some stuff
                        PlayerAnim.SetTrigger("Dead");
                        characterController.enabled=false;  //(only player,bcs enemy moves with NavAgent)
                        GetComponent<CapsuleCollider>().enabled=false;
                        PlayerAnim.SetLayerWeight(1,0f);
                    }
                }
                
            }
            
        }
  
       
    }
}
