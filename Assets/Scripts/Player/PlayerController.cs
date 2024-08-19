using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int level = 1;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private string playerName = "Player";
    [SerializeField] private Inventory inventory; // Reference to the player's inventory

    [SerializeField] private List<ItemBase> fixedItems; // Fixed items that will be assigned in the Unity Editor

    private int health;
    private int speed = 1;
    private int attack = 1;
    private int defense = 1;
    private float experience = 0;

    public int MaxHealth { get => maxHealth; private set => maxHealth = value; }
    public int Level { get => level; private set => level = value; }
    public int Health { get; private set; }
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    public string PlayerName { get => playerName; private set => playerName = value; }
    public int Speed { get => speed; private set => speed = value; }
    public int Attack { get => attack; private set => attack = value; }
    public int Defense { get => defense; private set => defense = value; }


    private bool isMoving;
    private Vector2 input;


    public event Action OnEncountered;
    const float offsety = 0.3f;


    private void Start()
    {
        InitializeStats(level);
        Health = maxHealth;

        if (inventory != null)
        {
            inventory.InitializeInventory(fixedItems); // Pass fixedItems list to the inventory
        }
        else
        {
            Debug.LogError("Inventory is not assigned!");
        }
    }

    private void InitializePlayerInventory()
    {
        inventory.ClearInventory(); // Ensure the inventory is empty before adding items

        // Add fixed items that cannot be removed
        foreach (var item in fixedItems)
        {
            if (item != null)
            {
                inventory.AddFixedItem(item);
            }
            else
            {
                Debug.LogError("One of the fixed items is null.");
            }
        }

        // Additional slots are added automatically if fewer than 4 fixed items exist
    }

    public void HandleUpdate()
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

                if (isWalkable(targetPos)) StartCoroutine(Move(targetPos));
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        //finish this after animator -- on episode 26
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
        
        onMoveOver();
        //CheckForEnemy();
    }

    private void onMoveOver()
    {   
        
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, offsety), 0.2f, GameLayer.i.TriggerableLayers);
        
        foreach (var collider in colliders)
        {
            Debug.Log("OnMoveOver");
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    private bool isWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.05f, GameLayer.i.SolidObjectsLayer) == null;
    }

    private void CheckForEnemy()
    {
        Debug.Log("Checking for enemy encounter...");

        // Adjust the radius if 0.05f is too small
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, 0.5f, GameLayer.i.EnemyLayer);

        if (enemyCollider != null)
        {
            Debug.Log("Enemy Layer Detected");
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                OnEncountered();
            }
            else
            {
                Debug.Log("No encounter triggered.");
            }
        }
    }

    public void TriggerBattle()
    {
        Debug.Log("Triggering Battle...");
        if (BattleSystem.Instance == null)
        {
            Debug.LogError("BattleSystem.Instance is null!");
            return;
        }

        var enemies = Resources.LoadAll<EnemyBase>("Enemy");
        if (enemies == null || enemies.Length == 0)
        {
            Debug.LogError("No enemies found in Resources/Enemy!");
            return;
        }

        var randomEnemy = enemies[Random.Range(0, enemies.Length)];

        // Pass item names from player's inventory to the battle dialog box
        BattleSystem.Instance.StartBattle(randomEnemy);
        Debug.Log("Battle started.");
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
    }

    public void Heal(int healAmount)
    {
        Health += healAmount;
        if (Health > MaxHealth) Health = MaxHealth;
    }

    public void GainExperience(float exp)
    {
        experience += exp;
        if (experience >= 1)
        {
            LevelUp();
            experience = 0;
        }
    }

    public void LevelUp()
    {
        SetLevel(Level + 1);
    }

    public void SetLevel(int newLevel)
    {
        if (newLevel < 1)
        {
            Debug.LogError("Level must be at least 1.");
            return;
        }

        Level = newLevel;
        InitializeStats(Level);
        Health = MaxHealth;
    }

    private void InitializeStats(int currentLevel)
    {
        MaxHealth = 100 + (currentLevel - 1) * 10;
        Attack = 1 + (currentLevel - 1) * 5;
        Defense = 1 + (currentLevel - 1) * 5;
        Speed = 1 + (currentLevel - 1) * 5;
    }

    public void ReplaceItem(int slotIndex, ItemBase newItem)
    {
        inventory.ReplaceDynamicItem(slotIndex, newItem); // Replace item in the given slot
    }

    public void OfferItemAfterBattle(ItemBase newItem)
    {
        // Handle the logic of offering the player the new item after battle
        Debug.Log($"You received a {newItem.Name}. Do you want to replace an item?");
        // Implement the logic for selecting a slot to replace here
    }

    // New method to get item names from inventory
    public List<ItemBase> GetItems()
    {
        return inventory.GetItemNames();
    }
}