using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Fori.Helpers;
using ForiDots.UI;
using UnityEngine;

namespace ForiDots
{
    /// <summary>
    /// Game field visual - dots view container
    /// </summary>
    public class GameView : MonoBehaviour
    {
        [SerializeField] private DotView _dotsView;
        [SerializeField] private LineView _lineView;
        [SerializeField] private List<Color> _dotsColor;
        [SerializeField] private ScoreHolder _scoreHolder;

        private List<DotView> _dotsViews = new ();
        private GameController _gameController;
        private Sequence _destroySequence = null;
        
        public void Setup(GameController gameController)
        {
            _gameController = gameController;

            var fieldData = gameController.Field;
            foreach (var dotData in fieldData)
            {
                var newDot = Instantiate(_dotsView, transform).GetComponent<DotView>();
                newDot.Setup(this);
                newDot.SetDotData(dotData.Position, GetColorByTypeId(dotData.TypeId));
                _dotsViews.Add(newDot);
            }

            _scoreHolder.UpdateScoreValue(_gameController.CurrentScore);
        }
        
        private void DestroyDots(List<Vector2Int> dotsToDestroy, Action updateAction)
        {
            var fieldData = _gameController.Field;
            _destroySequence = DOTween.Sequence();
            
            foreach (var dotData in fieldData)
            {
                var dotView = _dotsViews.Find(x => x.Position == dotData.Position);
                if (dotsToDestroy.Contains(dotView.Position))
                {
                    dotView.Hide(_destroySequence);
                }
            }

            _destroySequence.OnComplete(() =>
            {
                updateAction?.Invoke();
                _destroySequence = null;
            });
        }

        private void UpdateDots()
        {
            var fieldData = _gameController.Field;
            foreach (var dotData in fieldData)
            {
                var dotView = _dotsViews.Find(x => x.Position == dotData.Position);
                dotView.SetDotData(dotData.Position, GetColorByTypeId(dotData.TypeId));
            }
        }

        private void DeselectAll()
        {
            foreach (var dotView in _dotsViews)
            {
                dotView.Deselect();
            }
        }

        public void OnDotOver(Vector2Int position)
        {
            if (!Input.GetMouseButton(0)) return;
            var needUpdate = _gameController.TryChooseDot(position);
            if (needUpdate)
            {
                SelectChosenDots();
            }
            UpdateLinePositions();
        }
        
        public void OnDotUp()
        {
            var dotsToDestroy = _gameController.OnCursorUp();
            DeselectAll();
            
            DestroyDots(dotsToDestroy, UpdateDots);
            
            UpdateLinePositions();
            _scoreHolder.UpdateScoreValue(_gameController.CurrentScore);
        }

        private void SelectChosenDots()
        {
            var chosenDots = _gameController.ChosenDots;
            if (chosenDots.ContainsDuplicate())
            {
                var targetType = chosenDots[0].TypeId;
                foreach (var dotData in _gameController.Field)
                {
                    if (dotData.TypeId != targetType) continue;
                    
                    var dot = _dotsViews.Find(x => x.Position == dotData.Position);
                    dot.Select();
                }
            }
            else
            {
                foreach (var dotData in chosenDots)
                {
                    var dot = _dotsViews.Find(x => x.Position == dotData.Position);
                    dot.Select();
                }
            }
        }

        private void UpdateLinePositions()
        {
            var positions = new List<Vector3>();
            var typeId = _gameController.ChosenDots.Count > 0 ? _gameController.ChosenDots[^1].TypeId : -1;
            foreach (var dotData in _gameController.ChosenDots)
            {
                var dot = _dotsViews.Find(x => x.Position == dotData.Position);
                positions.Add(dot.transform.position);
            }
            
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            positions.Add(mouseWorldPos);

            _lineView.UpdateLine(positions, GetColorByTypeId(typeId));
        }

        private Color GetColorByTypeId(int typeId)
        {
            return typeId == -1 ? Color.black : _dotsColor[typeId];
        }
    }
}