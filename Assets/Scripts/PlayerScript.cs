using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{ 
    public enum AttackMode
    {
        Rock, Flow
    }

    private AttackMode attackMode;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeAttackMode(AttackMode t_mode)
    {
        attackMode = t_mode;
    }

    public AttackMode getAttackMode()
    {
        return attackMode;
    }

}
