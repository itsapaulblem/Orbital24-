using UnityEngine;
using UnityEngine.SceneManagement; 
public class EnemyHealth : MonoBehaviour
{
     public int maxHealth = 20;
     public HealthBar healthBar; 
    public int currentHealth; 
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene("RewardScene");
        }
    }
}
