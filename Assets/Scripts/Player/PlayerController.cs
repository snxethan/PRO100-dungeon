using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
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

    public event Action OnEncountered;
    private const float Offsety = 0.3f;

    private CountdownTimer timer;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        timer = GetComponent<CountdownTimer>();
        SetLevel(level);
        Heal(MaxHP);
        SetPositionAndSnapToTile(transform.position);
        InitializePlayerInventory();
    }

    private void InitializePlayerInventory()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned!");
            return;
        }

        inventory.InitializeInventory(initialItems);

        var items = inventory.GetItems();
        Debug.Log($"Player inventory initialized with {items.Count} items.");
        
    }

    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(1, newLevel);
        Level = level;
        InitializeStats(level);
        Heal(MaxHP);
    }

    private void InitializeStats(int currentLevel)
    {
        if (currentLevel == -1)
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

    public void HandleUpdate()
    {
        if (!isMoving && !isInTransition)
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

    private void CheckForEnemy()
    {
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, 0.5f, GameLayer.i.EnemyLayer);
        if (enemyCollider != null && Random.Range(1, 101) <= 11) 
        {
            OnEncountered?.Invoke();
        }
    }

    public void TriggerBattle()
    {
        if (BattleSystem.Instance == null)
        {
            Debug.LogError("BattleSystem.Instance is null!"); 
            return;
        }

        var enemies = Resources.LoadAll<EnemyBase>("Enemy");
        if (enemies.Length == 0)
        {
            Debug.LogError("No enemies found in Resources/Enemy!");
            return;
        }

        var randomEnemy = enemies[Random.Range(0, enemies.Length)];
        BattleSystem.Instance.StartBattle(randomEnemy);
    }

    public bool TakeDamage(ItemBase itemBase, Enemy attacker)
    {
        Debug.Log($"Player is taking damage from {attacker.Name} using {itemBase.Name}");
        float modifiers = Random.Range(0.85f, 1.15f);
        int itemDamage = itemBase.GetItemModifier(attacker.Level);
        if (itemDamage == -1)
        {
            Debug.Log("Item returned infinite damage");
            return SetHP(0);
        }
        int baseDamage = attacker.Attack;
        int totalDamage = Mathf.FloorToInt((itemDamage + baseDamage) * modifiers);

        return SetHP(HP - totalDamage);
    }

    public void Heal(int healAmount)
    {
        if (healAmount == -1)
        {
            // Full heal
            HP = MaxHP;
        }
        else
        {
            float modifiers = Random.Range(0.85f, 1.15f);
            int totalHeal = Mathf.FloorToInt(healAmount * modifiers);
            HP = Mathf.Min(MaxHP, HP + totalHeal);
        }
    }

    public bool SetHP(int newHealth)
    {
        HP = Mathf.Clamp(newHealth, 0, MaxHP);
        if(HP <= 0)
        {
            HP = 0;
            Debug.Log("Player has died.");
            return true;
        }

        return false;
    }

    public int AddDefense(int defense)
    {
        if(defense == -1)
        {
            Debug.Log("Item returned full defense");
            Defense = 9999;
            return Defense;
        }
        float modifiers = Random.Range(0.85f, 1.15f);
        int totalDefense = Mathf.FloorToInt(defense * modifiers);
        Defense += totalDefense;
        return Defense;
    }

    public bool GainExperience(float exp)
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

    public void LevelUp()
    {
        SetLevel(Level + 1);
    }

    public void ReplaceItem(int slotIndex, ItemBase newItem)
    {
        newItem.SetUses(10);
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

    public void UseItem(int slotIndex)
    {
        var item = inventory.GetItems()[slotIndex];
        if (item != null)
        {
            if (item.Uses <= 0 && !item.UnlimetedUse)
            {
                inventory.RemoveItem(slotIndex);
                Debug.Log("Item has no uses left and has been removed from the inventory.");
                return;
            }

            bool isUsedUp = item.UseItem();
            if (isUsedUp)
            {
                inventory.RemoveItem(slotIndex);
            }
        }
    }
}