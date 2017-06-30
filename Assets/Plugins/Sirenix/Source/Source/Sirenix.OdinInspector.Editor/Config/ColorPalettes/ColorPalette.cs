#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ColorPalette.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    internal class ColorPalette
    {
        [SerializeField, PropertyOrder(0)]
        private string name;

        [SerializeField]
        private bool showAlpha = false;

        [SerializeField, PropertyOrder(3)]
        [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowPaging = false, ShowItemCount = true)]
        private List<Color> colors = new List<Color>();

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public List<Color> Colors
        {
            get { return this.colors; }
            set { this.colors = value; }
        }

        public bool ShowAlpha
        {
            get { return this.showAlpha; }
            set { this.showAlpha = value; }
        }
    }
}
#endif