using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private GameObject health;

    private void Start()
    {

    }

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }
}
