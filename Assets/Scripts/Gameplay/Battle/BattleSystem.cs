using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// the enums that are used to determine the state of the entire battle system.
/// </summary>
public enum BattleState // the state of the battle that is used to determine when actions can be taken
{
    START,
    PLAYER_ACTION,
    PLAYER_MOVE,
    ENEMY_MOVE,
    ITEM_SELECTION,
    REWARD_SELECTION,
    BUSY,
    END
}

/// <summary>
/// the logic that handles the turn based combat system in the game.
/// </summary>
public class BattleSystem : MonoBehaviour
{
    #region Fields
    // serialized fields, these are assigned in the inspector
    [Header("Battle Components")]
    [SerializeField] private BattleUnit enemyUnit; 
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleDialogBox dialogBox; // the dialog box that displays text during battle
    [SerializeField] private GameObject battleCanvas;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    
    public BattleState state; // the current state of the battle
    private ItemBase droppedItem; // global var for the enemies' dropped item
    public event Action<bool> OnBattleOver; // event that is invoked when the battle is over
    private WaitForSeconds dialogDelay = new (.5f); // delay for the dialog box, used to give the player time to read the text

    private int currentAction; // the current action that the player has selected
    private int currentItem; // the current item that the player has selected
    private bool isBattleInProgress; 
    private bool playerGoesFirst; 
    private bool isInputBlocked; // used to block input during certain parts of the battle

    public static BattleSystem Instance { get; private set; } // singleton instance of the BattleSystem
    #endregion
    
