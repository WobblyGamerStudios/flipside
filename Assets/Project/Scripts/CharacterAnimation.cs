using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.Locomotion
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private CharacterLocomotion _characterLocomotion;
        
        private Animator _animator;
        
        private void OnValidate()
        {
            if (!_animator) _animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            if (!_animator) _animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            if (!_characterLocomotion) return;
            
            _animator.SetFloat("Forward", _characterLocomotion.MoveDirection.magnitude);
        }
    }
}
