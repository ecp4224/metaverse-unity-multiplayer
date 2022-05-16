using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource currentSong = null;

    public AudioSource CurrentSong
    {
        get =>  currentSong;
        set => currentSong = value;
    }
}
