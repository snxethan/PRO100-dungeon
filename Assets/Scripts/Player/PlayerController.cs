using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    const float offsety = 0.3f;

    public event Action OnEncountered;

    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask EnemyLayer;

    private bool isMoving;
    private Vector2 input;


    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");


            if (input != Vector2.zero)
            {

                
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (isWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
                
                
            }
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
        CheckForEnemy();

    }


    private void onMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, offsety), 0.2f, GameLayer.i.TriggerableLayers);

        foreach (var collider in colliders)
        {
           var triggerable =  collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    private bool isWalkable(Vector3 targetPos)
    {
       if(Physics2D.OverlapCircle(targetPos, 0.05f, solidObjectsLayer) != null)
        {
            return false;
        }
       return true;
    }

    private void CheckForEnemy()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.05f, EnemyLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                OnEncountered();
            }
        }
    }
}
