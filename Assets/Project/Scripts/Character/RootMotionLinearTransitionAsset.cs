using System;
using Animancer;
using Animancer.Editor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wgs.FlipSide
{
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Mixer Transition/Root Motion Linear")]
    public class RootMotionLinearTransitionAsset : AnimancerTransitionAsset<RootMotionLinearMixerTransition>
    {
        [Serializable]
        public class UnShared :
            AnimancerTransitionAsset.UnShared<RootMotionLinearTransitionAsset, RootMotionLinearMixerTransition,
                LinearMixerState>, LinearMixerState.ITransition
        {
        }
    }

    [Serializable]
    public class RootMotionLinearMixerTransition : LinearMixerTransition
    {
        [SerializeField] private bool _applyRootMotion;
        
        public override void Apply(AnimancerState state)
        {
            base.Apply(state);
            state.Root.Component.Animator.applyRootMotion = _applyRootMotion;
        }
        
                #region Drawer
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <inheritdoc/>
        [UnityEditor.CustomPropertyDrawer(typeof(RootMotionLinearMixerTransition), true)]
        public class Drawer : MixerTransitionDrawer
        {
            /************************************************************************************************************************/

            private static GUIContent _SortingErrorContent;
            private static GUIStyle _SortingErrorStyle;

            /// <inheritdoc/>
            // protected override void DoThresholdGUI(Rect area, int index)
            // {
            //     var color = GUI.color;
            //
            //     if (index > 0)
            //     {
            //         var previousThreshold = CurrentThresholds.GetArrayElementAtIndex(index - 1);
            //         var currentThreshold = CurrentThresholds.GetArrayElementAtIndex(index);
            //         if (previousThreshold.floatValue >= currentThreshold.floatValue)
            //         {
            //             if (_SortingErrorContent == null)
            //             {
            //                 // _SortingErrorContent = new GUIContent(AnimancerSettings.Editor.AnimancerGUI.LoadIcon("console.erroricon.sml"))
            //                 // {
            //                 //     tooltip = "Linear Mixer Thresholds must always be unique and sorted in ascending order (click to sort)"
            //                 // };
            //             }
            //
            //             if (_SortingErrorStyle == null)
            //                 _SortingErrorStyle = new GUIStyle(GUI.skin.label)
            //                 {
            //                     padding = new RectOffset(),
            //                 };
            //
            //             // var iconArea = AnimancerSettings.Editor.AnimancerGUI.StealFromRight(ref area, area.height, AnimancerSettings.Editor.AnimancerGUI.StandardSpacing);
            //             // if (GUI.Button(iconArea, _SortingErrorContent, _SortingErrorStyle))
            //             // {
            //             //     AnimancerSettings.Editor.Serialization.RecordUndo(Context.Property);
            //             //     ((LinearMixerTransition)Context.Transition).SortByThresholds();
            //             // }
            //
            //             GUI.color = Color.red;
            //         }
            //     }
            //
            //     base.DoThresholdGUI(area, index);
            //
            //     GUI.color = color;
            // }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            protected override void AddThresholdFunctionsToMenu(UnityEditor.GenericMenu menu)
            {
                const string EvenlySpaced = "Evenly Spaced";

                var count = CurrentThresholds.arraySize;
                if (count <= 1)
                {
                    menu.AddDisabledItem(new GUIContent(EvenlySpaced));
                }
                else
                {
                    var first = CurrentThresholds.GetArrayElementAtIndex(0).floatValue;
                    var last = CurrentThresholds.GetArrayElementAtIndex(count - 1).floatValue;

                    if (last == first)
                        last++;

                    AddPropertyModifierFunction(menu, $"{EvenlySpaced} ({first} to {last})", (_) =>
                    {
                        for (int i = 0; i < count; i++)
                        {
                            CurrentThresholds.GetArrayElementAtIndex(i).floatValue = Mathf.Lerp(first, last, i / (float)(count - 1));
                        }
                    });
                }

                AddCalculateThresholdsFunction(menu, "From Speed",
                    (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity) ? velocity.magnitude : float.NaN);
                AddCalculateThresholdsFunction(menu, "From Velocity X",
                    (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity) ? velocity.x : float.NaN);
                AddCalculateThresholdsFunction(menu, "From Velocity Y",
                    (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity) ? velocity.y : float.NaN);
                AddCalculateThresholdsFunction(menu, "From Velocity Z",
                    (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity) ? velocity.z : float.NaN);
                AddCalculateThresholdsFunction(menu, "From Angular Speed (Rad)",
                    (state, threshold) => AnimancerUtilities.TryGetAverageAngularSpeed(state, out var speed) ? speed : float.NaN);
                AddCalculateThresholdsFunction(menu, "From Angular Speed (Deg)",
                    (state, threshold) => AnimancerUtilities.TryGetAverageAngularSpeed(state, out var speed) ? speed * Mathf.Rad2Deg : float.NaN);
            }

            /************************************************************************************************************************/

            private void AddCalculateThresholdsFunction(UnityEditor.GenericMenu menu, string label,
                Func<Object, float, float> calculateThreshold)
            {
                AddPropertyModifierFunction(menu, label, (property) =>
                {
                    var count = CurrentAnimations.arraySize;
                    for (int i = 0; i < count; i++)
                    {
                        var state = CurrentAnimations.GetArrayElementAtIndex(i).objectReferenceValue;
                        if (state == null)
                            continue;

                        var threshold = CurrentThresholds.GetArrayElementAtIndex(i);
                        var value = calculateThreshold(state, threshold.floatValue);
                        if (!float.IsNaN(value))
                            threshold.floatValue = value;
                    }
                });
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
#endif
        #endregion

    }
}
