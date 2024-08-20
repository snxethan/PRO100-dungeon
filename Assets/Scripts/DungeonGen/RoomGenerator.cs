using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public int openingDirection;
    //1 needs Bottom
    //2 needs Top
    //3 needs Left
    //4 needs right
    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;
    private float waitTime = 5f;
    
    int[] directions = new int[2];
    void Start(){
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }
    void Spawn()
    {
        if(spawned == false){
        switch(openingDirection){
            case 1:
            //opening down
                rand = Random.Range(0, templates.bottomRooms.Length);
                Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
            break;
            case 2:
            //opening up
                rand = Random.Range(0, templates.topRooms.Length);
                Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
            break;
            case 3:
            //opening left
                rand = Random.Range(0, templates.leftRooms.Length);
                Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);
            break;
            case 4:
            //opening right
                rand = Random.Range(0, templates.rightRooms.Length);
                Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
            break;
        }
        spawned = true;
        }
    }
    void OnTriggerEnter2D(Collider2D other){
        directions[0] = openingDirection;
        if(other.CompareTag("SpawnPoint")){
            directions[1] = other.GetComponent<RoomGenerator>().openingDirection;
            /*if(other.GetComponent<RoomGenerator>().spawned == false && spawned == false){
                Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            */
            if(other.GetComponent<RoomGenerator>().spawned == false && spawned == false){switch(directions[0]){
            case 1:
                switch(directions[1]){
                    case 3:
                    Instantiate(templates.leftBottomRoom, transform.position, templates.topBottomLeftRoom.transform.rotation);
                    break;
                    case 4:
                    Instantiate(templates.rightBottomRoom, transform.position, templates.topBottomRightRoom.transform.rotation);
                    break;
                }
            break;
            case 2:
                switch(directions[1]){
                    case 3:
                    Instantiate(templates.leftTopRoom, transform.position, templates.leftTopRoom.transform.rotation);
                    break;
                    case 4:
                    Instantiate(templates.rightTopRoom, transform.position, templates.rightTopRoom.transform.rotation);
                    break;
                }
            break;
            case 3:
                switch(directions[1]){
                    case 1:
                    Instantiate(templates.leftBottomRoom, transform.position, templates.leftBottomRoom.transform.rotation);
                    break;
                    case 2:
                    Instantiate(templates.leftTopRoom, transform.position, templates.leftTopRoom.transform.rotation);
                    break;
                    case 4:
                    Instantiate(templates.leftRightRoom, transform.position, templates.leftRightRoom.transform.rotation);
                    break;
                }
            break;
            case 4:
                switch(directions[1]){
                    case 1:
                    Instantiate(templates.rightBottomRoom, transform.position, templates.rightBottomRoom.transform.rotation);
                    break;
                    case 2:
                    Instantiate(templates.rightTopRoom, transform.position, templates.rightTopRoom.transform.rotation);
                    break;
                    case 3:
                    Instantiate(templates.leftRightRoom, transform.position, templates.leftRightRoom.transform.rotation);
                    break;
                    }
            break;
                }
                Destroy(gameObject);
            }
            
                spawned = true;
        }
            }
    }
