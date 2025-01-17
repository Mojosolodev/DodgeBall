using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : Humanoid
{
    public bool isSelectedPlayer;
    void Start()
    {
        
        DoAtStart();
        isSelectedPlayer=false;
    }

    void Update()
    {
        //Only the selected player can do the following
        if(isSelectedPlayer==true)
        {
            //Part of the mov't system
            Vector3 move=new Vector3(Input.GetAxis("Vertical"),0,-Input.GetAxis("Horizontal"));
            characterController.Move(move*Time.deltaTime*moveSpeed);

            Picking_TargetMechanic();
            TriggerRestrictions();
            //for player,catch if pressing "g" (dif condition for enemy)
            if(Input.GetKeyDown("g"))
            {
                CatchInteraction();
            }
            SavedNotPicked();
            AfterShooting();
        }
        else{
            //if not selected,stays IDLE or might be eliminated
            PlayerAnim.SetBool("walk",false);
            PlayerAnim.SetBool("strafeL",false);
            PlayerAnim.SetBool("strafeR",false);
            if(CurrentHealth>0)
                PlayerAnim.SetBool("idle",true);
                else
                PlayerAnim.SetBool("idle",false);
            PlayerAnim.SetBool("walkBack",false);
        }
        
        
    }
    //End of Update

    private void TriggerRestrictions()
    {
        if(Input.GetKey("a"))
        {
            PlayerAnim.SetBool("walk",true);
            PlayerAnim.SetBool("strafeL",false);
            PlayerAnim.SetBool("strafeR",false);
            PlayerAnim.SetBool("idle",false);
            PlayerAnim.SetBool("walkBack",false);
        }
        else if(Input.GetKey("s"))
        {
            PlayerAnim.SetBool("walk",false);
            PlayerAnim.SetBool("strafeL",true);
            PlayerAnim.SetBool("strafeR",false);
            PlayerAnim.SetBool("idle",false);
            PlayerAnim.SetBool("walkBack",false);
        }
        else if(Input.GetKey("w"))
        {
            PlayerAnim.SetBool("walk",false);
            PlayerAnim.SetBool("strafeL",false);
            PlayerAnim.SetBool("strafeR",true);
            PlayerAnim.SetBool("idle",false);
            PlayerAnim.SetBool("walkBack",false);
        }
        else if(Input.GetKey("d"))
        {
            PlayerAnim.SetBool("walk",false);
            PlayerAnim.SetBool("strafeL",false);
            PlayerAnim.SetBool("strafeR",false);
            PlayerAnim.SetBool("idle",false);
            PlayerAnim.SetBool("walkBack",true);
        }
        else 
        {
            PlayerAnim.SetBool("walk",false);
            PlayerAnim.SetBool("strafeL",false);
            PlayerAnim.SetBool("strafeR",false);
                if(CurrentHealth>0)
                PlayerAnim.SetBool("idle",true);
                else
                PlayerAnim.SetBool("idle",false);
            PlayerAnim.SetBool("walkBack",false);

        }
    }
}
