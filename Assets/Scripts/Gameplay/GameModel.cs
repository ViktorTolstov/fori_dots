using UnityEngine;

namespace ForiDots
{
    /// <summary>
    /// Model to store local data
    /// </summary>
    public class GameModel
    {
        private const string MaxScoreKey = "MaxScore";

        public int CurrentScore => PlayerPrefs.HasKey(MaxScoreKey) ? PlayerPrefs.GetInt(MaxScoreKey) : 0;
        
        public void SaveData(int playerScore)
        {
            PlayerPrefs.SetInt(MaxScoreKey, playerScore);
            PlayerPrefs.Save();
        }
    }
}