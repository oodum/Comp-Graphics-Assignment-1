using UnityEngine;

public class RiffRoomLogic : IRoomLogic
{
    public RoomType GetRoomType(int random)
    {
        return (random % 100 < 75) ? RoomType.Riff : RoomType.Enemy;
    }

    public int GetMaxCount()
    {
        //Make some code for when we do jazz and edm era
        return 2;
    }
}
