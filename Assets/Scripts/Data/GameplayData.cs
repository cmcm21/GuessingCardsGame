using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Gameplay data", menuName = "Data/Gameplay data", order = 1)]
    public class GameplayData: ScriptableObject
    {
        [SerializeField] private CardsData _cardsData;
        public CardsData cardsData => _cardsData;

        [SerializeField] private AudioData _audioData;
        public AudioData audioData => _audioData;
    }
}