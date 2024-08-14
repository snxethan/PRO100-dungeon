using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text playerName;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    public void SetData(PlayerController player)
    {
        playerName.text = player.GetName();
        levelText.text = "Lvl " + player.GetLevel();
        hpBar.SetHP((float)player.GetHealth() / player.GetMaxHealth());
    }


    //FIX FOR ENEMY HUD
    public void SetData(Enemy enemy)
    {
        playerName.text = enemy.Base.Name;
        levelText.text = "Lvl " + enemy.Level;
        hpBar.SetHP((float)enemy.HP / enemy.MaxHP);
    }
}