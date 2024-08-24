using UnityEngine;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public float startTime;

    
    private float timeRemaining;

    
    public float TimeRemaining => timeRemaining;

    
    public void StartCountdown(float countdownTime)
    {
        startTime = countdownTime;
        timeRemaining = startTime;
        StartCoroutine(CountdownCoroutine());
    }

    
    private IEnumerator CountdownCoroutine()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;                      
        }

        
        Debug.Log("Countdown finished!");
    }
}