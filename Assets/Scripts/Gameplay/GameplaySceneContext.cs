using UnityEngine;

namespace ForiDots
{
    /// <summary>
    /// Custom realization of Context
    /// </summary>
    public class GameplaySceneContext : MonoBehaviour
    {
        [SerializeField] private GameView _gameView;
        
        private GameController _gameController;
        private GameModel _gameModel;
        
        private void Start()
        {
            _gameModel = new GameModel();

            _gameController = new GameController();
            _gameController.Setup(_gameModel);

            _gameView.Setup(_gameController);
        }
    }
}

