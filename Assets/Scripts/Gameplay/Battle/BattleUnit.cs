
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    #region fields
    [Header("Unit Info")]
    [SerializeField] private bool isPlayer; //stores if the unit is a player or not
    [SerializeField] public BattleHUD hud; //stores the hud of the unit
    [SerializeField] public PlayerController player; //stores the player
    public Enemy Enemy; //stores the enemy
    #endregion

    /// this method sets up the player or the enemy
    /// <param name="isPlayer">bool if it is the player or enemy that should be edited</param>
    /// <param name="enemyBase"></param>
    public void Setup(bool isPlayer, EnemyBase enemyBase = null)
    {

        if (isPlayer)
        {
            Debug.Log("Setting up player unit...");
            hud.SetData(player); // Set player HUD data
        }
        else
        {
            if (enemyBase == null)
            {
                Debug.LogError("EnemyBase is null! Cannot set up enemy unit.");
                return;
            }

            int playerLevel = player.Level; // Get player level
            int enemyLevel;
            if (playerLevel == -1)
            {
                enemyLevel = 1; // Set enemy level
            }
            else
            {
                int minEnemyLevel = Mathf.Max(1, playerLevel - 1); // Ensure level is at least 1
                int maxEnemyLevel = playerLevel + 1; // Set max level difference

                enemyLevel = Random.Range(minEnemyLevel, maxEnemyLevel + 1);
            }

            Debug.Log($"Starting battle with enemy at level {enemyLevel}...");

            try
            { 
                Enemy = new Enemy(enemyBase, enemyLevel); // Creates a new enemy
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error creating enemy: {ex.Message}");
                return;
            }

            if (Enemy.Base.Sprite != null)
            {
                GetComponent<UnityEngine.UI.Image>().sprite = Enemy.Base.Sprite; // Sets the sprite of the enemy
                Debug.Log("Setting up enemy unit...");
                hud.SetData(Enemy); // Set the data of the enemy in the HUD
            }
            else
            {
                Debug.LogError("Enemy sprite is null! Cannot set up enemy sprite.");
            }
        }
    }
}