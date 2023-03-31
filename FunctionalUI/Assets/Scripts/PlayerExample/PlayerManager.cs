using System;
using PlayerExample;
using UnityEngine;

[RequireComponent(typeof(HealthSystem), typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    private IHealth m_HealthSystem;
    private PlayerMovement m_PlayerMovement;
    private InventoryController m_InventoryController;
    private VelocityAnimator m_VelocityAnimator;
    private ParticleSystem m_ParticleSystem;


    // Start is called before the first frame update
    private void Awake()
    {
        _instance = this;
        m_PlayerMovement = GetComponent<PlayerMovement>();
        m_HealthSystem = GetComponent<IHealth>();
        m_VelocityAnimator = GetComponent<VelocityAnimator>();
        m_ParticleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    private void Update()
    {
        m_PlayerMovement.HandleSprint();
        m_PlayerMovement.MovePlayer();
        m_PlayerMovement.RotatePlayer();
        m_PlayerMovement.HandleGravity();
        m_VelocityAnimator.HandleAnimation();

        if (Input.GetKeyDown(KeyCode.P))
        {
            InventoryController.ChangeInventory("123456789-ABCDEFGH", InventoryController.InventoryChangeType.Pickup);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            InventoryController.ChangeInventory("A1B2C3D4E5", InventoryController.InventoryChangeType.Pickup);
        }

        if (transform.position.y < -25) Die();
    }

    public static bool Heal(float? amount)
    {
        var currentHealth = _instance.m_HealthSystem.GetCurrentHealth();
        if (Math.Abs(currentHealth - 100) > 0.1)
        {
            var healAmount = Mathf.Clamp(100 - currentHealth, 0, amount.GetValueOrDefault(10)); 
            _instance.m_HealthSystem.Heal(amount.GetValueOrDefault(10));

            PlayHealParticleEffect(healAmount);
            
            return true;
        }

        return false;
    }

    private static void PlayHealParticleEffect(float healAmount)
    {
        //Play heal particles
        _instance.m_ParticleSystem.gameObject.SetActive(true);
        var particleSystemEmission = _instance.m_ParticleSystem.emission;
        particleSystemEmission.rateOverTime = new ParticleSystem.MinMaxCurve(healAmount);
        _instance.m_ParticleSystem.Play();
    }

    public static bool RestoreStamina(float? amount)
    {
        if (Math.Abs(_instance.m_PlayerMovement.GetCurrentStamina() - 100) > 0.1)
        {
            _instance.m_PlayerMovement.RestoreStamina(amount.GetValueOrDefault(25));
            return true;
        }
        return false;
    }

    public static void Die()
    {
        GameManager.DisplayMessage("You are dead", 3);
        _instance.m_PlayerMovement.RespawnOnPosition(new Vector3(0, 5, 0));
        _instance.m_PlayerMovement.ResetStamina();
        _instance.m_HealthSystem.ResetHealth();
    }

    public void AddItemToInventory(string guid)
    {
        if (guid.Equals("COINGUID"))
        {
            GameManager.AddCoin();
            return;
        }
        InventoryController.ChangeInventory(guid, InventoryController.InventoryChangeType.Pickup);
    }
}