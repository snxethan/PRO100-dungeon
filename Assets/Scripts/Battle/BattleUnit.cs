using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    private EnemyBase _base;
    private int level;
    [SerializeField] bool isPlayer;
    [SerializeField] public BattleHUD hud;
    [SerializeField] public PlayerController player;


    public Enemy Enemy { get; private set; }

    public void Setup(bool isPlayer)
    {
        if (isPlayer)
        {
            hud.SetData(player);
        }
        else
        {
            Enemy = new Enemy(_base, level);
            GetComponent<UnityEngine.UI.Image>().sprite = Enemy.Base.Sprite;
            hud.SetData(Enemy);
        }
    }
}
