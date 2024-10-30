using UnityEngine;

public interface IRoomLogic
{
    RoomType GetRoomType(int random);
    int GetMaxCount();
}
