//-----------------------------------------------------------------------
// <copyright file="AssetListAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>AssetLists is used on lists and arrays, and replaces the default list drawer with a list of all possible assets with the specified filter.</para>
    /// <para>Use this to both filter and include or exclude assets from a list or an array, without navigating the project window.</para>
    /// </summary>
    /// <remarks>
    /// <para>Asset lists works on all asset types such as materials, scriptable objects, prefabs, custom components, audio, textures etc, and does also show inherited types.</para>
    /// </remarks>
    /// <example>
    /// <para>The following example will display an asset list of all prefabs located in the project window.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
    /// {
    ///     [AssetList]
    ///     public GameObject[] MyPrefabs;
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// <para>The following example shows various use cases for using the AssetListAttribute.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
    /// {
    ///     // Lists all materials located in the '/Assets/MyMaterials/' folder.
    ///     [AssetList(Path = "/MyMaterials")]
    ///     public List&lt;Material&gt; MyMaterials;
    ///
    ///     // Lists all prefabs with the layer name 'MyLayerName' and a MyComponent attached.
    ///     [AssetList(LayerNames = "MyLayerName")]
    ///     public List&lt;MyComponent&gt; MyPrefabs;
	///
	///		// List all prefabs with the layer name 'LayerA' or 'LayerB'.
	///		[AssetList(LayerNames= "LayerA, LayerB")]
	///		public List&lt;GameObject&gt; MyPrefabs;
    ///
    ///     // Lists all assets of type MyScriptableObject which file name starts with 'prefix'.
    ///     [AssetList(AssetNamePrefix = "prefix")]
    ///     public List&lt;MyScriptableObject&gt; MyPrefabs;
    ///
    ///     // Lists all prefabs with the tag 'MyTag' located in the '/Assets/Prefabs/' folder.
    ///     [AssetList(Tags = "MyTag", Path="/Assets/Prefabs/")]
    ///     public List&lt;GameObject&gt; MyPrefabs;
	///
    ///     // Lists all prefabs with the tag 'MyTagA' or 'MyTagB'
    ///     [AssetList(Tags = "MyTagA, MyTagB")]
    ///     public List&lt;GameObject&gt; MyPrefabs;
	///
	///		// List all prefabs with a "SomeComponent" attached.
	///		[AssetList(CustomFilterMethod = "HasSomeComponent")]
    ///     public List&lt;GameObject&gt; MyPrefabs;
	///
	///		private bool HasSomeComponent(GameObject gameObject)
	///		{
	///			return gameObject.GetComponent&lt;SomeComponent&gt;() != null;
	///		}
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AssetListAttribute : Attribute
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="AssetListAttribute"/> class.</para>
        /// </summary>
        public AssetListAttribute()
        {
            this.AutoPopulate = false;
            this.Tags = null;
            this.LayerNames = null;
            this.AssetNamePrefix = null;
            this.CustomFilterMethod = null;
        }

        /// <summary>
        /// <para>If <c>true</c>, all assets found and displayed by the asset list, will autoamatically be added to the list when inspected.</para>
        /// </summary>
        public bool AutoPopulate { get; set; }

        /// <summary>
		/// <para>Comma seperated list of tags to filter the asset list.</para>
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// <para>Filter the asset list to only include assets with a specified layer.</para>
        /// </summary>
        public string LayerNames { get; set; }

        /// <summary>
        /// <para>Filter the asset list to only include assets which name begins with.</para>
        /// </summary>
        public string AssetNamePrefix { get; set; }

        /// <summary>
        /// <para>Filter the asset list to only include assets which is located at the specified path.</para>
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// <para>Filter the asset list to only include assets for which the given filter method returns true.</para>
        /// </summary>
        public string CustomFilterMethod { get; set; }
    }
}