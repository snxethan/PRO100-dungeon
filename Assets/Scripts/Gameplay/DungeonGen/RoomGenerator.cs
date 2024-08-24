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
    private GameObject Dungeon;
    private GameObject go;
    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;
    private float waitTime = 2f;
    
    int[] directions = new int[2];
    // Initializes the RoomGenerator by setting up the templates, dungeon, and invoking the Spawn method after a short delay.
    void Start(){
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Dungeon = GameObject.FindWithTag("Dungeon");
        Invoke("Spawn", 0.1f);
    }
    void Spawn()
    {
        try{
        if(spawned == false){
        switch(openingDirection){
            case 1:
            //opening down
                rand = Random.Range(0, templates.bottomRooms.Length);
                go = Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
                go.transform.parent = Dungeon.transform;
            break;
            case 2:
            //opening up
                rand = Random.Range(0, templates.topRooms.Length);
                go = Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
                go.transform.parent = Dungeon.transform;
            break;
            case 3:
            //opening left
                rand = Random.Range(0, templates.leftRooms.Length);
                go = Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);
                go.transform.parent = Dungeon.transform;
            break;
            case 4:
            //opening right
                rand = Random.Range(0, templates.rightRooms.Length);
                go = Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
                go.transform.parent = Dungeon.transform;
            break;
        }
        spawned = true;
        }
        }catch(System.Exception e){
            Debug.Log(e);
        }
    }
    void OnTriggerEnter2D(Collider2D other){
        try{
        directions[0] = openingDirection;
        if(other.CompareTag("SpawnPoint")){
            directions[1] = other.GetComponent<RoomGenerator>().openingDirection;
            if(other.GetComponent<RoomGenerator>().spawned == false && spawned == false){
                switch(directions[0]){
            case 1:
                switch(directions[1]){
                    case 3:
                    go = Instantiate(templates.leftBottomRoom, transform.position, templates.leftBottomRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                    case 4:
                    go = Instantiate(templates.rightBottomRoom, transform.position, templates.rightBottomRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                }
            break;
            case 2:
                switch(directions[1]){
                    case 3:
                   go = Instantiate(templates.leftTopRoom, transform.position, templates.leftTopRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                    case 4:
                    go = Instantiate(templates.rightTopRoom, transform.position, templates.rightTopRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                }
            break;
            case 3:
                switch(directions[1]){
                    case 1:
                    go = Instantiate(templates.leftBottomRoom, transform.position, templates.leftBottomRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                    case 2:
                    go = Instantiate(templates.leftTopRoom, transform.position, templates.leftTopRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                    case 4:
                    go=Instantiate(templates.leftRightRoom, transform.position, templates.leftRightRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                }
            break;
            case 4:
                switch(directions[1]){
                    case 1:
                    go = Instantiate(templates.rightBottomRoom, transform.position, templates.rightBottomRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                    case 2:
                    go = Instantiate(templates.rightTopRoom, transform.position, templates.rightTopRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                    case 3:
                    go = Instantiate(templates.leftRightRoom, transform.position, templates.leftRightRoom.transform.rotation);
                    go.transform.parent = Dungeon.transform;
                    break;
                    }
            break;
                }
                Destroy(gameObject);
            }
            
                spawned = true;
        }
        }catch(System.Exception e){
            Debug.Log(e);
        }
            }
    }
