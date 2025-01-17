using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody Ballrb;
    public float factor=2f;
    public bool isHarmful=false;
    public float NotHarmfulWait=0.5f;
    public bool _onEnemyFloor;

    void Start()
    {
        Ballrb=GetComponent<Rigidbody>();
    }
    void Update()
    {
        //when picked by anyOne,collider is disabled,use as consition
        if(GetComponent<SphereCollider>().enabled==false && isHarmful==false)
        {
            _onEnemyFloor=false;
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //ball falls after hitting any object
        Ballrb.useGravity=true;
        Ballrb.linearVelocity/=factor;
        Invoke("NotHarmful",NotHarmfulWait);
    }
    private void OnCollisionStay(Collision other)
    {
        if(other.gameObject.CompareTag("EnemyFloor"))
        {
            _onEnemyFloor=true;
        }
        else if(!other.gameObject.CompareTag("EnemyFloor"))
        {
            _onEnemyFloor=false;
        }
    }
    
    private void NotHarmful()
    {
        isHarmful=false;
    }
}
