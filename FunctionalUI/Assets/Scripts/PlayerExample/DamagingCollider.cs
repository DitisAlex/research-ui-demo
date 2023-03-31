using UnityEngine;

public class CollisionDamageTimer : MonoBehaviour
{
    // Time before damage is taken, 1 second default
    [SerializeField] private float timeThreshold = 1f;
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private bool onlyPlayer = true;

    // The amount of time the collision is happening
    private float _timeColliding;

    private void OnTriggerStay(Collider collision)
    {
        if ((!onlyPlayer || !collision.tag.Equals("Player")) &&
            (onlyPlayer || collision.GetComponent<HealthSystem>() == null)) return;

        // If the time is below the threshold, add the delta time
        if (_timeColliding < timeThreshold)
        {
            _timeColliding += Time.deltaTime;
        }
        else //If time is over threshold, do damage
        {
            //TODO: check if collider has HealthSystem
            // Time is over threshold, the object with the HealthSystem-component will take damage.
            collision.GetComponent<HealthSystem>().TakeDamage(damagePerTick);

            // Reset timer
            _timeColliding = 0f;
        }
    }
}