using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText; 
    [SerializeField] Text levelText; 
    [SerializeField] HPBar hpBar;

    public void SetData(PlayerController player) //sets the player data
    {
        Debug.Log("Setting player data...");
        nameText.text = player.name; //sets the name of the player
        levelText.text = "Lvl " + player.Level; //sets the level of the player
        hpBar.SetHP((float)player.Health / player.MaxHealth); //sets the hp of the player
    }


    //FIX FOR ENEMY HUD
    public void SetData(Enemy enemy) //sets the enemy data
    {
        Debug.Log("Setting enemy data..."); 
        nameText.text = enemy.Base.name; //sets the name of the enemy
        levelText.text = "Lvl " + enemy.Level; //sets the level of the enemy
        hpBar.SetHP((float)enemy.HP / enemy.MaxHP); //sets the hp of the enemy
    }
}