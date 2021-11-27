using UnityEngine;

namespace Wgs.FlipSide
{
    public class Ladder : MonoBehaviour
    {
        private const string LOG_FORMAT = nameof(Ladder) + ".{0} :: {1}";

        [SerializeField] private BoxCollider _topCollider;

        private bool _isClimbing;
        public bool IsClimbing
        {
            get => _isClimbing;
            set
            {
                _isClimbing = value;
                if (_topCollider) _topCollider.enabled = !_isClimbing;
            }
        }

        public Vector3 Forward => transform.forward;
        public Vector3 Position => transform.position;
    }
}
