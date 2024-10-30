using UnityEngine;

public class JudgeRoomLogic : IRoomLogic
{
    public RoomType GetRoomType(int random)
    {
        return (random % 100 < 50) ? RoomType.Judge : RoomType.Enemy;
    }

    public int GetMaxCount()
    {
        //Make some code for when we do jazz and edm era
        return 1;
    }
}
