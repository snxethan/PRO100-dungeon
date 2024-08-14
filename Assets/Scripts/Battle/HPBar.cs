using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private GameObject health;

    private void Start()
    {
        SetHP(1); // Set the health bar to full at the start

    }

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }
}
