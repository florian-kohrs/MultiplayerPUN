using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour, IHealth
{

    protected List<IDeathListener> deathListeners = new List<IDeathListener>();

    public int maxHealth = 100;

    protected virtual bool IsImmune => false;
    protected float timeSinceDamage;
    public int currentHealth;

    private void NotifyListenersOnDeath()
    {
        deathListeners.ForEach(deathListener => deathListener.OnDeath(this));
    }

    private void OnDeath()
    {
        NotifyListenersOnDeath();
        OnEntityDied();
    }

    protected virtual void OnEntityDied() { }

    protected virtual void OnResetHealth() { }

    protected void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    protected bool IsDead => currentHealth <= 0;

    public void ResetWithMaxHealth(int maxHealth)
    {
        SetMaxHealth(maxHealth);
        currentHealth = this.maxHealth;
        OnResetHealth();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void AddDeathListener(IDeathListener listener)
    {
        deathListeners.Add(listener);
    }

    public bool TakeDamage(int damage)
    {
        bool died = false;
        if (!IsDead && !IsImmune)
        {
            currentHealth -= damage;
            timeSinceDamage = 0f;
            if (currentHealth <= 0)
            {
                OnDeath();
                currentHealth = 0;
                died = true;
            }
            else
            {
                OnTakeNonLethalDamage();
            }
        }
        return died;
    }

    protected virtual void OnTakeNonLethalDamage() { }

    public void HealDamage(int damage)
    {
        if (!IsDead)
        {
            currentHealth += damage;
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }

}
