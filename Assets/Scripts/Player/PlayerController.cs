using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask EnemyLayer;

    private bool isMoving;
    private Vector2 input;

    private void Update()
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

    //Needs testing
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
            if (Random.Range(1, 101) <= 10)
            {
                Debug.Log("Enemy Encounter");

            }
        }
    }
}
