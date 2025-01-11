
using UnityEngine;

public class YBotEnemy : Enemy
{
    LayerMask detectionLayers;
    public Transform gunBarrel;

    [Header("Vision Settings")]
    public float sightDistance = 18f;
    public float fieldOfView = 85f;
    public float eyeHeight = 1.5f;

    void Start()
    {
        detectionLayers = Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("WeaponRender"));
    }

    public override void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            isDead = true;
            animator.SetTrigger("DIE");

            StartCoroutine(DisableCollider());
            StartCoroutine(DisableAnimator());
            StartCoroutine(DisableGameObject());
        }
        else
        {
            animator.SetTrigger("DAMAGE");
        }
    }

    public bool CanSeeTarget(Transform player)
    {
        if (Vector3.Distance(transform.position, player.position) < sightDistance)
        {
            var targetDirection = player.position - transform.position - (Vector3.up * eyeHeight);
            var angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
            if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
            {
                var ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, sightDistance, detectionLayers))
                {
                    Debug.DrawRay(ray.origin, ray.direction * sightDistance,
                                  hitInfo.transform.gameObject == player.gameObject ? Color.green : Color.red);
                    return hitInfo.transform.gameObject == player.gameObject;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 12.5f);  // attacking // stop attacking

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f);  // start chasing

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f);  // stop chasing
    }
}