    #region Start and Awake
    private void Awake() // called when the script instance is being loaded
    {
        if (Instance == null) 
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); // don't destroy the game object when loading a new scene
        }
        else // if the instance is not null
        {
            Destroy(gameObject); // destroy the game object
        }
    }
    /// <summary>
    /// this method is immediately called when the script is loaded,
    /// so, we hide the battle UI and activate the main camera, this loads the battle system whenever the player is instantiated so we don't have to call it later.
    /// </summary>
    private void Start()
    {
        // check if any of the required components are missing
        if (battleCanvas == null || cameraSwitcher == null || enemyUnit == null || playerUnit == null ||
            enemyHUD == null || playerHUD == null || dialogBox == null)
        {
            Debug.LogError("BattleSystem: Missing references in the Inspector!");
            return;
        }

        HideBattleUI(); // hide the battle UI
        cameraSwitcher.ActivateMainCamera(); // activate the main camera
    }
    /// <summary>
    /// This method is called when the game wants to load a battle, it must pass an enemyBase object to start the battle.
    /// </summary>
    /// <param name="enemyBase">the enemy you want the player to battle agaisnt</param>
    public void StartBattle(EnemyBase enemyBase)
    {
        if (isBattleInProgress)
        {
            Debug.LogWarning("A battle is already in progress. Cannot start a new battle.");
            return;
        }

        isBattleInProgress = true ; // set the battle to in progress
        StartCoroutine(SetupBattle(enemyBase)); // start the battle
    }

    /// <summary>
    /// this properly creates the battle,
    /// resets the states, enables UI,
    /// and properly sets up the BATTLE UNITS and HUDs
    /// </summary>
    /// <param name="enemyBase">The enemy you want the player to battle agaisnt</param>
    /// <returns></returns>
    private IEnumerator SetupBattle(EnemyBase enemyBase)
    {
        //reset states
        state = BattleState.START;
        currentAction = 0;
        currentItem = 0;
        
        cameraSwitcher.ActivateBattleCamera(); //switch camera
        yield return null;
        ShowBattleUI();

        //setup battle units
        playerUnit.Setup(true);
        enemyUnit.Setup(false, enemyBase);

        //setup battle HUDs
        playerHUD.SetData(playerUnit.player);
        enemyHUD.SetData(enemyUnit.Enemy);

        //properly start the battle
        yield return StartCoroutine(dialogBox.TypeDialog($"You encountered {enemyUnit.Enemy.Base.Type}: {enemyUnit.Enemy.Name} (LVL {enemyUnit.Enemy.Level})!"));
        yield return dialogDelay;

        if (playerUnit.player.HP <= 0) //redundant check to make sure the player has health
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} is dead! Are they a ghost?");
            Debug.Log($"{playerUnit.player.Name} attempted to start battle, but {playerUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(false, false));
            yield break;
        }
        
        playerGoesFirst = DetermineFirstTurn(); //determine who goes first
        
        //toggle action selector
        dialogBox.ChangeActionText("Fight", "Run");
        yield return DisplayActionChoice();
    }

    #region UI
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

    #endregion
    
    #endregion

    #region Battle Logic
    
    #region Turn Order Logic
    private bool DetermineFirstTurn()
    {
        //determine who goes first based off of Speed values
        if (playerUnit.player.Speed == enemyUnit.Enemy.Speed)
        {
            //player will go first if speed is equal
            playerGoesFirst = true;
        }
        else
        {
            playerGoesFirst = playerUnit.player.Speed > enemyUnit.Enemy.Speed;
        }
        return playerGoesFirst; //updates the global variable
    }
    /// <summary>
    ///  properly instantiate the turn order system for a round,
    ///  a round is when both the player and enemy have used 1 item.
    /// the turn order will swap each round.
    /// (player -> enemy) and then (enemy -> player)
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetermineTurnOrder()
    {
      
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
        playerGoesFirst = !playerGoesFirst; //swap turn order
        //if the battle is still in progress, display the action choice
        if (state != BattleState.END && state != BattleState.ITEM_SELECTION && state != BattleState.PLAYER_MOVE && state != BattleState.ENEMY_MOVE)
        {
            state = BattleState.PLAYER_ACTION;
            dialogBox.ChangeActionText("Fight", "Run");
            yield return DisplayActionChoice();
        }
    }
    #endregion
    
    #region Player Move Logic
    /// <summary>
    /// the player's turn to move, this is where the player can choose to use an item or run away.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerMove()
    {
        if(playerUnit.player.HP <= 0) //redudant check to make sure the player has health
        {
            Debug.LogError($"{playerUnit.player.Name} attempted to play turn, but {playerUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(false, false));
            yield break;
        }

        if (playerUnit.player.Inventory.IsEmpty()) //check if the player has any items
        {
            while (isBattleInProgress)
            {
                yield return dialogBox.TypeDialog($"{playerUnit.player.Name}'s Inventory is empty.. Skipping turn!!");
                yield return EnemyMove();
            }
        }
        else
        {
            //player has items, so we can display the item selection
            state = BattleState.PLAYER_MOVE;
            dialogBox.ToggleActionSelector(false);
            dialogBox.ToggleItemSelector(true);
            currentItem = 0; //reset current item
            
            //display the items to select for a player to choose from
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
            while (state == BattleState.PLAYER_MOVE) //wait for the player to make a move
            {
                yield return null;
            }
        }
    }
    
    /// <summary>
    /// the method that is called when the player has selected an item to use.
    /// </summary>
    /// <returns></returns>
     private IEnumerator PerformPlayerMove()
    {
        state = BattleState.PLAYER_MOVE;
        isInputBlocked = true; // Block input to prevent multiple actions
        if (playerUnit.player.HP <= 0) //redudant check to make sure the player has health
        { 
            Debug.LogError($"{playerUnit.player.Name} attempted to play turn, but {playerUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(false, false));
            yield break;
        }
        if (enemyUnit.Enemy.HP <= 0) //redudant check to make sure the enemy has health
        {
            Debug.LogError($"{playerUnit.player.Name} attempted to play turn, but {enemyUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(true, false));
            yield break;
        }

        float exp = .03f; //experience gained throughout a players move, can increment based on certain conditions
        var item = playerUnit.player.GetItems()[currentItem]; //get the item the player has selected

        if (item == null) //redudant check to make sure the item is not null
        {
            Debug.LogError("Selected item is null.");
            yield break;
        }

        Debug.Log(item.Name); //use the item
        playerUnit.player.UseItem(currentItem);
        yield return dialogBox.TypeDialog($"{playerUnit.player.Name} used {item.Name}! {item.GetItemTypeStr(playerUnit.player.Level)}");
        yield return dialogDelay;

        if(item.Uses <= 0 && !item.UnlimetedUse) //check if the item has run out of uses
        {
            exp += .02f;
            if (playerUnit.player.Level != -1)
            {
                yield return dialogBox.TypeDialog($"{item.Name} ran out of uses! It was removed from {playerUnit.player.Name}'s inventory.");
            }
            else
            {
                //debug mode
                item.SetUses(-1);
            }
        }
        
        //if the item is an attack item
        if (item.ItemType == ItemType.AttackItem)
        {
            exp += .05f; //increment experience because the player attacked the enemy
            
            //take damage from the enemy
            int health = enemyUnit.Enemy.HP;
            bool enemyDefeated = enemyUnit.Enemy.TakeDamage(item, playerUnit.player);
            int damageTaken = health - enemyUnit.Enemy.HP;
            enemyHUD.UpdateHP(false);
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} took {damageTaken} damage! {enemyUnit.Enemy.HP} HP remaining.");
            
            if (enemyDefeated) //if the player has killed the enemy
            {
                exp += .1f; //increment experience because the player defeated the enemy
                if (playerUnit.player.Level < enemyUnit.Enemy.Level)
                {
                    exp += 1f; //increment experience because the player defeated an enemy with a higher level
                }
                yield return GainExperience(exp, true, true); //gain experience
                
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} was defeated!");
                
                //heal the player if they are not in debug mode
                playerUnit.player.Heal(playerUnit.player.MaxHP / 3);
                playerHUD.UpdateHP(true);
                if (playerUnit.player.HP != playerUnit.player.MaxHP || playerUnit.player.Level != -1)
                {
                    yield return dialogBox.TypeDialog($"{playerUnit.player.Name} healed {playerUnit.player.MaxHP / 3} HP! {playerUnit.player.Name} is now at {playerUnit.player.HP} HP");
                }
                
                yield return StartCoroutine(EndBattleCoroutine(true, false)); //end the battle because the player has won
                yield break;
            }
        }
        else if (item.ItemType == ItemType.RecoveryItem) //if the item is a recovery item
        {
            exp += .03f; //increment experience because the player healed themselves
            playerUnit.player.GainExperience(.3f);
            
            //heal the player
            int health = playerUnit.player.HP; 
            playerUnit.player.Heal(item.GetItemModifier(playerUnit.player.Level));
            int healAmount = playerUnit.player.HP - health;
            playerHUD.UpdateHP(true);
            
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} healed {healAmount} HP! {playerUnit.player.HP} HP remaining."); 
        }
        else if (item.ItemType == ItemType.DefenseItem) //if the item is a defense item
        {
            exp += .04f; //increment experience because the player increased their defense
            playerUnit.player.GainExperience(.4f);
            
            //add defense to the player
            int defense = playerUnit.player.AddDefense(item.GetItemModifier(playerUnit.player.Level));
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name}'s defense increased by {defense} DEF! They now have {playerUnit.player.Defense} Total DEF.");
        }
        else //if the item is invalid
        {
            Debug.LogError("Invalid item type.");
        }
        
        if(playerUnit.player.Level < enemyUnit.Enemy.Level) //if the player is a lower level than the enemy
        {
            exp += .05f; //increment experience because the player is using items agaisnt a higher level enemy
        }
        yield return GainExperience(exp, true, true);
        yield return dialogDelay;
        
        state = BattleState.BUSY;
        isInputBlocked = false; // Unblock input
    }
    
    #endregion

    #region Enemy Logic
    
    /// <summary>
    /// the enemy's turn to move, this is where the enemy will choose to use an item.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyMove() 
    {
        state = BattleState.ENEMY_MOVE;
        isInputBlocked = true; // Block input
         
        if (playerUnit.player.HP <= 0) //redudant check to make sure the player has health
        {
            Debug.Log($"{enemyUnit.player.Name} attempted to play turn, but {playerUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(false, false));
            yield break;
        }
        if (enemyUnit.Enemy.HP <= 0) //redudant check to make sure the enemy has health
        {
            Debug.Log($"{enemyUnit.player.Name} attempted to play turn, but {enemyUnit.player.Name} has no health");
            StartCoroutine(EndBattleCoroutine(true, false));
            yield break;
        }

        float exp = .02f; //experience gained throughout a enemies move, can increment based on certain conditions
        float playerExp = 0.01f;  //experience the player has gained by having an enemy make a move on them

        //get the items the enemy has, and sort them by type
        var items = enemyUnit.Enemy.GetItems().Where(i => i != null).ToList();
        var recoveryItems = items.Where(i => i.ItemType == ItemType.RecoveryItem).ToList();
        var defenseItems = items.Where(i => i.ItemType == ItemType.DefenseItem).ToList();
        var attackItems = items.Where(i => i.ItemType == ItemType.AttackItem).ToList();

        ItemBase itemToUse = null;
        
        //determine the health thresholds for the enemy to use items
        int defHealth = enemyUnit.Enemy.MaxHP / 2; //50% health threshold for the enemy to use a defense item
        int healHealth = enemyUnit.Enemy.MaxHP / 3; //33% health threshold for the enemy to use a recovery item
        int instantHealth = enemyUnit.Enemy.MaxHP / 4; //25% health threshold for the enemy to always use a recovery item

        if (enemyUnit.Enemy.HP <= healHealth && recoveryItems.Count > 0) //if the enemy is below 33% health, they will prefer to use a recovery item
        {
            if (enemyUnit.Enemy.HP <= instantHealth) //if the enemy is below 25% health, they will always use a recovery item
            {
                itemToUse = recoveryItems[Random.Range(0, recoveryItems.Count)];
            } else if (Random.Range(0, 2) == 0) //otherwise, they will randomize between using a recovery item or an attack item
            {
                itemToUse = recoveryItems[Random.Range(0, recoveryItems.Count)];
            }
            else if (attackItems.Count > 0)
            {
                itemToUse = attackItems[Random.Range(0, attackItems.Count)];
            }
        }
        else if (enemyUnit.Enemy.HP <= defHealth && defenseItems.Count > 0) //if the enemy is below 50% health, they will prefer to use a defense item
        {
            if (Random.Range(0, 2) == 0) //randomize between using a defense item or an attack item
            {
                itemToUse = defenseItems[Random.Range(0, defenseItems.Count)];
            }
            else if (attackItems.Count > 0) //if the enemy has no defense items, they will use an attack item
            {
                itemToUse = attackItems[Random.Range(0, attackItems.Count)];
            }
        } else if (attackItems.Count > 0) //if the enemy is above 50% health, they will prefer to use an attack item
        {
            itemToUse = attackItems[Random.Range(0, attackItems.Count)];
        }

        if (itemToUse == null) //if the enemy has no item after their preferred checks, they will attempt to use any item. (recovery -> defense -> attack)
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

        if (itemToUse != null) //if the enemy has an item to use
        {
            //use the item
            Debug.Log($"{enemyUnit.Enemy.Name} used: {itemToUse.Name}");
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} used {itemToUse.Name}! {itemToUse.GetItemTypeStr(enemyUnit.Enemy.Level)}");
            yield return dialogDelay;
            
            if (itemToUse.ItemType == ItemType.AttackItem) //if the enemies' item is an attack item
            {
                if (playerUnit.player.Level != -1) //and the player is not in debug mode
                {
                    exp += .04f; //increment experience because the enemy attacked the player
                    playerExp = .03f; //increment experience because the player was attacked by the enemy
                    
                    //take damage from the enemy
                    int health = playerUnit.player.HP;
                    bool playerDefeated = playerUnit.player.TakeDamage(itemToUse, enemyUnit.Enemy);
                    int damageTaken = health - playerUnit.player.HP;
                    playerHUD.UpdateHP(true);
                    yield return dialogBox.TypeDialog($"{playerUnit.player.Name} took {damageTaken} damage! {playerUnit.player.HP} HP remaining.");
                    
                    if (playerDefeated) //if the player has been defeated
                    {
                        StartCoroutine(EndBattleCoroutine(false, false)); //end the battle because the player has lost
                        yield break;
                    }
                }
                else
                {
                   yield return dialogBox.TypeDialog($"{playerUnit.player.Name} is in debug mode. Taking no damage.");
                }
            }
            else if (itemToUse.ItemType == ItemType.RecoveryItem) //the item is a recovery item
            {
                exp += .02f; //increment experience because the enemy healed themselves
                playerExp = .01f; //increment experience because the player witnessed the enemy heal themselves
                
                //heal the enemy
                int health = enemyUnit.Enemy.HP;
                enemyUnit.Enemy.Heal(itemToUse.GetItemModifier(enemyUnit.Enemy.Level));
                int healAmount = enemyUnit.Enemy.HP - health;
                enemyHUD.UpdateHP(false);
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} healed {healAmount} HP! {enemyUnit.Enemy.HP} HP remaining.");
            }
            else if (itemToUse.ItemType == ItemType.DefenseItem) //the item is a defense item
            {
                exp += .04f; //increment experience because the enemy increased their defense
                playerExp = .02f; //increment experience because the player witnessed the enemy increase their defense
                
                //add defense to the enemy
                int defense = enemyUnit.Enemy.AddDefense(itemToUse.GetItemModifier(enemyUnit.Enemy.Level));
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name}'s defense increased by {defense} DEF! They now have {enemyUnit.Enemy.Defense} Total DEF.");
            }
        }

        yield return GainExperience(playerExp, true, false); //gain experience for the player
        yield return GainExperience(exp, false, true); //gain experience for the enemy
        yield return dialogDelay; 
        state = BattleState.BUSY;
        isInputBlocked = false; // Unblock input
    }
    #endregion
    
    #region Experience Logic
    /// <summary>
    /// gain experience for the player or enemy, this is called when the player or enemy has used an item.
    /// </summary>
    /// <param name="exp">the float value of exp to add to the player</param>
    /// <param name="isPlayer">whether or not to add this exp to the player or enemy</param>
    /// <param name="wantDisplay">if you want to display to the player you have added EXP</param>
    /// <returns></returns>
    private IEnumerator GainExperience(float exp, bool isPlayer, bool wantDisplay)
    {
        if (isPlayer) //if the player is gaining experience
        {
            if (playerUnit.player.Level != -1) //if the player is not in debug mode
            { 
                if (playerUnit.player.GainExperience(exp))
                {
                    if (wantDisplay)
                    {
                        yield return dialogBox.TypeDialog(
                            $"{playerUnit.player.Name} leveled up to {playerUnit.player.Level}!");
                    }

                    playerHUD.UpdateLevel(true);
                }
                else
                {
                    if (wantDisplay) //if you want to display the experience gained
                    {
                        yield return dialogBox.TypeDialog(
                            $"{playerUnit.player.Name} gained {ToPercentageString(exp)} experience!");
                    }
                }
            }
        }
        else //if the enemy is gaining experience
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
    /// <summary>
    /// this method converts the experience to a percentage string.
    /// because we handle the experience as a float value. (0.0 - 1.0)
    /// </summary>
    /// <param name="experience">the experience you are converting</param>
    /// <returns></returns>
    private static string ToPercentageString(float experience)
    {
        return (experience * 100).ToString("F0"); //convert the experience to a percentage string
    }
    #endregion
    
    #region Action & Update Logic
    
    /// <summary>
    /// display the action choice for the player to choose from,
    /// this is called between each round of the battle.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayActionChoice()
    {
        state = BattleState.PLAYER_ACTION;
        dialogBox.ToggleItemSelector(false);
        dialogBox.ToggleActionSelector(true);
        yield return dialogBox.TypeDialog($"Choose {playerUnit.player.Name}'s action:");
    }
    
    /// <summary>
    /// this method is called in the game controller's update method,
    /// this is called every frame to handle the battle depending on the state of the battle.
    /// </summary>
    public void HandleUpdate()
    {
        if (state == BattleState.PLAYER_ACTION || state == BattleState.ITEM_SELECTION) 
        {
            //if the player is selecting an action or item
            HandleActionSelection();
        }

        if (state == BattleState.PLAYER_MOVE || state == BattleState.REWARD_SELECTION)
        {
            //if the player is selecting an item to use or replace
            HandleItemSelection();
        }
    }
    
    /// <summary>
    /// this method handles the action selection for the player,
    /// this is called when the player is selecting an action to take. (fight or run away)
    /// </summary>
    private void HandleActionSelection()
    {
        if (isInputBlocked) return; //if the input is blocked, return
        
        //handle the player's input
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentAction < 1) currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0) currentAction--;
        }
        
        //update the UI based on the player's input
        dialogBox.UpdateActionSelection(currentAction);

        //if the player selects an action
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state == BattleState.PLAYER_ACTION) //if the player is selecting an action
            {
                if (currentAction == 0) //fight the enemy
                {
                    dialogBox.ToggleActionSelector(false);
                    StartCoroutine(DetermineTurnOrder());
                }
                else if (currentAction == 1) //run away from the enemy
                {
                    dialogBox.ToggleActionSelector(false);
                    StartCoroutine(RunAwayCoroutine());
                }
            }
            else if (state == BattleState.ITEM_SELECTION) //if the player is selecting an item
            {
                if (currentAction == 0) // accept item
                {
                    dialogBox.ToggleActionSelector(false);
                    StartCoroutine(AcceptItem());
                }
                else if (currentAction == 1 && state != BattleState.REWARD_SELECTION) //run away (game end)
                {
                    dialogBox.ToggleActionSelector(false);
                    EndBattleFinalize(true);
                }
            }
        }
    }
    
    /// <summary>
    /// this method handles the item selection for the player,
    /// this is called when the player is selecting an item to use or replace.
    /// </summary>
    private void HandleItemSelection()
    {
        if(isInputBlocked) return; //if the input is blocked, return
        
        //get the player's items
        var items = playerUnit.player.GetItems();
        if (items == null || items.Count == 0)
        {
            Debug.LogError("Item list is null or empty.");
            return;
        }

        //handle the player's input, a lot of edge cases depending on how the user moves the selection (up, down, left, right)
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) //right
        {
            if (currentItem < items.Count - 1)
                currentItem++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) //left
        {
            if (currentItem > 0)
                currentItem--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) //down
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
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) //up
        {
            if (currentItem > 1)
                currentItem -= 2;
        }

        //update the UI based on the player's input
        if (currentItem >= 0 && currentItem < items.Count && items[currentItem] != null)
        {
            var selectedItem = items[currentItem]; 
            dialogBox.UpdateItemSelection(currentItem, selectedItem, playerUnit.player.Level);
        }

        // if the player selects an item
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (items[currentItem] != null)
            {
                if (state == BattleState.REWARD_SELECTION) //if the player is selecting an item to replace
                {
                    StartCoroutine(ReplaceItem());
                }
                else //the player is selecting an item to use on an enemy
                {
                    state = BattleState.BUSY;
                    dialogBox.ToggleItemSelector(false);
                    dialogBox.ToggleDialogText(true);
                    StartCoroutine(PerformPlayerMove()); //perform the move
                }
            }
        }
    }
    #endregion

    #region Item Reward Logic

    /// <summary>
    /// the first method called when the player has defeated the enemy,
    /// the enemy will drop an item and the player will have the option to accept it or run away.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleNewItem()
    {
        state = BattleState.ITEM_SELECTION;
        //get the enemy's items
        var enemyItems = enemyUnit.Enemy.GetItems();
        if (enemyItems.Count > 0) //if the enemy has items
        {
            //'drop' the item from the enemy and give to the player
            droppedItem = enemyItems[Random.Range(0, enemyItems.Count)];
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} dropped {droppedItem.Name} {droppedItem.GetItemTypeStr(enemyUnit.Enemy.Level)}, will you accept?");
            yield return dialogDelay;
            
            //if the player has a full inventory, warn, otherwise display the action choice
            if (playerUnit.player.Inventory.IsFull())
            {
                yield return dialogBox.TypeDialog("Be careful! If you accept, you must replace one of your current items.");
            }
            dialogBox.ChangeActionText("Accept Item", "Run");
            dialogBox.ToggleActionSelector(true);
            isInputBlocked = false; // Unblock input
            
            while (state == BattleState.ITEM_SELECTION){ //wait for the player to make a choice
                yield return null;
            }
        }
        else
        {
            EndBattleFinalize(true);
        }
    }
    
    /// <summary>
    /// if the player accepts the item, this method is called.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AcceptItem()
    {
        isInputBlocked = true; // Block input
        
        //get the player's items
        var items = playerUnit.player.GetItems();
        Debug.Log(items.Count);
        if (!playerUnit.player.Inventory.IsFull()) //if the player has space in their inventory, automatically accept the item
        {
            playerUnit.player.AddItem(droppedItem);
            yield return dialogBox.TypeDialog($"Accepted item: {droppedItem.Name} {droppedItem.GetItemTypeStr(playerUnit.player.Level)}");
        }
        else //if the player has a full inventory, prompt the player to replace an item
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} has dropped {droppedItem.Name}, but your inventory is full. Choose an item to replace:");
            dialogBox.ToggleItemSelector(true);
            dialogBox.SetItemNames(items);
            isInputBlocked = false; // Block input
            state = BattleState.REWARD_SELECTION;
            while (state == BattleState.REWARD_SELECTION) //wait for the player to make a choice
            {
                yield return null;
            }        }
        isInputBlocked = false; // Unblock input
    }
    
    /// <summary>
    /// this method will replace the item the player has selected with the dropped item.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReplaceItem()
    {
        dialogBox.ToggleItemSelector(false);
        dialogBox.ToggleDialogText(true);
        yield return dialogBox.TypeDialog($"Replaced {playerUnit.player.GetItems()[currentItem].Name} {playerUnit.player.GetItems()[currentItem].GetItemTypeStr(playerUnit.player.Level)} with {droppedItem.Name} {droppedItem.GetItemTypeStr(playerUnit.player.Level)}");
        playerUnit.player.ReplaceItem(currentItem, droppedItem);
        EndBattleFinalize(true); //finally end the game
    }
    #endregion

    #region Run Away Logic

    /// <summary>
    /// the player's attempt to run away from the battle.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunAwayCoroutine()
    {
        isInputBlocked = true; // Block input
        dialogBox.gameObject.SetActive(true);
        yield return dialogBox.TypeDialog($"{playerUnit.player.Name} is trying to run away!");
        
        //50% chance of running away
        bool runAway = Random.Range(0, 2) == 0;
        if (runAway) //if the player successfully runs away
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} ran away successfully!");
            yield return dialogDelay;
            StartCoroutine(EndBattleCoroutine(false, true)); //end the battle because the player has run away
        }
        else //if the player fails to run away
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} couldn't run away in time!");
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Name} attacks before {playerUnit.player.Name} can run away!");
            yield return EnemyMove(); //the enemy will attack the player
        }

        yield return dialogDelay;
        yield return StartCoroutine(EndBattleCoroutine(false, true));
        isInputBlocked = false; // Unblock input
    }

    #endregion

    #region End Battle Logic 
    /// <summary>
    /// end the battle, this is called when the battle is over.
    /// </summary>
    /// <param name="playerWins"></param>
    public void EndBattle(bool playerWins)
    {
        if (state == BattleState.END)
            return;
        StartCoroutine(EndBattleCoroutine(playerWins,false));
    }
    /// <summary>
    /// the method that is called when the battle is over,
    /// </summary>
    /// <param name="playerWins">the boolean that must indicate if the player has won</param>
    /// <param name="runaway">the boolean that must indiciate if the player has ran away</param>
    /// <returns></returns>
    private IEnumerator EndBattleCoroutine(bool playerWins, bool runaway) //the method that is called when the battle is over
    {
        state = BattleState.END; 
        if (!runaway) //if the player has not run away
        {
            yield return dialogBox.TypeDialog(playerWins ? $"{playerUnit.player.Name} won the battle!" : $"{playerUnit.player.Name} was defeated...");
        }
        else
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name} ran away!");
        }
        isBattleInProgress = false; 
        if (playerWins && !runaway) //if the player has won and has not run away
        {
            yield return HandleNewItem();
        }
        else if (!playerWins && !runaway) //if the player has lost and has not run away
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name}'s soul was absorbed... Goodbye...");
            EndBattleFinalize(false);
        }
        else //if the player has run away
        {
            yield return dialogBox.TypeDialog($"{playerUnit.player.Name}'s soul was protected...");
            EndBattleFinalize(playerWins);
        }
    }
    /// <summary>
    /// finalize the battle, this is called when the battle is over.
    /// </summary>
    /// <param name="playerWins"></param>
    private void EndBattleFinalize(bool playerWins)
    {
        //reset states
        StartCoroutine(dialogBox.TypeDialog("Returning to the dungeon...")); 
        state = BattleState.END;
        isBattleInProgress = false;
        
        //invoke the event
        OnBattleOver?.Invoke(playerWins);
        if (!playerWins) //if the player has lost
        {
            if(Application.isPlaying)
            {
                Application.Quit();
            }
            else //if the player has won
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
        
        //hide the battle UI and activate the main camera
        HideBattleUI();
        cameraSwitcher.ActivateMainCamera();
        currentAction = 0; // Reset currentAction
    }

    #endregion
    
    #endregion
}