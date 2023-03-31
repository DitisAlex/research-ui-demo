using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour, IHealth
{
    // Start is called before the first frame update
    [SerializeField] [Tooltip("The maximum amount of health of the object")]
    private float maxHealth = 100f;

    public UnityEvent<float> onHpChangedEvent = new();
    private float m_Health;

    //Property
    private float health
    {
        get => m_Health;
        set
        {
            if (gameObject.tag.Equals("Player"))
            {
                GameManager.ShowHitScreen(value - m_Health, 0.1f);
            }

            m_Health = Mathf.Clamp(value, 0, 100);
            
            //The UIManager is listening to this event
            onHpChangedEvent.Invoke(m_Health);

            if (health == 0)
            {
                //TODO: Replace with event so all objects can implement this script
                PlayerManager.Die();
            }
        }
    }

    private void Start()
    {
        m_Health = maxHealth;
        onHpChangedEvent.Invoke(m_Health);
    }

    public float GetCurrentHealth()
    {
        return health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void Heal(float amount)
    {
        health += amount;
    }

    public void ResetHealth()
    {
        health = maxHealth;
    }
}