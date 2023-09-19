using System.Collections.Generic;
using UnityEngine;

namespace ForiDots
{
    /// <summary>
    /// Visual of line connected between dots
    /// </summary>
    public class LineView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        
        public void UpdateLine(List<Vector3> linePositions, Color color)
        {
            _lineRenderer.positionCount = linePositions.Count;
            _lineRenderer.SetPositions(linePositions.ToArray());
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
        }
    }
}