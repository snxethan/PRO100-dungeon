using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100; //the max health for the player
    public float level = 0; //starting level for the player
    private int health = 0; //dynamic health level for the player
    public float moveSpeed; //move speed of the player
    private string name = "Player"; //name of the player
    private int speed = 1; //speed of the player for battle (not movement)
    private int attack = 1; //attack of the player for battle (damage)
    private int defense = 1; //defense of the player for battle (reduces damage taken)

    public LayerMask solidObjectsLayer; //layer for solid objects
    public LayerMask EnemyLayer; //layer for enemies

    private bool isMoving; //is the player moving
    private Vector2 input; //input for the player


    private void Update() // Update is called once per frame
    {
        if (!isMoving) //if the player is not moving
        {
            input.x = Input.GetAxisRaw("Horizontal"); //get the horizontal input
            input.y = Input.GetAxisRaw("Vertical"); //get the vertical input


            if (input != Vector2.zero) //if the input is not zero
            {
                var targetPos = transform.position; //set the target position to the current position
                targetPos.x += input.x; //add the horizontal input to the x position
                targetPos.y += input.y; //add the vertical input to the y position

                if (isWalkable(targetPos)) StartCoroutine(Move(targetPos)); //if the target position is walkable, move the player to the target position
            }
        }
    }

    private IEnumerator Move(Vector3 targetPos) //move the player to the target position
    {
        isMoving = true; //set isMoving to true

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) //while the distance between the target position and the player is greater than epsilon
        {
            //move the player towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); 
            yield return null; //wait for the next frame
        }

        transform.position = targetPos; //set the player's position to the target position

        isMoving = false; //set isMoving to false
        CheckForEnemy(); //check for an enemy
    }

    private bool isWalkable(Vector3 targetPos) //check if the target position is walkable
    {
        // check if the target position is walkable
        if (Physics2D.OverlapCircle(targetPos, 0.05f, solidObjectsLayer) != null) return false; //return false if the target position is not walkable
        return true; //return true if the target position is walkable
    }

    private void CheckForEnemy() //check for an enemy
    {
        // check for enemy encounter 
        if (Physics2D.OverlapCircle(transform.position, 0.05f, EnemyLayer) != null) //if an enemy is detected
            if (Random.Range(1, 101) <= 10) //10% chance of enemy encounter
                Debug.Log("Enemy Encounter"); 
        // LOGIC FOR BATTLE
    }

    public void TakeDamage(int damage) //take damage
    {
        health -= damage; //subtract the damage from the health
        if (health < 0) health = 0; //if the health is less than 0, set it to 0
        //trigger death stuff
    }

    public void Heal(int healAmount) //heal the player
    {
        health += healAmount; //add the heal amount to the health
        if (health > maxHealth) health = maxHealth; //if the health is greater than the max health, set it to the max health
    }

    public void LevelUp()
    {
        level++; //increase the level by 1
        maxHealth = Mathf.FloorToInt(maxHealth * 1.1f); //increase the max health by 10%
        health += maxHealth / 2; //increase the health by half of the max health
        attack = Mathf.FloorToInt(attack * 1.1f); //increase the attack by 10%
        defense = Mathf.FloorToInt(defense * 1.1f); //increase the defense by 10%
        speed = Mathf.FloorToInt(speed * 1.1f); //increase the speed by 10%
    }




    public int GetHealth()
    {
        return health; //return the health
    }

    public float GetLevel()
    {
        return level; //return the level
    }

    public int GetMaxHealth()
    {
        return maxHealth; //return the max health
    }

    public int GetAttack()
    { 
        return attack; //return the attack
    }

    public int GetDefense()
    {
        return defense; //return the defense
    }

    public int GetSpeed()
    {
        return speed; //return the speed
    }

    public void SetSpeed(int newSpeed)
    {
        speed = newSpeed; //set the speed to the new speed
    }

    public void SetAttack(int newAttack)
    {
        attack = newAttack; //set the attack to the new attack
    }

    public void SetDefense(int newDefense)
    {
        defense = newDefense; //set the defense to the new defense
    }

    public string GetName()
    {
        return name; //return the name
    }

    public void SetName(string newName)
    {
        name = newName; //set the name to the new name
    }
}