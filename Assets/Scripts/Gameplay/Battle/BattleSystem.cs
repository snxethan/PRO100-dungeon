using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public enum BattleState
{
    START,
    PLAYER_ACTION,
    PLAYER_MOVE,
    ENEMY_MOVE,
    ITEM_SELECTION,
    BUSY,
    END
}

public class BattleSystem : MonoBehaviour
{
    [Header("Battle Components")]
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleDialogBox dialogBox;
    [SerializeField] private GameObject battleCanvas;
    [SerializeField] private CameraSwitcher cameraSwitcher;

    public event Action<bool> OnBattleOver;
    public BattleState state;
    private int currentAction;
    private int currentItem;
    private bool isBattleInProgress;
    private ItemBase droppedItem; // dropped item from the enemy
    private bool playerGoesFirst;
    private WaitForSeconds dialogDelay = new (.5f);

    public static BattleSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (battleCanvas == null || cameraSwitcher == null || enemyUnit == null || playerUnit == null ||
            enemyHUD == null || playerHUD == null || dialogBox == null)
        {
            Debug.LogError("BattleSystem: Missing references in the Inspector!");
            return;
        }

        HideBattleUI();
        cameraSwitcher.ActivateMainCamera();
    }


    public void StartBattle(EnemyBase enemyBase)
    {
        if (isBattleInProgress)
        {
            Debug.LogWarning("A battle is already in progress. Cannot start a new battle.");
            return;
        }

        isBattleInProgress = true;
        StartCoroutine(SetupBattle(enemyBase));
    }

    private IEnumerator SetupBattle(EnemyBase enemyBase)
    {
        state = BattleState.START;

        cameraSwitcher.ActivateBattleCamera();
        yield return null;

        ShowBattleUI();

        playerUnit.Setup(true);
        enemyUnit.Setup(false, enemyBase);

        playerHUD.SetData(playerUnit.player);
        enemyHUD.SetData(enemyUnit.Enemy);

        yield return StartCoroutine(dialogBox.TypeDialog($"The {enemyUnit.Enemy.Base.Type}: {enemyUnit.Enemy.Name} appeared!"));
        yield return dialogDelay;

        if (playerUnit.player.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} is dead! Are they a ghost?");
            Debug.Log($"{playerUnit.player.Name} attempted to start battle, but {playerUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(false, false));
            yield break;
        }
        
        playerGoesFirst = DetermineFirstTurn();
        
        dialogBox.ChangeActionText("Fight", "Run");
        yield return DisplayActionChoice();
    }

    private void HideBattleUI()
    {
        if (battleCanvas != null)
        {
            battleCanvas.SetActive(false);
            dialogBox.ToggleActionSelector(false);
            dialogBox.ToggleItemSelector(false);
        }
    }

    private void ShowBattleUI()
    {
        if (battleCanvas != null) battleCanvas.SetActive(true);
    }

    private IEnumerator DisplayActionChoice()
    {
        Debug.Log("DisplayActionChoice: Called");
        state = BattleState.PLAYER_ACTION;
        dialogBox.ToggleItemSelector(false);
        dialogBox.ToggleActionSelector(true);
        yield return dialogBox.TypeDialog("Choose your action:");
    }

    private IEnumerator DetermineTurnOrder()
    {
        // Set state to BUSY immediately to avoid any other inputs
        state = BattleState.BUSY;
        Debug.Log("DetermineTurnOrder: Player goes first: " + playerGoesFirst);

        if (playerGoesFirst)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} goes first!");
            yield return PlayerMove();
            yield return dialogDelay;

            if (state != BattleState.END)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name}'s goes next!");
                yield return EnemyMove();
                yield return dialogDelay;
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} goes first!");
            yield return EnemyMove();
            yield return dialogDelay;

            if (state != BattleState.END)
            {
                yield return dialogBox.TypeDialog($"{playerUnit.player.Name} goes next!");
                yield return PlayerMove();
                yield return dialogDelay;
            }
        }
        playerGoesFirst = !playerGoesFirst;
        if (state != BattleState.END && state != BattleState.ITEM_SELECTION && state != BattleState.PLAYER_MOVE && state != BattleState.ENEMY_MOVE)
        {
            state = BattleState.PLAYER_ACTION;
            dialogBox.ChangeActionText("Fight", "Run");
            yield return DisplayActionChoice();
        }
    }


    private bool DetermineFirstTurn()
    {
        if (playerUnit.player.Speed == enemyUnit.Enemy.Speed)
        {
            //player will go first if speed is equal
            playerGoesFirst = true;
        }
        else
        {
            playerGoesFirst = playerUnit.player.Speed > enemyUnit.Enemy.Speed;
        }
        return playerGoesFirst;
    }

    private IEnumerator PlayerMove()
    {
        if(playerUnit.player.HP <= 0)
        {
            StartCoroutine(EndBattleCoroutine(false, false));
            yield break;
        }

        if (playerUnit.player.Inventory.IsFull())
        {
            while (isBattleInProgress)
            {
                yield return dialogBox.TypeDialog($"{playerUnit.player.Name}'s Inventory is empty.. Skipping turn!!");
                yield return EnemyMove();
            }
        }
        else
        {
            state = BattleState.PLAYER_MOVE;
            dialogBox.ToggleActionSelector(false);
            dialogBox.ToggleItemSelector(true);
            currentItem = 0;
            
            var items = playerUnit.player.GetItems();
            if (items.Count > 0 && items[currentItem] != null)
            {
                dialogBox.SetItemNames(items);
                dialogBox.UpdateItemSelection(currentItem, items[currentItem], playerUnit.player.Level);
            }
            else
            {
                Debug.LogError("Player item list is null or empty.");
            }
            yield return WaitForPlayerMove();
        }
    }

    private IEnumerator WaitForPlayerMove()
    {
        while (state == BattleState.PLAYER_MOVE)
        {
            yield return null;
        }
    }

    private IEnumerator PerformPlayerMove()
    {
        if (playerUnit.player.HP <= 0)
        {
            Debug.Log($"{playerUnit.player.Name} attempted to play turn, but {playerUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(false, false));
            yield break;
        }
        if (enemyUnit.Enemy.HP <= 0)
        {
            Debug.Log($"{playerUnit.player.Name} attempted to play turn, but {enemyUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(true, false));
            yield break;
        }

        float exp = .03f;
        state = BattleState.PLAYER_MOVE;
        var item = playerUnit.player.GetItems()[currentItem];

        if (item == null)
        {
            Debug.LogError("Selected item is null.");
            yield break;
        }

        Debug.Log(item.Name);
        playerUnit.player.UseItem(currentItem);
        yield return dialogBox.TypeDialog($"{playerUnit.player.Name} used {item.Name}! {item.GetItemTypeStr(playerUnit.player.Level)}");
        yield return dialogDelay;

        bool enemyDefeated;

        if (item.ItemType == ItemType.AttackItem)
        {
            exp += .05f;
            int health = enemyUnit.Enemy.HP;
            enemyDefeated = enemyUnit.Enemy.TakeDamage(item, playerUnit.player);
            int damageTaken = health - enemyUnit.Enemy.HP;
            enemyHUD.UpdateHP(false);
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} took {damageTaken} damage! {enemyUnit.Enemy.HP} HP remaining.");
            if (enemyDefeated)
            {
                exp += .1f;
                yield return GainExperience(exp, true, true);
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} was defeated!");
                playerUnit.player.Heal(playerUnit.player.MaxHP / 3);
                playerHUD.UpdateHP(true);
                yield return dialogBox.TypeDialog($"{playerUnit.player.Name} healed {playerUnit.player.MaxHP / 3} HP! {playerUnit.player.Name} is now at {playerUnit.player.HP} HP");
                yield return StartCoroutine(EndBattleCoroutine(true, false));
                yield break;
            }
        }
        else if (item.ItemType == ItemType.RecoveryItem)
        {
            exp += .03f;
            playerUnit.player.GainExperience(.3f);
            int health = playerUnit.player.HP;
            playerUnit.player.Heal(item.GetItemModifier(playerUnit.player.Level));
            int healAmount = playerUnit.player.HP - health;
            playerHUD.UpdateHP(true);
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} healed {healAmount} HP! {playerUnit.player.HP} HP remaining.");
        }
        else if (item.ItemType == ItemType.DefenseItem)
        {
            exp += .04f;
            playerUnit.player.GainExperience(.4f);
            int defense = playerUnit.player.AddDefense(item.GetItemModifier(playerUnit.player.Level));
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name}'s defense increased by {defense} DEF! They now have {playerUnit.player.Defense} Total DEF.");
        }
        else
        {
            Debug.LogError("Invalid item type.");
        }

        yield return GainExperience(exp, true, true);
        yield return dialogDelay;
        state = BattleState.BUSY;
    }

    private IEnumerator EnemyMove()
    {
        if (playerUnit.player.HP <= 0)
        {
            Debug.Log($"{enemyUnit.player.Name} attempted to play turn, but {playerUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(false, false));
            yield break;
        }
        if (enemyUnit.Enemy.HP <= 0)
        {
            Debug.Log($"{enemyUnit.player.Name} attempted to play turn, but {enemyUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(true, false));
            yield break;
        }
        
        float exp = .02f;
        float playerExp = 0.01f;
        state = BattleState.ENEMY_MOVE;

        // Get all items from the enemy's inventory
        var items = enemyUnit.Enemy.GetItems().Where(i => i != null).ToList();

        // Separate items by type
        var recoveryItems = items.Where(i => i.ItemType == ItemType.RecoveryItem).ToList();
        var defenseItems = items.Where(i => i.ItemType == ItemType.DefenseItem).ToList();
        var attackItems = items.Where(i => i.ItemType == ItemType.AttackItem).ToList();

        ItemBase itemToUse = null;

        if (attackItems.Count > 0)
        {
            // Use an attack item if available
            itemToUse = attackItems[Random.Range(0, attackItems.Count)];
        }
        // Prioritize item usage based on enemy's HP
        else if (enemyUnit.Enemy.HP < (enemyUnit.Enemy.MaxHP / 3) && recoveryItems.Count > 0)
        {
            // Use a recovery item if HP is less than 25%
            itemToUse = recoveryItems[Random.Range(0, recoveryItems.Count)];
        }
        else if (enemyUnit.Enemy.HP < (enemyUnit.Enemy.MaxHP / 2) && defenseItems.Count > 0)
        {
            // Use a defense item if HP is less than 50%
            itemToUse = defenseItems[Random.Range(0, defenseItems.Count)];
        }

        // Fallback logic if no prioritized item is available
        if (itemToUse == null)
        {
            if (recoveryItems.Count > 0)
            {
                itemToUse = recoveryItems[Random.Range(0, recoveryItems.Count)];
            }
            else if (defenseItems.Count > 0)
            {
                itemToUse = defenseItems[Random.Range(0, defenseItems.Count)];
            }
            else if (attackItems.Count > 0)
            {
                itemToUse = attackItems[Random.Range(0, attackItems.Count)];
            }
            else if (items.Count > 0)
            {
                itemToUse = items[Random.Range(0, items.Count)];
            }
        }

        if (itemToUse != null)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} used {itemToUse.Name}! {itemToUse.GetItemTypeStr(enemyUnit.Enemy.Level)}");
            yield return dialogDelay;

            if (itemToUse.ItemType == ItemType.AttackItem)
            {
                exp += .03f;
                playerExp = .03f;
                int health = playerUnit.player.HP;
                bool playerDefeated = playerUnit.player.TakeDamage(itemToUse, enemyUnit.Enemy);
                int damageTaken = health - playerUnit.player.HP;
                playerHUD.UpdateHP(true);
                yield return dialogBox.TypeDialog($"{playerUnit.player.Name} took {damageTaken} damage! {playerUnit.player.HP} HP remaining.");
                if (playerDefeated)
                {
                    StartCoroutine(EndBattleCoroutine(false, false));
                    yield break;
                }
            }
            else if (itemToUse.ItemType == ItemType.RecoveryItem)
            {
                exp += .02f;
                playerExp = .02f;
                int health = enemyUnit.Enemy.HP;
                enemyUnit.Enemy.Heal(itemToUse.GetItemModifier(enemyUnit.Enemy.Level));
                int healAmount = enemyUnit.Enemy.HP - health;
                enemyHUD.UpdateHP(false);
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} healed {healAmount} HP! {enemyUnit.Enemy.HP} HP remaining.");
            }
            else if (itemToUse.ItemType == ItemType.DefenseItem)
            {
                exp += .04f;
                playerExp = .02f;
                int defense = enemyUnit.Enemy.AddDefense(itemToUse.GetItemModifier(enemyUnit.Enemy.Level));
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name}'s defense increased by {defense} DEF! They now have {enemyUnit.Enemy.Defense} Total DEF.");
            }
        }

        yield return GainExperience(playerExp, true, false);
        yield return GainExperience(exp, false, true);
        yield return dialogDelay;
        state = BattleState.BUSY;
    }

    private void RunAway()
    {
        StartCoroutine(RunAwayCoroutine());
    }

    private IEnumerator RunAwayCoroutine()
    {
        dialogBox.gameObject.SetActive(true); // Ensure the dialog box is active
        yield return dialogBox.TypeDialog($"{playerUnit.player.Name} is trying to run away!");
        yield return EnemyMove(); // Enemy gets a move while the player runs away

        yield return dialogDelay;
        yield return StartCoroutine(EndBattleCoroutine(false, true)); // End the battle, switch scene after a delay
    }

    private IEnumerator EndBattleCoroutine(bool playerWins, bool runaway)
    {
        state = BattleState.END;
        if (!runaway)
        {
            yield return dialogBox.TypeDialog(playerWins ? $"{playerUnit.player.Name} won the battle!" : $"{playerUnit.player.Name} was defeated...");
        }
        else
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} ran away!");
        }
        isBattleInProgress = false;
        if (playerWins && !runaway)
        {
            yield return HandleNewItem();
        }
        else if (!playerWins && !runaway)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name}'s soul was absorbed... Goodbye...");
            EndBattleFinalize(false);
        }
        else
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name}'s soul was protected...");
            EndBattleFinalize(playerWins);
        }
    }
    private void EndBattleFinalize(bool playerWins)
    {
        isBattleInProgress = false;
        OnBattleOver?.Invoke(playerWins);
        state = BattleState.END;
        if (!playerWins)
        {
            Application.Quit();
        }
        HideBattleUI();
        cameraSwitcher.ActivateMainCamera();
    }

    private IEnumerator HandleNewItem()
    {
        state = BattleState.ITEM_SELECTION;
        var enemyItems = enemyUnit.Enemy.GetItems();
        if (enemyItems.Count > 0)
        {
            droppedItem = enemyItems[Random.Range(0, enemyItems.Count)];
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} dropped {droppedItem.Name} {droppedItem.GetItemTypeStr(enemyUnit.Enemy.Level)}, will you accept?");
            dialogBox.ChangeActionText("Accept Item", "Run");
            dialogBox.ToggleActionSelector(true);

            float timer = 0f;
            while (state == BattleState.ITEM_SELECTION && timer < 30f)
            {
                yield return null;
                timer += Time.deltaTime;
            }

            if (state == BattleState.ITEM_SELECTION)
            {
                // Automatically run away if no choice was made
                EndBattleFinalize(true);
            }
        }
        else
        {
            EndBattleFinalize(true);
        }
    }


    private IEnumerator AcceptItem()
    {
        var items = playerUnit.player.GetItems();
        if (!playerUnit.player.Inventory.IsFull())
        {
            // Add the new item directly if there is space in the inventory
            playerUnit.player.AddItem(droppedItem);
            yield return dialogBox.TypeDialog($"Accepted item: {droppedItem.Name}");
            EndBattleFinalize(true);
        }
        else
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} has dropped {droppedItem.Name}, but your inventory is full. Choose an item to replace.");
            state = BattleState.ITEM_SELECTION;
            dialogBox.ToggleItemSelector(true);
            dialogBox.SetItemNames(items);
            yield return WaitForItemReplacement();
        }
    }
    
    private IEnumerator WaitForItemReplacement()
    {
        while (state == BattleState.ITEM_SELECTION)
        {
            yield return null;
        }
    }


    public void EndBattle(bool playerWins)
    {
        if (state == BattleState.END)
            return;
        StartCoroutine(EndBattleCoroutine(playerWins,false));
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PLAYER_ACTION)
        {
            HandleActionSelection();
        }

        if (state == BattleState.PLAYER_MOVE)
        {
            HandleItemSelection();
        }

        if (state == BattleState.ITEM_SELECTION)
        {
            HandleActionSelection();
            HandleNewItem();
        }
        
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentAction < 1) currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0) currentAction--;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state == BattleState.PLAYER_ACTION)
            {
                if (currentAction == 0) //fight
                {
                    dialogBox.ToggleActionSelector(false);
                    StartCoroutine(DetermineTurnOrder());
                }
                else if (currentAction == 1) //run away
                {
                    dialogBox.ToggleActionSelector(false);
                    RunAway();
                }
            }
            else if (state == BattleState.ITEM_SELECTION)
            {
                if (currentAction == 0) // accept item
                {
                    dialogBox.ToggleActionSelector(false);
                    StartCoroutine(AcceptItem());
                }
                else if (currentAction == 1) //run away
                {
                    dialogBox.ToggleActionSelector(false);
                    EndBattleFinalize(true);
                }
            }
        }
    }

    private void HandleItemSelection()
    {
        var items = playerUnit.player.GetItems();
        if (items == null || items.Count == 0)
        {
            Debug.LogError("Item list is null or empty.");
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentItem < items.Count - 1)
                ++currentItem;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentItem > 0)
                currentItem--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentItem < items.Count - 2)
            {
                currentItem += 2;
            }
            else if (currentItem % 2 == 0 && currentItem < items.Count - 1)
            {
                currentItem++;
            }
            else if (currentItem % 2 != 0)
            {
                currentItem--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentItem > 1)
                currentItem -= 2;
        }

        if (currentItem >= 0 && currentItem < items.Count && items[currentItem] != null)
        {
            var selectedItem = items[currentItem];
            dialogBox.UpdateItemSelection(currentItem, selectedItem, playerUnit.player.Level);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state == BattleState.ITEM_SELECTION)
            {
                if (items[currentItem] != null)
                {
                    playerUnit.player.ReplaceItem(currentItem, droppedItem);
                    dialogBox.ToggleItemSelector(false);
                    dialogBox.ToggleDialogText(true);
                    StartCoroutine(dialogBox.TypeDialog($"Replaced item with {droppedItem.Name}"));
                    EndBattleFinalize(true);
                }
            }
            else
            {
                if (items[currentItem] != null)
                {
                    state = BattleState.BUSY;
                    dialogBox.ToggleItemSelector(false);
                    dialogBox.ToggleDialogText(true);
                    StartCoroutine(PerformPlayerMove());
                }
            }
        }
    }


    private IEnumerator GainExperience(float exp, bool isPlayer, bool wantDisplay)
    {
        if (isPlayer)
        {
            if (playerUnit.player.GainExperience(exp))
            {
                if (wantDisplay)
                {
                    yield return dialogBox.TypeDialog($"{playerUnit.player.Name} leveled up to {playerUnit.player.Level}!");
                }
                playerHUD.UpdateLevel(true);
            }
            else
            {
                if (wantDisplay)
                {
                    yield return dialogBox.TypeDialog($"{playerUnit.player.Name} gained {ToPercentageString(exp)} experience!");
                }
            }
        }
        else
        {
            if (enemyUnit.Enemy.GainExperience(exp))
            {
                if (wantDisplay)
                {
                    yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} leveled up to {enemyUnit.Enemy.Level}!");
                }
                enemyHUD.UpdateLevel(false);
            }
            else
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} gained {ToPercentageString(exp)} experience!");
            }
        }
    }
    private static string ToPercentageString(float experience)
    {
        return (experience * 100).ToString("F0");
    }
}