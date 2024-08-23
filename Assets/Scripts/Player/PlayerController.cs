using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    #region fields
    [SerializeField] private int maxHP = 100; // Max health of the player
    [SerializeField] private int level = 1; // Level of the player
    [SerializeField] private float moveSpeed = 5; // walk speed of the player
    [SerializeField] private new string name = "Player"; // Name of the player
    [SerializeField] private Inventory inventory; // Inventory of the player
    [SerializeField] private List<ItemBase> fixedItems; // Fixed items of the player
    #endregion

    #region properties
    private float experience;
    private bool isInTransition;
    private bool isMoving;
    private Vector2 input;
    private Animator animator;

    public int MaxHP => maxHP; // Reference maxHealth field
    public int Level { get; private set; }
    public int HP { get; private set; }
    public string Name => name;
    public int Speed { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }

    public event Action OnEncountered; // Event to trigger when the player encounters an enemy
    private const float Offsety = 0.3f; // Offset to adjust the player position
    #endregion

    private CountdownTimer timer;

    #region initialize player
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        timer = GetComponent<CountdownTimer>();
        SetLevel(level); // Ensure level is at least 1
        Heal(MaxHP); // Heal the player to full health
        SetPositionAndSnapToTile(transform.position); // Snap the player to the tile
        InitializePlayerInventory(); // Initialize the player inventory
    }

    private void InitializePlayerInventory()
    {
        if (inventory == null) // Check if the inventory is assigned
        {
            Debug.LogError("Inventory is not assigned!");
            return;
        }

        inventory.InitializeInventory(fixedItems); // Initialize the inventory with fixed items

        // Add debug log to verify inventory initialization
        var items = inventory.GetItems();
        Debug.Log($"Player inventory initialized with {items.Count} items.");
    }
    private void InitializeStats(int currentLevel)
    {
        if (currentLevel == -1)
        {
            // Debug/testing mode
            maxHP = int.MaxValue;
            Attack = int.MaxValue;
            Defense = int.MaxValue;
            Speed = int.MaxValue;
        }
        else
        {
            // Initialize the player stats based on the current level
            maxHP = 100 + (currentLevel - 1) * 10;
            Attack = 1 + (currentLevel - 1) * 5;
            Defense = 1 + (currentLevel - 1) * 5;
            Speed = 1 + (currentLevel - 1) * 5;
        }
    }
    public void SetLevel(int newLevel)
    {
        if (newLevel == -1)
        {
            // Debug/testing mode
            level = -1;
            Level = level;
            HP = -1;
            maxHP = int.MaxValue;
            Attack = int.MaxValue;
            Defense = int.MaxValue;
            Speed = int.MaxValue;
        }
        else
        {
            level = Mathf.Max(1, newLevel); // Ensure level is at least 1
            Level = level; // Update the Level property
            InitializeStats(level); // Initialize the stats based on the new level
            Heal(MaxHP); // Heal the player to full health
        }
    }
    #endregion

    #region movement & animation
    public void HandleUpdate()
    {
        if (!isMoving && !isInTransition)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;  // removes diagonal movement

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (isWalkable(targetPos)) StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("isMoving", isMoving);
    }
    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + Offsety;

        transform.position = pos;
    }
    IEnumerator Move(Vector3 targetPos)
    {
        if(isInTransition) yield break;
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        
        onMoveOver();
        
    }
    private void onMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, Offsety), 0.2f, GameLayer.i.TriggerableLayers);

        foreach (var collider in colliders)
        {
            Debug.Log($"Detected collider: {collider.name} on layer {collider.gameObject.layer}");
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }

        if (timer.TimeRemaining <= 0)
            CheckForEnemy();
    }
    
    public void StartTimer(float time)
    {
        timer.StartCountdown(time);
    }

    public void StartPortalTransition()
    {
        isInTransition = true;
        input = Vector2.zero;
    }

    public void EndPortalTransition()
    {
        isInTransition = false;
    }

    private bool isWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.05f, GameLayer.i.SolidObjectsLayer) == null;
    }
    #endregion
    
    #region enemy encounter
    private void CheckForEnemy()
    {
        // Check for enemy encounter
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, 0.5f, GameLayer.i.EnemyLayer);
        // Check if the enemy collider is not null and a random chance of 10%
        if (enemyCollider != null && Random.Range(1, 101) <= 11) 
        {
            OnEncountered?.Invoke(); // Trigger the encountered event
        }
    }

    public void TriggerBattle() // Trigger the battle
    {
        if (BattleSystem.Instance == null) // Check if the BattleSystem instance is null 
        {
            Debug.LogError("BattleSystem.Instance is null!"); 
            return;
        }

        var enemies = Resources.LoadAll<EnemyBase>("Enemy"); // Load all enemies from the Resources/Enemy folder
        if (enemies.Length == 0) // Check if no enemies are found
        {
            Debug.LogError("No enemies found in Resources/Enemy!");
            return;
        }

        var randomEnemy = enemies[Random.Range(0, enemies.Length)]; // Get a random enemy from the list
        BattleSystem.Instance.StartBattle(randomEnemy); // Start the battle with the random enemy
    }
    #endregion

    #region battle logic
    public bool TakeDamage(ItemBase itemBase, Enemy attacker)
    {
        Debug.Log($"Player is taking damage from {attacker.Name} using {itemBase.Name}");
        float modifiers = Random.Range(0.85f, 1.15f); // Random factor between 0.85 and 1.15
        int itemDamage = itemBase.GetItemModifier(attacker.Level);
        int baseDamage = attacker.Attack;
        int totalDamage = Mathf.FloorToInt((itemDamage + baseDamage) * modifiers);

        return SetHP(HP - totalDamage); // Subtract damage from current health
    }

    public void Heal(int healAmount)
    {
        float modifiers = Random.Range(0.85f, 1.15f); // Random factor between 0.85 and 1.15
        int totalHeal = Mathf.FloorToInt(healAmount * modifiers);
        HP = Mathf.Min(MaxHP, HP + totalHeal);
    }
    public bool SetHP(int newHealth) // Set the health of the player
    {
        HP = Mathf.Clamp(newHealth, 0, MaxHP); // Clamp the health between 0 and max health
        if(HP <= 0)
        {
            HP = 0;
            Debug.Log("Player has died.");
            return true;
        }

        return false; // Return false if the player is still alive
    }

    public int AddDefense(int defense)
    {
        float modifiers = Random.Range(0.85f, 1.15f); // Random factor between 0.85 and 1.15
        int totalDefense = Mathf.FloorToInt(defense * modifiers);
        Defense += totalDefense;
        return Defense;
    }

    public bool GainExperience(float exp) // Gain experience
    {
        experience += exp;
        if (experience >= 1)
        {
            LevelUp();
            experience = 0;
            return true;
        }
        return false;
    }

    public void LevelUp() // Level up the player
    {
        SetLevel(Level + 1);
    }
    #endregion
    
    #region item & inventory
    public void ReplaceItem(int slotIndex, ItemBase newItem) // Replace an item in the inventory
    {
        newItem.SetUses(10); // Set the uses of the new item
        inventory.ReplaceDynamicItem(slotIndex, newItem);
    }

    public void OfferItemAfterBattle(ItemBase newItem) // Offer an item after battle
    {
        Debug.Log($"You received a {newItem.Name}. Do you want to replace an item?");
    }

    public List<ItemBase> GetItems() // Get the items from the inventory
    {
        return inventory.GetItems();
    }
    public void UseItem(int slotIndex)
    {
        var item = inventory.GetItems()[slotIndex];
        if (item != null)
        {
            if (item.UnlimetedUse)
            {
                Debug.Log("Item has unlimited uses.");
                return;
            }
            bool isUsedUp = item.UseItem();
            if (isUsedUp && !fixedItems.Contains(item))
            {
                inventory.RemoveItem(slotIndex);
            }
            inventory.CheckAndConvertFixedSlots();
        }
    }
    #endregion

}