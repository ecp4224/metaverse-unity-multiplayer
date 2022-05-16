using UnityEngine;

public class TransitionMusic : MonoBehaviour
{
    [SerializeField] private AudioSource musicToChange = null;

    private void OnTriggerEnter(Collider other)
    {
        bool isAPlayer = other.gameObject.CompareTag("Player");
        if (isAPlayer)
        {
            ChangeSong();
        }
    }

    private void ChangeSong()
    {
        MusicManager mm = FindObjectOfType<MusicManager>();
        if (mm == null) return;
        if (mm.CurrentSong == null)
        {
            mm.CurrentSong = musicToChange;
            mm.CurrentSong.mute = false;
            return;
        }

        mm.CurrentSong.mute = true;
        mm.CurrentSong = musicToChange;
        mm.CurrentSong.mute = false;
    }
}
