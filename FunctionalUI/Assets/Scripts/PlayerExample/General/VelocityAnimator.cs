using UnityEngine;

namespace PlayerExample
{
    [RequireComponent(typeof(Animator))]
    public class VelocityAnimator : MonoBehaviour
    {
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private Animator _animator;
        private Vector3 _previousFrameLocation;

        private float _velocity;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void HandleAnimation()
        {
            if (Time.timeScale == 0f) return;
            _velocity = (transform.position - _previousFrameLocation).magnitude / Time.deltaTime;
            _previousFrameLocation = transform.position;
            _animator.SetFloat(Velocity, _velocity, 1f, Time.deltaTime);
        }
    }
}