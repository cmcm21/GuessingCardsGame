using UnityEngine;

public enum ClipId {SFX_CARDS_MATCH,SFX_CARDS_MISMATCH,SFX_CARD_PLACE}

[CreateAssetMenu(fileName = "Audio Data", menuName = "Data/AudioData",order = 1)]
public class AudioData : ScriptableObject
{
    [SerializeField] private AudioObject[] _audioObjects;
    public AudioObject[] audioObjects => _audioObjects;
}

[System.Serializable]
public class AudioObject
{
    public ClipId clipId;
    public AudioClip clip;
}
