using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;

    public void Initialize()
    {
        // default state
        ChangeState(new PatrolState());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (activeState != null)
        {
            activeState.Perform();
        }
    }

    public void ChangeState(BaseState newState)
    {
        if (activeState != null)
        {
            activeState.Exit(); // clean-up
        }

        // change to the new state
        activeState = newState;

        if (activeState != null)
        {
            // set-up the new state
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<ArmedEnemy>();
            activeState.Enter();
        }
    }
}
