#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="PropertyContextContainer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// <para>Contains a context for an <see cref="InspectorProperty"/>, which offers the ability to address persistent values by key across several editor GUI frames.</para>
    /// <para>Use this in drawers to store contextual editor-only values such as the state of a foldout.</para>
    /// </summary>
    public sealed class PropertyContextContainer
    {
        private Dictionary<string, object> globalContexts = new Dictionary<string, object>();
        private List<DoubleLookupDictionary<Type, string, object>> drawerContexts = new List<DoubleLookupDictionary<Type, string, object>>();
        private List<DoubleLookupDictionary<Type, string, ITemporaryContext>> temporaryContexts = new List<DoubleLookupDictionary<Type, string, ITemporaryContext>>();
        private InspectorProperty property;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyContextContainer"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public PropertyContextContainer(InspectorProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            this.property = property;
        }

        /// <summary>
        /// <para>Gets a global context value for a given key, using a given delegate to generate a default value if the context doesn't already exist.</para>
        /// <para>Global contexts are not associated with any one specific drawer, and so are shared across all drawers for this property.</para>
        /// </summary>
        /// <typeparam name="T">The type of the context value to get.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="getDefaultValue">A delegate for generating a default value.</param>
        /// <returns>The found context.</returns>
        public PropertyContext<T> GetGlobal<T>(string key, Func<T> getDefaultValue)
        {
            PropertyContext<T> result;
            bool isNew;

            if (this.TryGetGlobalConfig(key, out result, out isNew) && isNew)
            {
                result.Value = getDefaultValue();
            }

            return result;
        }

        /// <summary>
        /// <para>Gets a global context value for a given key, using a given default value if the context doesn't already exist.</para>
        /// <para>Global contexts are not associated with any one specific drawer, and so are shared across all drawers for this property.</para>
        /// </summary>
        /// <typeparam name="T">The type of the context value to get.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="defaultValue">The default value to set if the context value doesn't exist yet.</param>
        /// <returns>The found context.</returns>
        public PropertyContext<T> GetGlobal<T>(string key, T defaultValue)
        {
            PropertyContext<T> result;
            bool isNew;

            if (this.TryGetGlobalConfig(key, out result, out isNew) && isNew)
            {
                result.Value = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// <para>Gets a global context value for a given key, and creates a new instance of <see cref="T"/> as a default value if the context doesn't already exist.</para>
        /// <para>Global contexts are not associated with any one specific drawer, and so are shared across all drawers for this property.</para>
        /// </summary>
        /// <typeparam name="T">The type of the context value to get.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <returns>The found context.</returns>
        public PropertyContext<T> GetGlobal<T>(string key) where T : new()
        {
            PropertyContext<T> result;
            bool isNew;

            if (this.TryGetGlobalConfig(key, out result, out isNew) && isNew)
            {
                result.Value = new T();
            }

            return result;
        }

        private bool TryGetGlobalConfig<T>(string key, out PropertyContext<T> context, out bool isNew)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            context = null;
            object value;

            if (this.globalContexts == null)
            {
                this.globalContexts = new Dictionary<string, object>();
            }

            var contexts = this.globalContexts;

            if (contexts.TryGetValue(key, out value))
            {
                isNew = false;
                context = value as PropertyContext<T>;

                if (context == null)
                {
                    throw new InvalidOperationException("Tried to get global property of type " + typeof(T).GetNiceName() + " with key " + key + " on property at path " + this.property.Path + ", but a global property of a different type (" + value.GetType().GetArgumentsOfInheritedOpenGenericClass(typeof(PropertyContext<>))[0].GetNiceName() + ") already existed with the same key.");
                }
            }
            else
            {
                isNew = true;
                context = new PropertyContext<T>();
                contexts[key] = context;
            }

            return true;
        }

        /// <summary>
        /// <para>Gets a context value local to a drawer type for a given key, using a given delegate to generate a default value if the context doesn't already exist.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerInstance">An instance of the drawer type linked to the context value to get.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="getDefaultValue">A delegate for generating a default value.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue>(OdinDrawer drawerInstance, string key, Func<TValue> getDefaultValue)
        {
            if (drawerInstance == null)
            {
                throw new ArgumentNullException("drawerInstance");
            }

            return this.Get(drawerInstance.GetType(), key, getDefaultValue);
        }

        /// <summary>
        /// <para>Gets a context value local to a drawer type for a given key, using a given delegate to generate a default value if the context doesn't already exist.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TDrawer">The type of the drawer.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="getDefaultValue">A delegate for generating a default value.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue, TDrawer>(string key, Func<TValue> getDefaultValue) where TDrawer : OdinDrawer
        {
            return this.Get(typeof(TDrawer), key, getDefaultValue);
        }

        /// <summary>
        /// <para>Gets a context value local to a drawer type for a given key, using a given delegate to generate a default value if the context doesn't already exist.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerType">The type of the drawer.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="getDefaultValue">A delegate for generating a default value.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue>(Type drawerType, string key, Func<TValue> getDefaultValue)
        {
            PropertyContext<TValue> context;
            bool isNew;

            if (this.TryGetDrawerContext(drawerType, key, out context, out isNew) && isNew)
            {
                context.Value = getDefaultValue();
            }

            return context;
        }

        /// <summary>
        /// <para>Gets a context value local to a drawer type for a given key, using a given default value if the context doesn't already exist.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerInstance">An instance of the drawer type linked to the context value to get.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="defaultValue">The default value to set if the context value doesn't exist yet.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue>(OdinDrawer drawerInstance, string key, TValue defaultValue)
        {
            if (drawerInstance == null)
            {
                throw new ArgumentNullException("drawerInstance");
            }

            return this.Get(drawerInstance.GetType(), key, defaultValue);
        }

        /// <summary>
        /// <para>Gets a context value local to a drawer type for a given key, using a given default value if the context doesn't already exist.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TDrawer">The type of the drawer.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="defaultValue">The default value to set if the context value doesn't exist yet.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue, TDrawer>(string key, TValue defaultValue) where TDrawer : OdinDrawer
        {
            return this.Get(typeof(TDrawer), key, defaultValue);
        }

        /// <summary>
        /// <para>Gets a context value local to a drawer type for a given key, using a given default value if the context doesn't already exist.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerType">The type of the drawer.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="defaultValue">The default value to set if the context value doesn't exist yet.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue>(Type drawerType, string key, TValue defaultValue)
        {
            PropertyContext<TValue> context;
            bool isNew;

            if (this.TryGetDrawerContext(drawerType, key, out context, out isNew) && isNew)
            {
                context.Value = defaultValue;
            }

            return context;
        }

        /// <summary>
        /// Gets a context value local to a drawer type for a given key, and creates a new instance of <see cref="T" /> as a default value if the context doesn't already exist.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerInstance">An instance of the drawer type linked to the context value to get.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue>(OdinDrawer drawerInstance, string key) where TValue : new()
        {
            if (drawerInstance == null)
            {
                throw new ArgumentNullException("drawerInstance");
            }

            return this.Get<TValue>(drawerInstance.GetType(), key);
        }

        /// <summary>
        /// Gets a context value local to a drawer type for a given key, and creates a new instance of <see cref="T" /> as a default value if the context doesn't already exist.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TDrawer">The type of the drawer.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue, TDrawer>(string key) where TValue : new() where TDrawer : OdinDrawer
        {
            return this.Get<TValue>(typeof(TDrawer), key);
        }

        /// <summary>
        /// Gets a context value local to a drawer type for a given key, and creates a new instance of <see cref="T" /> as a default value if the context doesn't already exist.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerType">The type of the drawer.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public PropertyContext<TValue> Get<TValue>(Type drawerType, string key) where TValue : new()
        {
            PropertyContext<TValue> context;
            bool isNew;

            if (this.TryGetDrawerContext(drawerType, key, out context, out isNew) && isNew)
            {
                context.Value = new TValue();
            }

            return context;
        }

        private bool TryGetDrawerContext<T>(Type drawerType, string key, out PropertyContext<T> context, out bool isNew)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (drawerType == null)
            {
                throw new ArgumentNullException("drawerType");
            }

            object value;

            var configs = this.GetCurrentDrawerContexts();

            if (configs.TryGetInnerValue(drawerType, key, out value))
            {
                isNew = false;
                context = value as PropertyContext<T>;

                if (context == null)
                {
                    throw new InvalidOperationException("Tried to get drawer property of type " + typeof(T).GetNiceName() + " with key " + key + " for drawer " + drawerType.GetNiceName() + " on property at path " + this.property.Path + ", but a drawer property for the same drawer type, of a different value type (" + value.GetType().GetArgumentsOfInheritedOpenGenericClass(typeof(PropertyContext<>))[0].GetNiceName() + ") already existed with the same key.");
                }
            }
            else
            {
                isNew = true;
                context = new PropertyContext<T>();
                configs[drawerType][key] = context;
            }

            return true;
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, using a given delegate to generate a default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext"/> have <see cref="ITemporaryContext.Reset"/> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerInstance">An instance of the drawer type linked to the context value to get.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="getDefaultValue">A delegate for generating a default value.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue>(OdinDrawer drawerInstance, string key, Func<TValue> getDefaultValue)
        {
            if (drawerInstance == null)
            {
                throw new ArgumentNullException("drawerInstance");
            }

            return this.GetTemporary(drawerInstance.GetType(), key, getDefaultValue);
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, using a given delegate to generate a default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext" /> have <see cref="ITemporaryContext.Reset" /> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TDrawer">The type of the drawer.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="getDefaultValue">A delegate for generating a default value.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue, TDrawer>(string key, Func<TValue> getDefaultValue) where TDrawer : OdinDrawer
        {
            return this.GetTemporary(typeof(TDrawer), key, getDefaultValue);
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, using a given delegate to generate a default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext" /> have <see cref="ITemporaryContext.Reset" /> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerType">The type of the drawer.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="getDefaultValue">A delegate for generating a default value.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue>(Type drawerType, string key, Func<TValue> getDefaultValue)
        {
            TemporaryPropertyContext<TValue> context;
            bool isNew;

            if (this.TryGetTemporaryContext(drawerType, key, out context, out isNew) && isNew)
            {
                context.Value = getDefaultValue();
            }

            return context;
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, using a given default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext"/> have <see cref="ITemporaryContext.Reset"/> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerInstance">An instance of the drawer type linked to the context value to get.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="defaultValue">The default value to set if the context value doesn't exist yet.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue>(OdinDrawer drawerInstance, string key, TValue defaultValue)
        {
            if (drawerInstance == null)
            {
                throw new ArgumentNullException("drawerInstance");
            }

            return this.GetTemporary(drawerInstance.GetType(), key, defaultValue);
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, using a given default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext" /> have <see cref="ITemporaryContext.Reset" /> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TDrawer">The type of the drawer.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="defaultValue">The default value to set if the context value doesn't exist yet.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue, TDrawer>(string key, TValue defaultValue) where TDrawer : OdinDrawer
        {
            return this.GetTemporary(typeof(TDrawer), key, defaultValue);
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, using a given default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext" /> have <see cref="ITemporaryContext.Reset" /> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerType">The type of the drawer.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <param name="defaultValue">The default value to set if the context value doesn't exist yet.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue>(Type drawerType, string key, TValue defaultValue)
        {
            TemporaryPropertyContext<TValue> context;
            bool isNew;

            if (this.TryGetTemporaryContext(drawerType, key, out context, out isNew) && isNew)
            {
                context.Value = defaultValue;
            }

            return context;
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, and creates a new instance of <see cref="T" /> as a default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext" /> have <see cref="ITemporaryContext.Reset" /> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerInstance">An instance of the drawer type linked to the context value to get.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue>(OdinDrawer drawerInstance, string key) where TValue : new()
        {
            if (drawerInstance == null)
            {
                throw new ArgumentNullException("drawerInstance");
            }

            return this.GetTemporary<TValue>(drawerInstance.GetType(), key);
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, and creates a new instance of <see cref="T" /> as a default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext" /> have <see cref="ITemporaryContext.Reset" /> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TDrawer">The type of the drawer.</typeparam>
        /// <param name="key">The key of the context value to get.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue, TDrawer>(string key) where TValue : new() where TDrawer : OdinDrawer
        {
            return this.GetTemporary<TValue>(typeof(TDrawer), key);
        }

        /// <summary>
        /// <para>Gets a temporary context value local to a drawer type for a given key, and creates a new instance of <see cref="T" /> as a default value if the context doesn't already exist.</para>
        /// <para>Temporary context values are reset at the start of every GUI frame; arrays are set to default values, collections are cleared, and context types that implement <see cref="ITemporaryContext" /> have <see cref="ITemporaryContext.Reset" /> called.</para>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="drawerType">The type of the drawer.</param>
        /// <param name="key">The key of the context value to get.</param>
        /// <returns>
        /// The found context.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">drawerInstance is null</exception>
        public TemporaryPropertyContext<TValue> GetTemporary<TValue>(Type drawerType, string key) where TValue : new()
        {
            TemporaryPropertyContext<TValue> context;
            bool isNew;

            if (this.TryGetTemporaryContext(drawerType, key, out context, out isNew) && isNew)
            {
                context.Value = new TValue();
            }

            return context;
        }

        private bool TryGetTemporaryContext<T>(Type drawerType, string key, out TemporaryPropertyContext<T> context, out bool isNew)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (drawerType == null)
            {
                throw new ArgumentNullException("drawerType");
            }

            var contexts = this.GetCurrentTemporaryContexts();

            ITemporaryContext value;

            if (contexts.TryGetInnerValue(drawerType, key, out value))
            {
                isNew = false;
                context = value as TemporaryPropertyContext<T>;

                if (context == null)
                {
                    throw new InvalidOperationException("Tried to get temporary property of type " + typeof(T).GetNiceName() + " with key " + key + " for drawer " + drawerType.GetNiceName() + " on property at path " + this.property.Path + ", but a temporary property for the same drawer type, of a different value type (" + value.GetType().GetArgumentsOfInheritedOpenGenericClass(typeof(PropertyContext<>))[0].GetNiceName() + ") already existed with the same key.");
                }
            }
            else
            {
                isNew = true;
                context = new TemporaryPropertyContext<T>();
                contexts[drawerType][key] = context;
            }

            return true;
        }

        /// <summary>
        /// Resets all temporary contexts for the property.
        /// </summary>
        public void ResetTemporaryContexts()
        {
            if (this.temporaryContexts != null)
            {
                for (int i = 0; i < this.temporaryContexts.Count; i++)
                {
                    foreach (var configSet in this.temporaryContexts[i].GFValueIterator())
                    {
                        foreach (var config in configSet.GFValueIterator())
                        {
                            config.Reset();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Swaps context values with a given <see cref="PropertyContextContainer"/>.
        /// </summary>
        /// <param name="otherContext">The context to swap with.</param>
        public void SwapContext(PropertyContextContainer otherContext)
        {
            // Swap global configs
            {
                var temp = otherContext.globalContexts;
                otherContext.globalContexts = this.globalContexts;
                this.globalContexts = temp;
            }

            // Swap drawer configs
            {
                var temp = otherContext.drawerContexts;
                otherContext.drawerContexts = this.drawerContexts;
                this.drawerContexts = temp;
            }

            // Swap temporary configs
            {
                var temp = otherContext.temporaryContexts;
                otherContext.temporaryContexts = this.temporaryContexts;
                this.temporaryContexts = temp;
            }
        }

        private DoubleLookupDictionary<Type, string, object> GetCurrentDrawerContexts()
        {
            while (this.drawerContexts.Count <= this.property.DrawCount)
            {
                this.drawerContexts.Add(new DoubleLookupDictionary<Type, string, object>());
            }

            return this.drawerContexts[this.property.DrawCount];
        }

        private DoubleLookupDictionary<Type, string, ITemporaryContext> GetCurrentTemporaryContexts()
        {
            while (this.temporaryContexts.Count <= this.property.DrawCount)
            {
                this.temporaryContexts.Add(new DoubleLookupDictionary<Type, string, ITemporaryContext>());
            }

            return this.temporaryContexts[this.property.DrawCount];
        }
    }
}
#endif