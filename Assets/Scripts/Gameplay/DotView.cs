using System;
using DG.Tweening;
using UnityEngine;

namespace ForiDots
{
    /// <summary>
    /// Dot Visual
    /// </summary>
    public class DotView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _visual;
        [SerializeField] private SpriteRenderer _selectVisual;

        public Vector2Int Position => _position;
        private Vector2Int _position;
        
        private GameView _parent;
        private bool _isSelected;
        private Sequence _selectSequence = null;

        private const float VisualStartScale = 0.5f;
        private const float VisualSelectedScale = 0.75f;
        private const float AnimationTime = 0.5f;
        private const float SelectionAlpha = 0.5f;
        
        public void Setup(GameView parent)
        {
            _parent = parent;
        }

        public void SetDotData(Vector2Int position, Color color)
        {
            _position = position;
            
            _visual.color = color;

            _selectVisual.transform.DOScale(0f, 0f);
            _visual.transform.DOScale(VisualStartScale, 0f);

            color.a = SelectionAlpha;
            _selectVisual.color = color;
        }

        public void Select()
        {
            if (_isSelected) return;
            
            _isSelected = true;
            _selectSequence = DOTween.Sequence();
            _selectSequence.Append(_selectVisual.transform.DOScale(VisualSelectedScale, AnimationTime));
        }
        
        public void Deselect()
        {
            if (!_isSelected) return;
            
            _isSelected = false;
            
            if (_selectSequence != null)
            {
                _selectSequence.Kill();
                _selectSequence = null;
            }
            
            _selectVisual.transform.DOScale(0f, 0f);
        }

        public void Hide(Sequence destroySequence)
        {
            _selectVisual.transform.DOScale(0f, 0f);
            
            destroySequence.Join(_visual.transform.DOScale(0f, AnimationTime));
        }
        
        private void OnMouseOver()
        {
            _parent.OnDotOver(_position);
        }
        
        private void OnMouseUp()
        {
            _parent.OnDotUp();
        }
    }
}