#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ShowForPrefabOnlyAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
	using UnityEngine;
	using Sirenix.OdinInspector.Editor;
	using UnityEditor;
	using Sirenix.Utilities;
	using Ubject = UnityEngine.Object;

	/// <summary>
	/// Draws properties marked with <see cref="ShowForPrefabOnlyAttribute"/>.
	/// </summary>
	/// <seealso cref="EnableForPrefabOnlyAttribute"/>
	/// <seealso cref="ShowIfAttribute"/>
	/// <seealso cref="HideIfAttribute"/>
	/// <seealso cref="EnableIfAttribute"/>
	/// <seealso cref="DisableIfAttribute"/>
	[OdinDrawer]
	public class ShowForPrefabOnlyAttributeDrawer : OdinAttributeDrawer<ShowForPrefabOnlyAttribute>
	{
		private class PrefabContext
		{
			public PrefabType PrefabType;
		}
		
        /// <summary>
        /// Draws the property.
        /// </summary>
		protected override void DrawPropertyLayout(InspectorProperty property, ShowForPrefabOnlyAttribute attribute, GUIContent label)
		{
			var contextBuffer = property.Context.Get<PrefabContext>(this, "PrefabContext", (PrefabContext)null);
			var context = contextBuffer.Value;

			if (context == null)
			{
				context = new PrefabContext() { PrefabType = PrefabType.None, };
				contextBuffer.Value = context;

				Ubject parent = null;
				if (property.ParentType.InheritsFrom<Component>())
				{
					parent = ((Component)property.ParentValues[0]).gameObject;
				}
				else if (property.ParentType.InheritsFrom<Ubject>())
				{
					parent = (Ubject)property.ParentValues[0];
				}

				if (parent != null)
				{
					context.PrefabType = PrefabUtility.GetPrefabType(parent);
				}
			}
			
			if (context.PrefabType == PrefabType.None ||
				context.PrefabType == PrefabType.Prefab ||
				context.PrefabType == PrefabType.ModelPrefab ||
				context.PrefabType == PrefabType.MissingPrefabInstance ||
				context.PrefabType == PrefabType.DisconnectedPrefabInstance ||
				context.PrefabType == PrefabType.DisconnectedModelPrefabInstance)
			{
				this.CallNextDrawer(property, label);
			}
		}
	}
}
#endif