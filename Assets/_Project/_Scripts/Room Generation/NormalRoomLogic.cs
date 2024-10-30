using UnityEngine;

public class NormalRoomLogic : IRoomLogic
{
    public RoomType GetRoomType(int random)
    {
        return (random % 100 < 80) ? RoomType.Enemy : RoomType.Elite;
    }

    public int GetMaxCount()
    {
        //Make some code for when we do jazz and edm era
        return 10;
    }
}
