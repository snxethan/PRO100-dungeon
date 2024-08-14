using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static float moveSpeed;
    public static int maxHealth = 100;
    public int level = 1;
    public int health = maxHealth;
    public string name = "Player";
    public int speed = 1;
    public int attack = 1;
    public int defense = 1;

    public LayerMask solidObjectsLayer;
    public LayerMask EnemyLayer;

    private bool isMoving;
    private Vector2 input;

    

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");


            if (input != Vector2.zero)
            {
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (isWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
                
            }
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
        CheckForEnemy();

    }

    private bool isWalkable(Vector3 targetPos)
    {
       if(Physics2D.OverlapCircle(targetPos, 0.05f, solidObjectsLayer) != null)
        {
            return false;
        }
       return true;
    }

    private void CheckForEnemy()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.05f, EnemyLayer) != null)
        {
            if (Random.Range(1, 101) <= 10)
            {
                Debug.Log("Enemy Encounter");

            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
            //trigger death stuff
        }
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > 100)
        {
            health = 100;
        }
    }

    public void LevelUp()
    {
        level++;
        maxHealth = Mathf.FloorToInt(maxHealth * 1.1f);
        health = maxHealth;
        attack = Mathf.FloorToInt(attack * 1.1f);
        defense = Mathf.FloorToInt(defense * 1.1f);
        speed = Mathf.FloorToInt(speed * 1.1f);
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetAttack()
    {
        return attack;
    }

    public int GetDefense()
    {
        return defense;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(int newSpeed)
    {
        speed = newSpeed;
    }

    public void SetAttack(int newAttack)
    {
        attack = newAttack;
    }

    public void SetDefense(int newDefense)
    {
        defense = newDefense;
    }




}
