using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    #region fields
    [SerializeField] Text nameText; 
    [SerializeField] Text levelText; 
    [SerializeField] HPBar hpBar;
    #endregion

    #region properties
    private PlayerController player;
    private Enemy enemy;
    #endregion

    #region set data
    /// this method sets the data of the player
    /// <param name="_player">the player object</param>
    public void SetData(PlayerController _player) //sets the player data
    {
        player = _player;
        Debug.Log("Setting player data...");
        nameText.text = player.Name; //sets the name of the player
        if (player.Level == -1)
        {
            levelText.text = "LVL \u221e"; //sets the level of the player
        }
        else
        {
            levelText.text = "LVL " + player.Level; //sets the level of the player
        }
        hpBar.SetHP((float)player.HP / player.MaxHP); //sets the hp of the player
    }

    /// this method sets the data of the enemy
    /// <param name="_enemy">the enemy object</param>
    public void SetData(Enemy _enemy) //sets the enemy data
    {
        enemy = _enemy;
        Debug.Log("Setting enemy data...");
        nameText.text = enemy.Name; //sets the name of the enemy
        levelText.text = "LVL " + enemy.Level; //sets the level of the enemy
        hpBar.SetHP((float)enemy.HP / enemy.MaxHP); //sets the hp of the enemy
    }
    #endregion

    #region update data
    /// this method updates the hp of the player or the enemy
    /// <param name="isPlayer">bool to decide if the player or enemy HP should be updated</param>
    public void UpdateHP(bool isPlayer)
    {
        if (isPlayer)
        {
            hpBar.SetHP((float)player.HP / player.MaxHP); // Update player's HP bar
        }
        else
        {
            hpBar.SetHP((float)enemy.HP / enemy.MaxHP); // Update enemy's HP bar
        }
    }

    /// this method updates the level of the player or the enemy
    /// <param name="isPlayer">bool to decide if the player or enemy level should be updated</param>
    public void UpdateLevel(bool isPlayer)
    {
        if (isPlayer)
        {
            levelText.text = "LVL " + player.Level; // Update player's level
        }
        else
        {
            levelText.text = "LVL " + enemy.Level; // Update enemy's level
        }
    }
    #endregion
}