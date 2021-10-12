using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wgs.FlipSide;

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

        private void Start()
        {
        }

        private void LateUpdate()
        {
            if (!_characterLocomotion) return;
            
            // _animator.SetFloat("Forward", _characterLocomotion.MoveDirection.magnitude);
            // _animator.SetBool("IsSprinting", _characterLocomotion.TryGetDecorator(out MoveDecorator groundDecorator) && groundDecorator.IsSprinting);
        }
    }
}
