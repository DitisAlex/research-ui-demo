public interface IHealth
{
    float GetCurrentHealth();
    void TakeDamage(float damage);
    void Heal(float amount);
    void ResetHealth();
}