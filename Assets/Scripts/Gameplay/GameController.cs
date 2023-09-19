using System.Collections.Generic;
using System.Linq;
using Fori.Helpers;
using UnityEngine;

namespace ForiDots
{
    /// <summary>
    /// Data Class to store dot data
    /// </summary>
    public class DotData
    {
        public Vector2Int Position;
        public int TypeId;
        
        public DotData(Vector2Int position, int typeId)
        {
            Position = position;
            TypeId = typeId;
        }
    }
    
    /// <summary>
    /// Controller of game field
    /// </summary>
    public class GameController
    {
        private const int FieldSize = 6;
        private const int DotTypeCount = 5;

        public List<DotData> Field => _field;
        private List<DotData> _field = new ();

        public List<DotData> ChosenDots => _chosenDots;
        private List<DotData> _chosenDots = new ();
        
        public int CurrentScore => _currentScore;
        private int _currentScore;

        private DotData _lastChosenDot;
        private GameModel _gameModel;

        public void Setup(GameModel gameModel)
        {
            _gameModel = gameModel;
            _currentScore = gameModel.CurrentScore;
            
            InitializeField();
        }
        
        private void InitializeField()
        {
            for (var i = 0; i < FieldSize; i++)
            {
                for (var j = 0; j < FieldSize; j++)
                {
                    var dotPos = new Vector2Int(i, j);
                    var dotType = GetRandomDotType();
                    var newDotData = new DotData(dotPos, dotType);
                    _field.Add(newDotData);
                }
            }
        }

        private DotData GetDotDataByPos(Vector2Int dotPos)
        {
            return _field.Find(x => x.Position == dotPos);
        }

        public bool TryChooseDot(Vector2Int dotPos)
        {
            if (_chosenDots.ContainsDuplicate()) return false;
            
            var targetDot = GetDotDataByPos(dotPos);
            var isAlreadyChosen = _chosenDots.Contains(targetDot);
            var isAvailableByType = true;
            var isNeighborToPrevDot = true;
            if (_chosenDots.Count > 0)
            {
                var prevDot = _chosenDots[^1];
                isAvailableByType =  prevDot.TypeId == targetDot.TypeId;
                
                var horizontalDistance = Mathf.Abs(targetDot.Position.x - prevDot.Position.x);
                var verticalDistance = Mathf.Abs(targetDot.Position.y - prevDot.Position.y);
                isNeighborToPrevDot = 
                    (horizontalDistance == 1 && verticalDistance == 0) || 
                    (horizontalDistance == 0 && verticalDistance == 1);
            }
            
            if (!isNeighborToPrevDot) return false;
            
            if (!isAlreadyChosen && isAvailableByType)
            {
                _chosenDots.Add(targetDot);
                _lastChosenDot = targetDot;
                return true;
            }

            if (!isAlreadyChosen || _chosenDots.Count <= 1 || targetDot == _lastChosenDot) return false;
            
            if (targetDot == _chosenDots[^2])
            {
                _chosenDots.Remove(_chosenDots[^1]);
                _lastChosenDot = targetDot;
                return true;
            }
            
            _chosenDots.Add(targetDot);
            _lastChosenDot = targetDot;
            return true;
        }

        public List<Vector2Int> OnCursorUp()
        {
            var dotsToDestroy = new List<Vector2Int>();

            if (_chosenDots.Count > 1)
            {
                if (_chosenDots.ContainsDuplicate())
                {
                    var targetType = _chosenDots[0].TypeId;
                    _currentScore += _field.Count(x => x.TypeId == targetType);
                    ClearAllDotsByType(targetType, dotsToDestroy);
                }
                else
                {
                    _currentScore += _chosenDots.Count;
                    foreach (var dotData in _chosenDots)
                    {
                        dotData.TypeId = -1;
                        dotsToDestroy.Add(dotData.Position);
                    }
                }

                MoveDotsDown();
            }

            UpdateEmptyDots();
            _lastChosenDot = null;
            _chosenDots.Clear();
            _gameModel.SaveData(_currentScore);
            return dotsToDestroy;
        }

        private void MoveDotsDown()
        {
            var errorIterator = 0;
            while (IsAnyEmptyNotAbove() && errorIterator < FieldSize + 1)
            {
                TryMoveDotsDown();
                errorIterator++;
            }

            if (errorIterator > FieldSize)
            {
                Debug.LogError("GameController:: possible while infinite loop");
            }
        }

        private void UpdateEmptyDots()
        {
            foreach (var dotData in _field)
            {
                if (dotData.TypeId != -1) continue;
                
                dotData.TypeId = GetRandomDotType();
            }
        }

        private bool IsAnyEmptyNotAbove()
        {
            return _field.Any(dot =>
            {
                var dotPos = dot.Position;
                var isDestroyed = dot.TypeId == -1;
                var prevDot = GetDotDataByPos(new Vector2Int(dotPos.x - 1 , dotPos.y));
                var isPrevDestroyed = prevDot == null || prevDot.TypeId == -1;
                
                return isDestroyed && !isPrevDestroyed;
            });
        }

        private void TryMoveDotsDown()
        {
            for (var i = FieldSize - 2; i >= 0; i--)
            {
                for (var j = 0; j < FieldSize; j++)
                {
                    var currentPos = new Vector2Int(i, j);
                    var belowPos = new Vector2Int(i + 1, j);
                    var currentDot = GetDotDataByPos(currentPos);
                    var belowDot = GetDotDataByPos(belowPos);
                    if (belowDot.TypeId == -1)
                    {
                        belowDot.TypeId = currentDot.TypeId;
                        currentDot.TypeId = -1;
                    }
                }
            }
        }

        private int GetRandomDotType()
        {
            return Random.Range(0, DotTypeCount);
        }

        private void ClearAllDotsByType(int targetType, List<Vector2Int> dotsToDestroy)
        {
            foreach (var dotData in _field.Where(dotData => dotData.TypeId == targetType))
            {
                dotData.TypeId = -1;
                dotsToDestroy.Add(dotData.Position);
            }
        }
    }
}