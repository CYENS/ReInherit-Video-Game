using UnityEngine;

public class FollowCamera : MonoBehaviour {

    [SerializeField] private Transform m_player;

    // Update is called once per frame
    void Update () {
        transform.position = m_player.transform.position + new Vector3(0, 3, -4);
    }
}
