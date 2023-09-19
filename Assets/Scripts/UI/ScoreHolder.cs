using TMPro;
using UnityEngine;

namespace ForiDots.UI
{
    public class ScoreHolder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreValue;

        public void UpdateScoreValue(int scoreValue)
        {
            _scoreValue.text = scoreValue.ToString();
        }
    }
}