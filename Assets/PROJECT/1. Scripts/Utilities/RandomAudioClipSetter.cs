using UnityEngine;

public class RandomAudioClipSetter : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;

    public AudioClip GetRandomClip()
    {
        AudioClip randomClip = null;
        if (_audioClips != null)
        {
            int random = Random.Range(0, _audioClips.Length);
            randomClip = _audioClips[random];
            _audioSource.clip = randomClip;
            return randomClip;
        }
        return randomClip;
    }
}
