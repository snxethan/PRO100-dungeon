using System;
using UnityEngine;
using System.Collections;

public enum BattleState
{
    START,
    PLAYER_ACTION,
    PLAYER_MOVE,
    ENEMY_MOVE,
    BUSY,
    END
} // Enum for battle states

public class BattleSystem : MonoBehaviour
{
    [Header("Battle Components")] // Header for battle components
    [SerializeField] private BattleUnit enemyUnit; // Reference to the enemy unit
    [SerializeField] private BattleHUD enemyHUD; // Reference to the enemy HUD

    [SerializeField] private BattleUnit playerUnit; // Reference to the player unit
    [SerializeField] private BattleHUD playerHUD; // Reference to the player HUD

    [SerializeField] private GameObject battleCanvas; // Reference to the battle UI canvas
    [SerializeField] private CameraSwitcher cameraSwitcher; // Reference to the CameraSwitcher script

    [SerializeField] private BattleDialogBox dialogBox; // Reference to the BattleDialogBox script

    public event Action<bool> OnBattleOver; // Event to end the battle

    public static BattleSystem Instance { get; private set; } // Singleton instance

    private BattleState state; // Current battle state
    private int currentAction; // Current action index
    private bool isBattleInProgress = false; // Flag to check if a battle is already in progress

    private void Awake() // Singleton pattern implementation
    {
        if (Instance == null) // Check if an instance already exists
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: If you want the BattleSystem to persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure there is only one instance
        }
    }

    private void Start() // Start is called before the first frame update
    {
        if (battleCanvas == null || cameraSwitcher == null || enemyUnit == null || playerUnit == null ||
            enemyHUD == null || playerHUD == null || dialogBox == null) // Check for missing references
        {
            Debug.LogError("BattleSystem: Missing references in the Inspector!");
            return;
        }

        HideBattleUI(); // Ensure battle UI is hidden at the start

        // Ensure Main Camera is activated
        cameraSwitcher.ActivateMainCamera();
    }

    public void StartBattle(EnemyBase enemyBase)
    {
        if (isBattleInProgress)
        {
            Debug.LogWarning("A battle is already in progress. Cannot start a new battle.");
            return;
        }

        isBattleInProgress = true; // Mark that a battle is in progress
        StartCoroutine(SetupBattle(enemyBase)); // Start the battle coroutine
    }

    private IEnumerator SetupBattle(EnemyBase enemyBase)
    {
        state = BattleState.START; // Set the battle state to START

        Debug.Log("Activating Battle Camera...");
        cameraSwitcher.ActivateBattleCamera(); // Switch to the battle camera

        yield return null; // Wait for a frame to ensure camera switch happens smoothly

        Debug.Log("Showing Battle UI...");
        ShowBattleUI(); // Show the UI when a battle starts
        
        Debug.Log("Setting up battle...");
        playerUnit.Setup(true); // Setup the player unit
        enemyUnit.Setup(false, enemyBase); // Setup the enemy unit
        
        playerHUD.SetData(playerUnit.player); // Set player HUD data
        enemyHUD.SetData(enemyUnit.Enemy); // Set enemy HUD data
        
        dialogBox.setItemNames(playerUnit.player.GetItems());
        
        var enemyName = enemyUnit.Enemy.Name; // Get the enemy name
        var enemyType = enemyUnit.Enemy.Base.Type.ToString(); // Get the enemy type
        
        yield return
            StartCoroutine(dialogBox.TypeDialog($"The {enemyType}:\n{enemyName} has appeared!")); // Display dialog
        yield return new WaitForSeconds(1.5f); // Optional delay
        
        DetermineTurnOrder(); // Determine the turn order
        
        PlayerAction();
        
        yield return null; // Allow UI to update

        Debug.Log("Battle setup complete."); // Log battle setup completion
    }

    private void PlayerAction() // Player action phase
    {
        state = BattleState.PLAYER_ACTION; // Set the battle state to PLAYER_ACTION
        StartCoroutine(dialogBox.TypeDialog("Choose an action:")); // Display dialog
        dialogBox.EnableActionSelector(true); // Enable the action selector
    }

    private void PlayerMove() // Player move phase
    {
        state = BattleState.PLAYER_MOVE; // Set the battle state to PLAYER_MOVE
        dialogBox.EnableActionSelector(false); // Disable the action selector
        dialogBox.EnableDialogText(false); // Disable the dialog text
        dialogBox.EnableItemSelector(true); // Enable the move selector
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PLAYER_ACTION) // Check if the battle state is PLAYER_ACTION
            HandleActionSelection(); // Handle player action selection
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) // Check for down arrow key press
        {
            if (currentAction < 1) currentAction++;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) // Check for up arrow key press
        {
            if (currentAction > 0) currentAction--;
        }

        dialogBox.UpdateActionSelection(currentAction); // Update action selection
        if (Input.GetKeyDown(KeyCode.Space)) // Check for Z key press
        {
            if (currentAction == 0) //fight
                PlayerMove();
            else if (currentAction == 1) //run
                OnBattleOver(false);
        }
    }

    public void EndBattle()
    {
        if (state == BattleState.END)
            return;
        StartCoroutine(EndBattleCoroutine());
    }

    private IEnumerator EndBattleCoroutine()
    {
        state = BattleState.END; // Set the battle state to END
        Debug.Log("Ending Battle...");
        yield return null; // Optional: Add a delay or cleanup process here

        Debug.Log("Hiding Battle UI...");
        HideBattleUI(); // Hide the battle UI

        Debug.Log("Switching to Main Camera...");
        cameraSwitcher.ActivateMainCamera(); // Switch back to the main camera
        yield return
            new WaitForSeconds(10f); // 10 second delay so that the player does not immediately encounter another enemy

        isBattleInProgress = false; // Reset the battle in progress flag
    }

    private void HideBattleUI()
    {
        if (battleCanvas != null)
        {
            battleCanvas.SetActive(false); // Hide the battle UI
            dialogBox.EnableActionSelector(false); // Disable the action selector
            dialogBox.EnableItemSelector(false); // Disable the move selector
        }
    }

    private void ShowBattleUI()
    {
        if (battleCanvas != null) battleCanvas.SetActive(true); // Show the battle UI
    }

    private void DetermineTurnOrder()
    {
        if (playerUnit.player.Speed >= enemyUnit.Enemy.Speed)
            Debug.Log("Player goes first!");
        // Handle player's first move
        else
            Debug.Log("Enemy goes first!");
        // Handle enemy's first move
    }
}