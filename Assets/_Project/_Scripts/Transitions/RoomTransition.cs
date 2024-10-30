using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //if player object has tag "Player"
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            //I move the player just because I put the door in the exact same spot in all rooms so it would just go through every room instantly
            player.transform.position = player.transform.position + new Vector3(-14, 0, 0);
            RoomManager.Instance.AdvanceRoom();
        }
    }
}
