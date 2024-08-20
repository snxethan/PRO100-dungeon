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
    #endregion
    public List<GameObject> rooms;
    public GameObject closedRoom;
    public GameObject trapRoom;
    public GameObject shopRoom;
    public GameObject Dungeon;
    public GameObject go;
    public int numShops;
    public int numTraps;
    public float waitTime;
    private bool exitRoom;

    // Update is called once per frame
    void Update()
    {
        if(waitTime <=0 && exitRoom == false){
            for(int i = 0; i < numShops; i++){
                go = Instantiate(shopRoom, rooms[Random.Range(1, rooms.Count-1)].transform.position, Quaternion.identity);
                go.transform.parent = Dungeon.transform;
            }

            for(int i = 0; i < numTraps; i++){
                go = Instantiate(trapRoom, rooms[Random.Range(1, rooms.Count-1)].transform.position, Quaternion.identity);
                go.transform.parent = Dungeon.transform;
            }
            go = Instantiate(closedRoom, rooms[rooms.Count-1].transform.position, Quaternion.identity);
                    go.transform.parent = Dungeon.transform;
                    exitRoom = true;
        }else{
            waitTime -= Time.deltaTime;
        }
    }
}
