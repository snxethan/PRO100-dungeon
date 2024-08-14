using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private EnemyBase _base;
    [SerializeField] private int level;

    public Enemy Enemy { get; private set; }

    public void Setup(bool isPlayer)
    {
        if (isPlayer)
        {

        }
        else
        {
            Enemy = new Enemy(_base, level);
            GetComponent<UnityEngine.UI.Image>().sprite = Enemy.Base.Sprite;
        }
    }
}
