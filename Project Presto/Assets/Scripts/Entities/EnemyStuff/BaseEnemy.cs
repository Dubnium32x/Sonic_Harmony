using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//extend bosses from me
public class BaseEnemy : MonoBehaviour
{
    public float moveLeftAmnt = 5.0f;

    public float moveRightAmnt = 5.0f;

    public int hitpoints = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public BaseEnemy()
    {

    }

    public BaseEnemy(int setHitpoints)
    {
        hitpoints = setHitpoints;
    }

    public void MoveAround()
    {
        //move left then attack
        //move right then attack
    }
    public void AttackPlayer()
    {
        //extend in subclasses
    }
    public void TauntPlayer()
    {
        //override in extending classes
    }

    public void HurtEnemy()
    {
        if (hitpoints <= 0)
        {
            Die();
        }
        hitpoints -= 1;
    }

    public void Die()
    {
        //Active Killanimation then kill object
        Destroy(gameObject.transform.parent.gameObject);
    }
}
