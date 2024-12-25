using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{

    private float moveTimer;
    private float losePlayerTimer;
    private EnemyWeapon weapon;
    public override void Enter()
    {
        weapon = enemy.GetComponentInChildren<EnemyWeapon>();
        if (weapon == null)
        {
            Debug.LogError("no enemy weapon component");
        }
    }

    public override void Exit()
    {
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;

            enemy.transform.LookAt(enemy.Player.transform);

            if (weapon != null)
            {
                weapon.ShootAtTarget(enemy.Player.transform);
            }

            if (moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                moveTimer = 0;
            }

            enemy.LastKnownPos = enemy.Player.transform.position;
        }
        else // lost sight of player
        {
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > 4f)
            {
                // go to search state
                stateMachine.ChangeState(new SearchState());
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
