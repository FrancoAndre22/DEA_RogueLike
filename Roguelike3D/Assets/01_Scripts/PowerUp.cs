using UnityEngine;

public enum PowerUpType { timeBtwShoot, damage, hp, shield, speed }

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    public float powerUpValue;  // How much the stat will increase
    public float duration = 10f;  // How long the buff lasts (set 0 for permanent)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerStats = other.GetComponent<Player>();
            if (playerStats != null)
            {
                ApplyPowerUp(playerStats);
            }
            Destroy(gameObject);  // Destroy the power-up after it has been collected
        }
    }

    void ApplyPowerUp(Player playerStats)
    {
        switch (powerUpType)
        {
            case PowerUpType.timeBtwShoot:
                playerStats.IncreaseAttackSpeed(powerUpValue, duration);
                break;
            case PowerUpType.damage:
                playerStats.IncreaseDamage(powerUpValue, duration);
                break;
            case PowerUpType.hp:
                playerStats.IncreaseHealth(powerUpValue);
                break;
            case PowerUpType.shield:
                playerStats.IncreaseShield(powerUpValue, duration);
                break;
            case PowerUpType.speed:
                playerStats.IncreaseMovementSpeed(powerUpValue, duration);
                break;
        }
    }
}
