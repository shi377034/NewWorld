#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MethodPropertyDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using Utilities;
    using Utilities.Editor;

    /// <summary>
    /// Draws all methods included in a property tree. Note that methods does not get passed to regular drawers.
    /// Methods are currently handled as a special case, this might change in the future.
    /// </summary>
    [DrawerPriority(0, 0, 0.1)]
    public sealed class MethodPropertyDrawer<TInstance> : OdinDrawer
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyImplementation(InspectorProperty property, GUIContent label)
        {
            if (property.Info.PropertyType != PropertyType.Method)
            {
                throw new ArgumentException("A method property drawer of type " + this.GetType().GetNiceName() + " cannot draw properties of type " + property.Info.PropertyType + ".");
            }

            var methodInfo = (MethodInfo)property.Info.MemberInfo;

            //
            // If method is not valid, render error message and return
            //

            var isValid = property.Context.Get(this, "IsValid", (bool?)null);

            if (!isValid.Value.HasValue)
            {
                isValid.Value = methodInfo.GetParameters().Length == 0;
            }

            if (!isValid.Value.Value)
            {
                var error = property.Context.Get(this, "Error", (string)null);

                if (error.Value == null)
                {
                    error.Value = "Cannot show button for method '" + methodInfo.DeclaringType.GetNiceName() + "." + methodInfo.GetNiceName() + "' in the inspector, because it does not have zero parameters.";
                }

                SirenixEditorGUI.ErrorMessageBox(error.Value);
                return;
            }

            //
            // Handle button style and label
            //

            GUIStyle style = property.Context.GetGlobal("ButtonStyle", (GUIStyle)null).Value;

            var buttonAttribute = property.Info.GetAttribute<ButtonAttribute>();
            if (buttonAttribute != null && buttonAttribute.Name != null)
            {
                if (label == null)
                {
                    label = new GUIContent(buttonAttribute.Name);
                }
                else
                {
                    label.text = buttonAttribute.Name;
                }
            }

            //
            // Render button and invoke
            //

            if (style == null ? GUILayout.Button(label) : GUILayout.Button(label, style))
            {
                var caller = property.Context.Get(this, "MethodCaller", (Action<object>)null);

                if (caller.Value == null)
                {
                    caller.Value = EmitUtilities.CreateWeakInstanceMethodCaller(methodInfo);
                }

                var parentValueProperty = property.ParentValueProperty;
                var targets = property.ParentValues;

                for (int i = 0; i < targets.Count; i++)
                {
                    object value = targets[i];

                    if (object.ReferenceEquals(value, null) == false)
                    {
                        try
                        {
                            caller.Value(value);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }

                        if (parentValueProperty != null && value.GetType().IsValueType)
                        {
                            // If it's a struct, it will have been boxed and the invoke call might
                            // have changed the struct and this won't be reflected in the original,
                            // unboxed source struct.

                            // Therefore, set the source value to the boxed struct that we just invoked on.
                            parentValueProperty.ValueEntry.WeakValues[i] = value;
                        }
                    }
                }
            }
        }
    }
}
#endif