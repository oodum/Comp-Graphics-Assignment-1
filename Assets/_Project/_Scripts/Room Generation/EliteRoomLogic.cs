using UnityEngine;

public class EliteRoomLogic : IRoomLogic
{
    public RoomType GetRoomType(int random)
    {
        return (random % 100 < 50) ? RoomType.Elite : RoomType.Enemy;
    }

    public int GetMaxCount()
    {
        //Make some code for when we do jazz and edm era
        return 3;
    }
}