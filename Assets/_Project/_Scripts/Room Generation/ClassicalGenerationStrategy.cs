using UnityEngine;

public class ClassicalGenerationStrategy : IEraGenerationStrategy
{
    private IRoomLogic[] roomLogics;
    private int[] specialClassicalRooms = { 0, 8, 9 };

    public ClassicalGenerationStrategy()
    {
        //Basically what rooms can show up in Classical Era
        roomLogics = new IRoomLogic[]
        {
            new NormalRoomLogic(),
            new EliteRoomLogic(),
            new RiffRoomLogic(),
            new JudgeRoomLogic(),
        };
    }

    public RoomType[] GenerateEra(RoomType[] rooms)
    {
        if(rooms.Length < 10)
        {
            throw new("Must be at least ten rooms big");
        }

        for(int i = 0; i < 10; i++)
        {
            int random = RandomGen.Get("RoomGenerator");
            int index = random % roomLogics.Length;

            rooms[i] = roomLogics[index].GetRoomType(random);
            if(!RoomChecker.CheckRoom(rooms[i], roomLogics, specialClassicalRooms, i))
            {
                rooms[i] = RoomType.Enemy;
            }
            Debug.Log($"iteration {i} is {rooms[i]}");
        }

        RoomChecker.ResetRoomCounts();
        AssignSpecialRooms(rooms);
        return rooms;
    }

    private void AssignSpecialRooms(RoomType[] rooms)
    {
        rooms[0] = RoomType.Tutorial;
        rooms[8] = RoomType.Shop;
        rooms[9] = RoomType.Boss;
    }
}
