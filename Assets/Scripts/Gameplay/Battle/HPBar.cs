using UnityEngine;

public class HPBar : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private GameObject health;

    private void Start()
    {
        SetHP(1); // Set the health bar to full at the start
    }

    public void SetHP(float hpNormalized)
    {
        // Ensure the value is between 0 and 1, and is not NaN
        hpNormalized = Mathf.Clamp(hpNormalized, 0f, 1f);

        if (float.IsNaN(hpNormalized))
        {
            Debug.LogError("HP Normalized is NaN! This indicates an invalid calculation.");
            return;
        }

        health.transform.localScale = new Vector3(hpNormalized, 1f, 1f);
    }
}