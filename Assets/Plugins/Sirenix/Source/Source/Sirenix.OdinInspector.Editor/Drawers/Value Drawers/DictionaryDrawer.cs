#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using Sirenix.Serialization;
    using System.Text;

    /// <summary>
    /// Property drawer for <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    [OdinDrawer]
    public class DictionaryDrawer<TDictionary, TKey, TValue> : OdinValueDrawer<TDictionary> where TDictionary : IDictionary<TKey, TValue>
    {
        private const string CHANGE_ID = "DICTIONARY_DRAWER";

        private static readonly bool KeyIsValueType = typeof(TKey).IsValueType;
        private static readonly bool IsSupportedKeyType = InspectorSerializationUtility.IsSupportedDictionaryKeyType(typeof(TKey));

        private static GUIStyle addKeyPaddingStyle;

        private static GUIStyle AddKeyPaddingStyle
        {
            get
            {
                if (addKeyPaddingStyle == null)
                {
                    addKeyPaddingStyle = new GUIStyle("CN Box")
                    {
                        overflow = new RectOffset(0, 0, 1, 0),
                        fixedHeight = 0,
                        stretchHeight = false,
                        padding = new RectOffset(10, 10, 10, 10)
                    };
                }

                return addKeyPaddingStyle;
            }
        }

        private class Context
        {
            public GUIPagingHelper Paging = new GUIPagingHelper();
            public GeneralDrawerConfig Config;
            public bool Toggled;
            public float KeyWidthOffset;
            public bool ShowAddKeyGUI = false;
            public bool? NewKewIsValid;
            public string NewKeyErrorMessage;
            public TKey NewKey;
            public TValue NewValue;
            public DictionaryHandler<TDictionary, TKey, TValue> DictionaryHandler;
            public GUIContent Label;

            public GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
            {
                padding = new RectOffset(7, 20, 3, 3)
            };
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<TDictionary> entry, GUIContent label)
        {
            if (!IsSupportedKeyType)
            {
                var message = entry.Property.Context.Get(this, "error_message", (string)null);
                var detailedMessage = entry.Property.Context.Get(this, "error_message_detailed", (string)null);

                if (message.Value == null)
                {
                    message.Value = "The dictionary key type " + typeof(TKey).GetNiceName() + " is not supported in the inspector.";
                }

                if (detailedMessage.Value == null)
                {
                    var sb = new StringBuilder("The following key types are supported:");

                    sb.AppendLine()
                      .AppendLine();

                    foreach (var type in InspectorSerializationUtility.GetSupportedDictionaryKeyTypes())
                    {
                        sb.AppendLine(type.GetNiceName());
                    }

                    message.Value = sb.ToString();
                }

                SirenixEditorGUI.ErrorMessageBox(message.Value);

                return;
            }

            var context = entry.Property.Context.Get(this, "context", (Context)null);
            if (context.Value == null)
            {
                context.Value = new Context();
                context.Value.Toggled = GeneralDrawerConfig.Instance.OpenListsByDefault;
                context.Value.KeyWidthOffset = 130;
                context.Value.Label = label ?? new GUIContent(typeof(TDictionary).GetNiceName());
            }

            context.Value.DictionaryHandler = (DictionaryHandler<TDictionary, TKey, TValue>)entry.GetDictionaryHandler();
            context.Value.Config = GeneralDrawerConfig.Instance;
            context.Value.Paging.NumberOfItemsPrPage = context.Value.Config.NumberOfItemsPrPage;
            context.Value.ListItemStyle.padding.right = entry.IsEditable ? 20 : 4;

            SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
            {
                context.Value.Paging.Update(elementCount: entry.Property.Children.Count);
                this.DrawToolbar(entry, context.Value);
                context.Value.Paging.Update(elementCount: entry.Property.Children.Count);

                this.DrawAddKey(entry, context.Value);

                float t;
                GUIHelper.BeginLayoutMeasuring();
                if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(entry.Property, this), context.Value.Toggled, out t, context.Value.Config.GUIFoldoutAnimationDuration))
                {
                    var rect = SirenixEditorGUI.BeginVerticalList(false);
                    {
                        var maxWidth = rect.width - 90;
                        rect.xMin = context.Value.KeyWidthOffset + 22;
                        rect.xMax = rect.xMin + 10;
                        context.Value.KeyWidthOffset = context.Value.KeyWidthOffset + SirenixEditorGUI.SlideRect(rect).x;

                        if (Event.current.type == EventType.Repaint)
                        {
                            context.Value.KeyWidthOffset = Mathf.Clamp(context.Value.KeyWidthOffset, 90, maxWidth);
                        }

                        if (context.Value.Paging.ElementCount != 0)
                        {
                            var headerRect = SirenixEditorGUI.BeginListItem(false);
                            {
                                GUILayout.Space(14);
                                if (Event.current.type == EventType.Repaint)
                                {
                                    GUI.Label(headerRect.SetWidth(context.Value.KeyWidthOffset), "Key", SirenixGUIStyles.LabelCentered);
                                    GUI.Label(headerRect.AddXMin(context.Value.KeyWidthOffset), "Value", SirenixGUIStyles.LabelCentered);
                                    SirenixEditorGUI.DrawSolidRect(headerRect.AlignBottom(1), SirenixGUIStyles.BorderColor);
                                }
                            }
                            SirenixEditorGUI.EndListItem();
                        }

                        this.DrawElements(entry, label, context.Value);
                    }
                    SirenixEditorGUI.EndVerticalList();
                }
                SirenixEditorGUI.EndFadeGroup();
                var outerRect = GUIHelper.EndLayoutMeasuring();

                if (t > 0 && Event.current.type == EventType.Repaint)
                {
                    Color col = SirenixGUIStyles.BorderColor;
                    col.a *= t;
                    outerRect.yMin -= 1;
                    SirenixEditorGUI.DrawBorders(outerRect, 1, col);
                    outerRect.width = 1;
                    outerRect.x += context.Value.KeyWidthOffset + 13;
                    SirenixEditorGUI.DrawSolidRect(outerRect, col);
                }
            }
            SirenixEditorGUI.EndIndentedVertical();
        }

        private void DrawAddKey(IPropertyValueEntry<TDictionary> entry, Context context)
        {
            if (entry.IsEditable == false)
            {
                return;
            }

            if (SirenixEditorGUI.BeginFadeGroup(context, context.ShowAddKeyGUI, context.Config.GUIFoldoutAnimationDuration))
            {
                GUILayout.BeginVertical(AddKeyPaddingStyle);
                {
                    if (typeof(TKey) == typeof(string) && context.NewKey == null)
                    {
                        context.NewKey = (TKey)(object)"";
                        context.NewKewIsValid = null;
                    }

                    if (context.NewKewIsValid == null)
                    {
                        context.NewKewIsValid = CheckKeyIsValid(entry, context.NewKey, out context.NewKeyErrorMessage);
                    }

                    GUI.changed = false;

                    context.NewKey = SirenixEditorGUI.DynamicPrimitiveField(GUIHelper.TempContent("Key"), context.NewKey);

                    if (GUI.changed)
                    {
                        EditorApplication.delayCall += () => context.NewKewIsValid = null;
                        GUIHelper.RequestRepaint();
                    }

                    if (SirenixEditorGUI.DynamicPrimitiveFieldCanDraw<TValue>())
                    {
                        context.NewValue = SirenixEditorGUI.DynamicPrimitiveField(GUIHelper.TempContent("Value"), context.NewValue);
                    }

                    GUIHelper.PushGUIEnabled(GUI.enabled && context.NewKewIsValid.Value);
                    if (GUILayout.Button(context.NewKewIsValid.Value ? "Add" : context.NewKeyErrorMessage))
                    {
                        context.DictionaryHandler.SetValue(context.NewKey, context.NewValue);
                        EditorApplication.delayCall += () => context.NewKewIsValid = null;
                        GUIHelper.RequestRepaint();
                    }
                    GUIHelper.PopGUIEnabled();
                }
                GUILayout.EndVertical();
            }
            SirenixEditorGUI.EndFadeGroup();
        }

        private void DrawToolbar(IPropertyValueEntry<TDictionary> entry, Context context)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                if (entry.ListLengthChangedFromPrefab) GUIHelper.PushIsBoldLabel(true);

                if (context.Config.HideFoldoutWhileEmpty && context.Paging.ElementCount == 0)
                {
                    GUILayout.Label(context.Label, GUILayoutOptions.ExpandWidth(false));
                }
                else
                {
                    EditorGUI.indentLevel++;
                    context.Toggled = SirenixEditorGUI.Foldout(context.Toggled, context.Label);
                    EditorGUI.indentLevel--;
                }

                if (entry.ListLengthChangedFromPrefab) GUIHelper.PopIsBoldLabel();

                GUILayout.FlexibleSpace();

                // Item Count
                if (context.Config.ShowItemCount)
                {
                    if (entry.ValueState == PropertyValueState.CollectionLengthConflict)
                    {
                        int min = entry.Values.Min(x => x.Count);
                        int max = entry.Values.Max(x => x.Count);
                        GUILayout.Label(min + " / " + max + " items", EditorStyles.centeredGreyMiniLabel);
                    }
                    else
                    {
                        GUILayout.Label(context.Paging.ElementCount == 0 ? "Empty" : context.Paging.ElementCount + " items", EditorStyles.centeredGreyMiniLabel);
                    }
                }

                bool hidePaging =
                        context.Config.HidePagingWhileCollapsed && context.Toggled == false ||
                        context.Config.HidePagingWhileOnlyOnePage && context.Paging.PageCount == 1;

                if (!hidePaging)
                {
                    var wasEnabled = GUI.enabled;
                    bool pagingIsRelevant = context.Paging.IsEnabled && context.Paging.PageCount != 1;

                    GUI.enabled = wasEnabled && pagingIsRelevant && !context.Paging.IsOnFirstPage;
                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.ArrowLeft, true))
                    {
                        if (Event.current.button == 0)
                        {
                            context.Paging.CurrentPage--;
                        }
                        else
                        {
                            context.Paging.CurrentPage = 0;
                        }
                    }

                    GUI.enabled = wasEnabled && pagingIsRelevant;
                    var width = GUILayoutOptions.Width(10 + context.Paging.PageCount.ToString().Length * 10);
                    context.Paging.CurrentPage = EditorGUILayout.IntField(context.Paging.CurrentPage + 1, width) - 1;
                    GUILayout.Label(GUIHelper.TempContent("/ " + context.Paging.PageCount));

                    GUI.enabled = wasEnabled && pagingIsRelevant && !context.Paging.IsOnLastPage;
                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.ArrowRight, true))
                    {
                        if (Event.current.button == 0)
                        {
                            context.Paging.CurrentPage++;
                        }
                        else
                        {
                            context.Paging.CurrentPage = context.Paging.PageCount - 1;
                        }
                    }

                    GUI.enabled = wasEnabled && context.Paging.PageCount != 1;
                    if (context.Config.ShowExpandButton)
                    {
                        if (SirenixEditorGUI.ToolbarButton(context.Paging.IsEnabled ? EditorIcons.ArrowDown : EditorIcons.ArrowUp, true))
                        {
                            context.Paging.IsEnabled = !context.Paging.IsEnabled;
                        }
                    }
                    GUI.enabled = wasEnabled;
                }

                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
                {
                    context.ShowAddKeyGUI = !context.ShowAddKeyGUI;
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private static GUIStyle margin;

        private static GUIStyle Margin
        {
            get
            {
                if (margin == null)
                {
                    margin = new GUIStyle() { margin = new RectOffset(8, 0, 0, 0) };
                }
                return margin;
            }
        }

        private void DrawElements(IPropertyValueEntry<TDictionary> entry, GUIContent label, Context context)
        {
            for (int i = context.Paging.StartIndex; i < context.Paging.EndIndex; i++)
            {
                var keyValuePairProperty = entry.Property.Children[i];
                var keyValuePairEntry = (PropertyDictionaryElementValueEntry<TDictionary, TKey, TValue>)keyValuePairProperty.BaseValueEntry;

                Rect rect = SirenixEditorGUI.BeginListItem(false, context.ListItemStyle);
                {
                    GUILayout.BeginHorizontal();
                    //var test = (GUILayoutOption[]).MaxWidth(context.KeyWidthOffset);
                    GUILayout.BeginVertical(GUILayoutOptions.Width(context.KeyWidthOffset));
                    {
                        var keyProperty = keyValuePairProperty.Children[0];

                        if (keyValuePairEntry.HasTempInvalidKey)
                        {
                            GUIHelper.PushColor(Color.red);
                        }

                        InspectorUtilities.DrawProperty(keyProperty, null);

                        if (keyValuePairEntry.HasTempInvalidKey)
                        {
                            GUIHelper.PopColor();
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(Margin);
                    {
                        var valueEntry = keyValuePairProperty.Children[1];
                        var tmp = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 150;
                        InspectorUtilities.DrawProperty(valueEntry, null);
                        EditorGUIUtility.labelWidth = tmp;
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                    if (entry.IsEditable && SirenixEditorGUI.IconButton(new Rect(rect.xMax - 21, rect.y + 2 + ((int)rect.height - 23) / 2, 19, 19), EditorIcons.X))
                    {
                        context.DictionaryHandler.Remove(context.DictionaryHandler.GetKey(0, i));
                        EditorApplication.delayCall += () => context.NewKewIsValid = null;
                        GUIHelper.RequestRepaint();
                    }
                }
                SirenixEditorGUI.EndListItem();
            }

            if (context.Paging.IsOnLastPage && entry.ValueState == PropertyValueState.CollectionLengthConflict)
            {
                SirenixEditorGUI.BeginListItem(false);
                GUILayout.Label(GUIHelper.TempContent("------"), EditorStyles.centeredGreyMiniLabel);
                SirenixEditorGUI.EndListItem();
            }
        }

        private static bool CheckKeyIsValid(IPropertyValueEntry<TDictionary> entry, TKey key, out string errorMessage)
        {
            if (!KeyIsValueType && object.ReferenceEquals(key, null))
            {
                errorMessage = "Key cannot be null.";
                return false;
            }

            string keyStr = InspectorSerializationUtility.GetDictionaryKeyString(key);

            if (entry.Property.Children[keyStr] == null)
            {
                errorMessage = "";
                return true;
            }
            else
            {
                errorMessage = "An item with the same key already exists.";
                return false;
            }
        }

        //void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        //{
        //    if (property.ValueEntry.WeakSmartValue == null)
        //    {
        //        return;
        //    }

        //    if (property.ValueEntry.IsEditable && property.Children.Count > 0)
        //    {
        //        genericMenu.AddItem(new GUIContent("Clear Dictionary"), false, () =>
        //        {
        //            property.ValueEntry.GetDictionaryHandler().Clear();
        //            var context = property.Context.Get(this, "context", (Context)null);

        //            if (context.Value != null)
        //            {
        //                EditorApplication.delayCall += () => context.Value.NewKewIsValid = null;
        //            }
        //        });
        //    }
        //    else
        //    {
        //        genericMenu.AddDisabledItem(new GUIContent("Clear Dictionary"));
        //    }
        //}
    }
}
#endif