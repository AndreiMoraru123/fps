using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArmedEnemy : Enemy
{
    private StateMachine stateMachine;
    private GameObject player;
    private Vector3 lastKnownPos;
    public GameObject Player { get => player; }
    public Vector3 LastKnownPos { get => lastKnownPos; set => lastKnownPos = value; }

    public Path path;

    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight = 1f;
    public LayerMask detectionLayers;

    [Header("Weapon Values")]
    public Transform gunBarrel;
    [Range(0.1f, 10f)]
    public float fireRate;

    [SerializeField]
    private string currentState;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player");
        // include everything but the weapon layer
        detectionLayers = Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("Weapon"));
    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer()
    {
        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    var ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, sightDistance, detectionLayers))
                    {
                        if (hitInfo.transform.gameObject == player)
                        {
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance, Color.green);
                            lastKnownPos = player.transform.position;
                            return true;
                        }
                        else
                        {
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance, Color.red);
                        }
                    }
                }
            }
        }
        return false;
    }

    public override void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
}
