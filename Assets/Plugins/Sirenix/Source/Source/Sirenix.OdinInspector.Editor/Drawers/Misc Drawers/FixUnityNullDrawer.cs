#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="NullReferenceDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Runtime.Serialization;
    using UnityEditor;
    using UnityEngine;

    [OdinDrawer]
    [DrawerPriority(10, 0, 0)]
    internal sealed class FixUnityNullDrawer<T> : OdinValueDrawer<T> where T : class
    {
        public override bool CanDrawTypeFilter(Type type)
        {
            return !typeof(UnityEngine.Object).IsAssignableFrom(typeof(T));
        }

        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, GUIContent label)
        {
            bool valueNeedsFixing = entry.ValueState == PropertyValueState.NullReference &&
                                    entry.SerializationBackend == SerializationBackend.Unity;

            if (valueNeedsFixing && Event.current.type == EventType.Layout)
            {
                Type type = typeof(T);

                for (int i = 0; i < entry.ValueCount; i++)
                {
                    object value;

                    if (type.IsArray)
                    {
                        long[] lengths = new long[type.GetArrayRank()];
                        value = Array.CreateInstance(type.GetElementType(), lengths);
                    }
                    else if (type.GetConstructor(Type.EmptyTypes) != null)
                    {
                        value = Activator.CreateInstance(type);
                    }
                    else if (!type.IsAbstract)
                    {
                        value = FormatterServices.GetUninitializedObject(type);
                    }
                    else
                    {
                        Debug.LogError("Abstract type or interface is backed by Unity serialization and is null. This should never happen. Blame Tor.");
                        value = null;
                        goto CALL_NEXT;
                    }

                    entry.WeakValues.ForceSetValue(i, value);
                }

                entry.ApplyChanges();

                var tree = entry.Property.Tree;

                if (tree.UnitySerializedObject != null)
                {
                    tree.UnitySerializedObject.ApplyModifiedPropertiesWithoutUndo();
                    Undo.RecordObjects(tree.UnitySerializedObject.targetObjects, "Odin inspector value changed");
                }

                entry.Property.Update(true);
            }

            CALL_NEXT:

            this.CallNextDrawer(entry, label);
        }
    }
}
#endif