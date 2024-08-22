using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    #region Rooms
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    public GameObject leftRightRoom;
    public GameObject topBottomRoom;
    public GameObject leftTopRoom;
    public GameObject rightTopRoom;
    public GameObject rightBottomRoom;
    public GameObject leftBottomRoom;
    public GameObject leftRightTopRoom;
    public GameObject topBottomRightRoom;
    public GameObject topBottomLeftRoom;
    public GameObject leftRightBottomRoom;
    
    public GameObject[] roomsU;
    public GameObject[] roomsD;
    public GameObject[] roomsL;
    public GameObject[] roomsR;
    public GameObject[] roomsDU;
    public GameObject[] roomsLU;
    public GameObject[] roomsRU;
    public GameObject[] roomsLD;
    public GameObject[] roomsRD;
    public GameObject[] roomsLR;
    public GameObject[] roomsLRD;
    public GameObject[] roomsLRU;
    public GameObject[] roomsRDU;
    public GameObject[] roomsLDU;

    #endregion
    public List<GameObject> rooms;
    public GameObject closedRoom;
    public GameObject trapRoom;
    public GameObject shopRoom;

    public GameObject entryRoom;
    public GameObject Dungeon;
    public GameObject map;
    private GameObject go;
    public float waitTime;
    private bool exitRoom;
    private int rand;
    // Update is called once per frame
    void Update()
    {
        if(waitTime <=0 && exitRoom == false){
            Debug.Log(rooms[0].name);
            map = new GameObject("Dungeon Map");
            go = Instantiate(entryRoom, rooms[0].transform.position, Quaternion.identity);
            go.transform.parent = map.transform;
            for(int i = 1; i < rooms.Count-2; i++){
                go = Instantiate(GetRandomRoom(rooms[i]), rooms[i].transform.position, Quaternion.identity);
                go.transform.parent = map.transform;
            }
            go = Instantiate(closedRoom, rooms[rooms.Count-1].transform.position, Quaternion.identity);
                    go.transform.parent = map.transform;
                    exitRoom = true;
            Destroy(Dungeon);
        }else{
            waitTime -= Time.deltaTime;
        }
    }

    GameObject GetRandomRoom( GameObject randRoom ){
            string roomName = randRoom.name.Substring(0,2);
            Debug.Log(roomName);
            switch(roomName){
                case string s when s.Contains("LRD"):
                    rand = Random.Range(0, roomsLRD.Length);
                    return roomsLRD[rand];
                    break;
                case string s when s.Contains("LDU"):
                    rand = Random.Range(0, roomsLDU.Length);
                    return roomsLDU[rand];
                    break;
                case string s when s.Contains("LRU"):
                    rand = Random.Range(0, roomsLRU.Length);
                    return roomsLRU[rand];
                    break;
                case string s when s.Contains("RDU"):
                    rand = Random.Range(0, roomsRDU.Length);
                    return roomsRDU[rand];
                    break;
                case string s when s.Contains("LU"):
                    rand = Random.Range(0, roomsLU.Length);
                    return roomsLU[rand];
                    break; 
                case string s when s.Contains("RU"):
                    rand = Random.Range(0, roomsRU.Length);
                    return roomsRU[rand];
                    break; 
                case string s when s.Contains("DU"):
                    rand = Random.Range(0, roomsDU.Length);
                    return roomsDU[rand];
                    break;
                case string s when s.Contains("LD"):
                    rand = Random.Range(0, roomsLD.Length);
                    return roomsLD[rand];
                    break; 
                case string s when s.Contains("RD"):
                    rand = Random.Range(0, roomsRD.Length);
                    return roomsRD[rand];
                    break;
                case string s when s.Contains("LR"):
                    rand = Random.Range(0, roomsLR.Length);
                    return roomsLR[rand];
                    break;
                case string s when s.Contains("D"):
                    rand = Random.Range(0, roomsD.Length);
                    return roomsD[rand];
                    break;
                case string s when s.Contains("U"):
                    rand = Random.Range(0, roomsU.Length);
                    return roomsU[rand];
                    break;
                case string s when s.Contains("L"):
                    rand = Random.Range(0, roomsL.Length);
                    return roomsL[rand];
                    break;
                case string s when s.Contains("R"):
                    rand = Random.Range(0, roomsR.Length);
                    return roomsR[rand];
                    break; 
                default:
                    Debug.Log(randRoom.name);
                    return null;
                    break;
            }
            
        }
        
}

