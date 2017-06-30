#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="InspectorConfig.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using Utilities;
    using UnityEngine;

    /// <summary>
    /// <para>
    /// Tell Odin which types should be drawn or should not be drawn by Odin.
    /// If a type is drawn by Odin, a single line of code is generated, telling Unity that we have a custom editor for the type.
    /// All generated editors is compiled into a DLL assembly located in Sirenix/Assemblies/Editor/GeneratedEditors.dll</para>
    /// <para>
    /// You can modify which types should be drawn by Odin in the Preferences window found in 'Window -> Odin Preferences -> Editor Types',
    /// or by locating the configuration file stored as a serialized object in the Sirenix folder under 'Odin Inspector/Config/Editor/InspectorConfig'.
    /// </para>
    /// </summary>
    [SirenixEditorConfig("Odin Inspector/Editor Types")]
    public class InspectorConfig : GlobalConfig<InspectorConfig>, ISerializationCallbackReceiver
    {
        [SerializeField, ToggleLeft, Tooltip("Writes a debug message in the console containing information as to why an automatic recompilation occurred.")]
        private bool enableEditorGenerationLogging = false;

        [SerializeField, ToggleLeft, LabelText(" Disable type name conflict warning message")]
        private bool disableTypeNameConflictWarning = false;

        [SerializeField, HideInInspector]
        private bool autoRecompileOnChangesDetected = true;

        [SerializeField, HideInInspector]
        private InspectorDefaultEditors defaultEditorBehaviour = InspectorDefaultEditors.UserTypes | InspectorDefaultEditors.PluginTypes | InspectorDefaultEditors.OtherTypes;

        [SerializeField, HideInInspector]
        private bool processMouseMoveInInspector = true;

        [SerializeField, DisableContextMenu(true, true)]
        private InspectorTypeDrawingConfig drawingConfig = new InspectorTypeDrawingConfig();

        /// <summary>
        /// InspectorDefaultEditors is a bitmask used to tell which types should have an Odin Editor generated.
        /// </summary>
        public InspectorDefaultEditors DefaultEditorBehaviour
        {
            get { return this.defaultEditorBehaviour; }
            set { this.defaultEditorBehaviour = value; }
        }

        // TODO: (TOR) I would make all of this internal. But if you want it to be public you can have the honor of documenting it as well -.-
        public InspectorTypeDrawingConfig DrawingConfig { get { return this.drawingConfig; } }

        /// <summary>
        /// Whether to disable the type name conflict warning that can occur during Odin generated editor compilation.
        /// </summary>
        public bool DisableTypeNameConflictWarning
        {
            get { return this.disableTypeNameConflictWarning; }
            set { this.disableTypeNameConflictWarning = value; }
        }

        /// <summary>
        /// If true, Odin will generate a DLL assembly containing custom drawers for all specified types.
        /// </summary>
        public bool AutoRecompileOnChangesDetected
        {
            get { return this.autoRecompileOnChangesDetected; }
            set { this.autoRecompileOnChangesDetected = value; }
        }

        /// <summary>
        /// If true, a debug message will appear in the console when an automatic editor recompilation occurred, containing information as to why it happened.
        /// </summary>
        public bool EnableEditorGenerationLogging
        {
            get { return this.enableEditorGenerationLogging; }
            set { this.enableEditorGenerationLogging = value; }
        }

        internal bool ProcessMouseMoveInInspector
        {
            get { return this.processMouseMoveInInspector; }
            set { this.processMouseMoveInInspector = value; }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.drawingConfig.UpdateCaches();
        }
    }
}
#endif