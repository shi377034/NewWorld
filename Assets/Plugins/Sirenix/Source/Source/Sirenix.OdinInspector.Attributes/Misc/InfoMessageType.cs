//-----------------------------------------------------------------------
// <copyright file="InfoBoxType.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
	/// <summary>
	/// Type of info message box.
	/// </summary>
	public enum InfoMessageType
    {
		/// <summary>
		/// Generic message box with no type.
		/// </summary>
		None,

		/// <summary>
		/// Information message box.
		/// </summary>
		Info,

		/// <summary>
		/// Warning message box.
		/// </summary>
		Warning,

		/// <summary>
		/// Error message box.
		/// </summary>
		Error
	}
}