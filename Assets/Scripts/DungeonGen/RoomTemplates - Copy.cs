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
    public float waitTime;
    private bool exitRoom;

    // Update is called once per frame
    void Update()
    {
        if(waitTime <=0 && exitRoom == false){
            for(int i = 0; i < rooms.Count; i++){
                if(i == rooms.Count-1){
                    Instantiate(closedRoom, rooms[i].transform.position, Quaternion.identity);
                    exitRoom = true;
                }
            }
        }else{
            waitTime -= Time.deltaTime;
        }
    }
}
