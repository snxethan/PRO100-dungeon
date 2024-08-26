using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    #region Fields & Properties
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int level = 1;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private new string name = "Player";
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<ItemBase> initialItems;

    private float experience;
    private bool isInTransition;
    private bool isMoving;
    private Vector2 input;
    private Animator animator;

    public int MaxHP => maxHP;
    public int Level { get; private set; }
    public int HP { get; private set; }
    public string Name => name;
    public int Speed { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public Inventory Inventory => inventory;

    public event Action OnEncountered;
    private const float Offsety = 0.3f;

    private CountdownTimer timer;
    #endregion
    
    #region Initialization
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() // Start is called before the first frame update
    {
        timer = GetComponent<CountdownTimer>(); 
        SetLevel(level);
        Heal(MaxHP);
        SetPositionAndSnapToTile(transform.position);
        InitializePlayerInventory();
    }

    /// <summary>
    /// Initializes the player's inventory with the initial items.
    /// </summary>
    private void InitializePlayerInventory()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned!");
            return;
        }
        if (Level == -1)
        {
            foreach (var item in initialItems)
            {
                item.SetUses(-1);
            }
        }
        else
        {
            foreach (var item in initialItems)
            {
                item.ResetUses();
            }
        }

        inventory.InitializeInventory(Name,initialItems);

        var items = inventory.GetItems();
        Debug.Log($"{Name} inventory initialized with {items.Count} items.");
        
    }
    
    /// <summary>
    /// Initializes the player's stats based on the level provided in the inspector.
    /// </summary>
    /// <param name="currentLevel"></param>
    private void InitializeStats(int currentLevel)
    {
        if (currentLevel == -1) // Debug mode
        {
            maxHP = 9999;
            Attack = 9999;
            Defense = 9999;
            Speed = 9999;
        }
        else
        {
            maxHP = 100 + (currentLevel - 1) * 10;
            Attack = 1 + (currentLevel - 1) * 5;
            Defense = 1 + (currentLevel - 1) * 5;
            Speed = 1 + (currentLevel - 1) * 5;
        }
    }
    #endregion

    #region Movement & Actions
    
    public void HandleUpdate() // Update is called once per frame
    {
        if (!isMoving && !isInTransition) // If the player is not moving and not in transition
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos)) StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("isMoving", isMoving); // Set the isMoving parameter in the animator to the isMoving field
    }
    
    public void SetPositionAndSnapToTile(Vector2 pos) // Snap the player to the center of the tile
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + Offsety;

        transform.position = pos;
    }
    IEnumerator Move(Vector3 targetPos) // Coroutine to move the player
    {
        if(isInTransition) yield break;
        isMoving = true;
        // While the distance between the target position and the player's position is greater than a very small number
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) 
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            // Wait until the next frame
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        
        OnMoveOver(); // Call the OnMoveOver method
    }

    /// <summary>
    /// This method is called when the player has finished moving.
    /// </summary>
    private void OnMoveOver() 
    {
        // Check if the player is on a triggerable tile
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, Offsety), 0.2f, GameLayer.i.TriggerableLayers);

        foreach (var collider in colliders) // Loop through all the colliders
        {
            Debug.Log($"Detected collider: {collider.name} on layer {collider.gameObject.layer}");
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                // Call the OnPlayerTriggered method on the triggerable object
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }

        if (timer.TimeRemaining <= 0) // If the timer has run out
            CheckForEnemy(); // Check for an enemy to trigger a battle
    }
    private bool IsWalkable(Vector3 targetPos)
    {
        // Check if the player is on a walkable tile
        return Physics2D.OverlapCircle(targetPos, 0.05f, GameLayer.i.SolidObjectsLayer) == null;
    }
    
    /// <summary>
    /// Randomly checks for an enemy to trigger a battle.
    /// The chance of triggering a battle is 10%.
    /// </summary>
    private void CheckForEnemy()
    {
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, 0.5f, GameLayer.i.EnemyLayer);
        if (enemyCollider != null && Random.Range(1, 101) <= 11) 
        {
            OnEncountered?.Invoke();
        }
    }
    #endregion

    #region Portal Logic
    public void StartPortalTransition() // Start the portal transition
    {
        isInTransition = true;
        input = Vector2.zero;
    }

    public void EndPortalTransition() // End the portal transition
    {
        isInTransition = false;
    }

    #endregion

    public void StartTimer(float time)
    {
        timer.StartCountdown(time);
    }

    #region Battle Logic

    /// <summary>
    /// Triggers a battle with a random enemy.
    /// </summary>
    public void TriggerBattle()
    {
        if (HP <= 0) // redundant check for HP
        {
            Application.Quit();
        }
        if (BattleSystem.Instance == null) // Check if the BattleSystem instance is null
        {
            Debug.LogError("BattleSystem.Instance is null!"); 
            return;
        }

        var enemies = Resources.LoadAll<EnemyBase>("Enemy"); // Load all the enemies from the Resources/Enemy folder
        if (enemies.Length == 0)
        {
            Debug.LogError("No enemies found in Resources/Enemy!");
            return;
        }

        var randomEnemy = enemies[Random.Range(0, enemies.Length)]; // Get a random enemy from the list
        BattleSystem.Instance.StartBattle(randomEnemy); // Start the battle with the random enemy
    }

    #region Level, Health, and Defense
    
    #region Level Logic
    /// <summary>
    /// Sets the player's level and initializes the stats based on the level.
    /// </summary>
    /// <param name="newLevel">The New Level to initialize</param>
    public void SetLevel(int newLevel)
    {
        if(newLevel == -1)
        {
            Debug.Log($"{Name} is in debug mode. Setting stats to max.");
            level = -1;
            Level = level;
            InitializeStats(level);
            Heal(MaxHP);
        }
        else
        {
            level = Mathf.Max(1, newLevel);
            Level = level;
            InitializeStats(level);
            Heal(MaxHP);
        }
    }
    public bool GainExperience(float exp) // Gain experience and level up if enough experience is gained
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
    private void LevelUp()
    {
        SetLevel(Level + 1);
    }
    #endregion
    
    #region Health Logic
    /// <summary>
    /// just assign the new health to the HP field.
    /// </summary>
    /// <param name="newHealth"></param>
    /// <returns></returns>
    private bool SetHP(int newHealth)
    {
        HP = Mathf.Clamp(newHealth, 0, MaxHP);
        if(HP <= 0)
        {
            HP = 0;
            Debug.Log($"{Name} has died.");
            return true;
        }

        return false;
    }
    /// <summary>
    /// Heal the player by the healAmount.
    /// </summary>
    /// <param name="healAmount"></param>
    public void Heal(int healAmount)
    {
        if (healAmount == -1)
        {
            HP = MaxHP;
        }
        else
        {
            float modifiers = Random.Range(0.85f, 1.15f);
            int totalHeal = Mathf.FloorToInt(healAmount * modifiers);
            HP = Mathf.Min(MaxHP, HP + totalHeal);
        }
    }
    
    /// <summary>
    /// Take damage from an enemy using an item.
    /// </summary>
    /// <param name="itemBase">The item being used</param>
    /// <param name="attacker">the enemy attacking</param>
    /// <returns>If the player dies or not</returns>
    public bool TakeDamage(ItemBase itemBase, Enemy attacker)
    {
        if(Level == -1)
        {
            Debug.Log($"{Name} is in debug mode. Taking no damage.");
            return false;
        }
        Debug.Log($"{Name} is taking damage from {attacker.Name} using {itemBase.Name}");
        float modifiers = Random.Range(0.85f, 1.15f);
        int itemDamage = itemBase.GetItemModifier(attacker.Level);
        if (itemDamage == -1)
        {
            Debug.Log("Item returned infinite damage");
            return SetHP(0);
        }
        int baseDamage = attacker.Attack; // Get the attacker's attack
        int totalDamage = Mathf.FloorToInt((itemDamage + baseDamage) * modifiers); // Calculate the total damage

        totalDamage = Mathf.Max(0, totalDamage - Defense); // Subtract the player's defense from the total damage

        return SetHP(HP - totalDamage); // Set the player's HP to the new value
    }
    #endregion
    
    #region Defense Logic
    
    public int AddDefense(int defense) // Add defense to the player
    {
        if(defense == -1) // If the defense is -1, set the defense to 9999
        {
            Debug.Log("Item returned full defense");
            Defense = 9999;
            return Defense;
        }
        float modifiers = Random.Range(0.85f, 1.15f); // Get a random modifier between 0.85 and 1.15
        int totalDefense = Mathf.FloorToInt(defense * modifiers); // Calculate the total defense
        Defense += totalDefense; // Add the total defense to the player's defense
        return Defense;
    }
    #endregion

    #region Item Logic

    public void UseItem(int slotIndex) // Use an item from the player's inventory
    {
        var item = inventory.GetItems()[slotIndex]; // Get the item from the inventory
        if (item != null)
        {
            if (Level != -1) // If the player is not in debug mode
            {
                if (item.Uses <= 0 && !item.UnlimetedUse) // If the item has no uses left
                {
                    inventory.RemoveItem(slotIndex);
                    Debug.Log($"{item} has no uses left and has been removed from the inventory."); 
                    return;
                }
                bool isUsedUp = item.UseItem();  // Use the item and check if it's used up
                if (isUsedUp) 
                {
                    inventory.RemoveItem(slotIndex); // Remove the item from the inventory
                }
            }
            else
            {
                item.SetUses(-1); // Set the item's uses to -1 (debug mode/infinite)
                Debug.Log($"{Name} is in debug mode. {item} has infinite uses.");
            }
        }
    }

    public void ReplaceItem(int slotIndex, ItemBase newItem)
    {
        inventory.ReplaceItem(slotIndex, newItem);
    }

    public List<ItemBase> GetItems()
    {
        return inventory.GetItems();
    }
    public void AddItem(ItemBase item)
    {
        inventory.AddItem(item);
    }
    #endregion

    #endregion
    
    #endregion
    
}