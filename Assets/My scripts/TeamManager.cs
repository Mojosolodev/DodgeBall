using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject[] Players=new GameObject[3]; 
    private GameObject[] Enemies=new GameObject[3]; 
    public List<GameObject> PlayerTeam;
    public List<GameObject> EnemyTeam;
    private Camera mainCam;
    public GameObject selectionVFX;
    private float smallerEnemyDistFromBall;
    private GameObject foundBall;
    
    void Start()
    {
        //put players and enemies in array,then insert in list
        Players=GameObject.FindGameObjectsWithTag("Player");
        PlayerTeam=new List<GameObject>(Players);
        Enemies=GameObject.FindGameObjectsWithTag("enemy");
        EnemyTeam=new List<GameObject>(Enemies);

        mainCam=Camera.main;
        foundBall=GameObject.FindGameObjectWithTag("ball");
    }

    // Update is called once per frame
    void Update()
    {
        //manage selection of player to control
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray= mainCam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit hit))
            {
                //if no player has picked the ball,I can select another player to control
                bool TeamAlreadyPicked=false;
                foreach(GameObject p in PlayerTeam)
                {
                    if(p.GetComponent<PlayerScript>().PickedBall!=null)
                    {
                        TeamAlreadyPicked=true;
                    }
                }
                if(TeamAlreadyPicked==false)
                {
                    if(hit.transform.gameObject.CompareTag("Player"))
                    {
                        foreach(GameObject p in PlayerTeam)
                        {
                            if(p==hit.transform.gameObject)
                            {
                                p.GetComponent<PlayerScript>().isSelectedPlayer=true;
                                Destroy(Instantiate(selectionVFX,p.transform.position,p.transform.rotation),1f);
                            }
                            else
                                p.GetComponent<PlayerScript>().isSelectedPlayer=false;
                        }
                    }
                }
                
            }
        }
        
        foreach(GameObject e in EnemyTeam)
        {
            //for enemy closer to ball ,set bool
            if(e==FindClosestEnemy())
            {
                e.GetComponent<EnemyScript>().isClosestToBall=true;
                continue;
            }
            e.GetComponent<EnemyScript>().isClosestToBall=false;
            
        }
        //remove reference of eliminated players in Team
        foreach(GameObject p in PlayerTeam)
        {
            if(p.GetComponent<PlayerScript>().CurrentHealth<=0)
            {
                PlayerTeam.Remove(p);
            }
        }
        //remove reference of eliminated Enemies in Team
        foreach(GameObject e in EnemyTeam)
        {
            if(e.GetComponent<EnemyScript>().CurrentHealth<=0)
            {
                EnemyTeam.Remove(e);
            }
        }
        
    }//end of Update

    public void UnselectAllPlayers()
    {
       foreach(GameObject p in PlayerTeam)
       {
            p.GetComponent<PlayerScript>().isSelectedPlayer=false;
       } 
    }
    public GameObject FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (GameObject obj in EnemyTeam)
        {
            float distance = Vector3.Distance(foundBall.transform.position, obj.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }
}
