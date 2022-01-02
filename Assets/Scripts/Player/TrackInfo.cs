using UnityEngine;

[System.Serializable]
public class TrackInfo 
{
    public PlayerStates state;
    public Quaternion rot;
    public Vector3 position;


    public TrackInfo(PlayerStates state, Vector3 position, Quaternion r)
    {
        this.state = state;
        this.position = position;
        rot = r;
    }
}
