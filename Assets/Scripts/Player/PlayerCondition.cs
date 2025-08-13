using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}


public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }
    Condition hunger { get { return uiCondition.hunger; } }

    public float noHungerHealthDecay;

    public event Action onTakeDamage;

    public float currentHealth;
    public float maxHealth = 100;
    public float currentHunger;
    public float maxHunger = 300;

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnHungerChanged;

    // Update is called once per frame

    private void Awake()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnHungerChanged?.Invoke(currentHunger, maxHunger);
    }
    void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (hunger.curValue <= 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public IEnumerator ApplyConsumableEffectsOverTime(ItemDataConSumable[] effects, int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            foreach (ItemDataConSumable effect in effects)
            {
                switch (effect.type)
                {
                    case ConsumableType.Health:
                        // health 스크립트의 Add 함수 호출
                        health.Add(effect.value);
                        break;

                    case ConsumableType.Hunger:
                        // hunger 스크립트의 Add 함수 호출
                        hunger.Add(effect.value);
                        break;
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }




    public void Health(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    void Die()
    {
        Debug.Log("Player has died.");
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }
}
