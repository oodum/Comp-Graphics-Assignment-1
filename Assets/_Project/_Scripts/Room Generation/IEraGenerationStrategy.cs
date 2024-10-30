using UnityEngine;

public interface IEraGenerationStrategy
{
    RoomType[] GenerateEra(RoomType[] rooms);
}