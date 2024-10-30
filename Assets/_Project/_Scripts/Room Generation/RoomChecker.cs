using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class RoomChecker
{
    private static Dictionary<RoomType, int> roomCounts = new Dictionary<RoomType, int>();

    public static bool CheckRoom(RoomType room, IRoomLogic[] roomLogics, int[] specialRooms, int index)
    {
        if(specialRooms.Contains(index) || room == RoomType.Enemy) 
        {
            return true;
        }

        if(roomCounts.ContainsKey(room))
        {
            roomCounts[room]++;
        }
        else
        {
            roomCounts[room] = 1;
        }

        foreach(IRoomLogic roomLogic in roomLogics)
        {
            int maxCount = roomLogic.GetMaxCount(); //Get the maximum count for that room type
            int currentCount = roomCounts.ContainsKey(roomLogic.GetRoomType(0)) ? roomCounts[roomLogic.GetRoomType(0)] : 0;

            if(currentCount > maxCount)
            {
                //Return false, make the room a enemy room
                return false;
            }
        }

        return true;
    }

    //Need to reset roomCounts per Era
    public static void ResetRoomCounts()
    {
        roomCounts.Clear();
    }
}
