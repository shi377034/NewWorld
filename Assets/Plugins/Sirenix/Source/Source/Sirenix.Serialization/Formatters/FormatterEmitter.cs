//-----------------------------------------------------------------------
// <copyright file="FormatterEmitter.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if (UNITY_EDITOR || UNITY_STANDALONE) && !ENABLE_IL2CPP
#define CAN_EMIT
#endif

namespace Sirenix.Serialization
{
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

#if CAN_EMIT

    using System.Reflection.Emit;

#endif

    /// <summary>
    /// Utility class for emitting formatters using the <see cref="System.Reflection.Emit"/> namespace.
    /// <para />
    /// NOTE: Some platforms do not support emitting. Check whether you can emit on the current platform using <see cref="EmitUtilities.CanEmit"/>.
    /// </summary>
    public static class FormatterEmitter
    {
#if CAN_EMIT
        private const string EMIT_ASSEMBLY_NAME = "Sirenix.Serialization.Emitted";
        private static readonly object LOCK = new object();
        private static readonly DoubleLookupDictionary<ISerializationPolicy, Type, Type> EmittedFormatters = new DoubleLookupDictionary<ISerializationPolicy, Type, Type>();

        private static AssemblyBuilder emittedAssembly;
        private static ModuleBuilder emittedModule;

        private delegate void ReadDataEntryMethodDelegate<T>(ref T value, string entryName, EntryType entryType, IDataReader reader, Dictionary<string, int> switchLookup);

        private delegate void WriteDataEntriesMethodDelegate<T>(ref T value, IDataWriter writer);

#endif

        /// <summary>
        /// Gets an emitted formatter for a given type.
        /// <para />
        /// NOTE: Some platforms do not support emitting. On such platforms, this method logs an error and returns null. Check whether you can emit on the current platform using <see cref="EmitUtilities.CanEmit"/>.
        /// </summary>
        /// <param name="type">The type to emit a formatter for.</param>
        /// <param name="policy">The serialization policy to use to determine which members the emitted formatter should serialize. If null, <see cref="SerializationPolicies.Strict"/> is used.</param>
        /// <returns>The type of the emitted formatter.</returns>
        /// <exception cref="System.ArgumentNullException">The type argument is null.</exception>
        public static Type GetEmittedFormatter(Type type, ISerializationPolicy policy)
        {
#if !CAN_EMIT
        Debug.LogError("Cannot use Reflection.Emit on the current platform. The FormatterEmitter class is currently disabled. Check whether emitting is currently possible with EmitUtilities.CanEmit.");
        return null;
#else
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (policy == null)
            {
                policy = SerializationPolicies.Strict;
            }

            Type result = null;

            if (EmittedFormatters.TryGetInnerValue(policy, type, out result) == false)
            {
                lock (LOCK)
                {
                    if (EmittedFormatters.TryGetInnerValue(policy, type, out result) == false)
                    {
                        EnsureAssembly();

                        try
                        {
                            result = EmitCustomFormatter(type, emittedModule, policy);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("The following error occurred while emitting a formatter for the type " + type.Name);
                            Debug.LogException(ex);
                        }

                        EmittedFormatters.AddInner(policy, type, result);
                    }
                }
            }

            return result;
#endif
        }

#if CAN_EMIT

        private static void EnsureAssembly()
        {
            // We always hold the lock in this method

            if (emittedAssembly == null)
            {
                var assemblyName = new AssemblyName(EMIT_ASSEMBLY_NAME);

                assemblyName.CultureInfo = System.Globalization.CultureInfo.InvariantCulture;
                assemblyName.Flags = AssemblyNameFlags.None;
                assemblyName.ProcessorArchitecture = ProcessorArchitecture.MSIL;
                assemblyName.VersionCompatibility = System.Configuration.Assemblies.AssemblyVersionCompatibility.SameDomain;

                emittedAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            }

            if (emittedModule == null)
            {
                bool emitSymbolInfo;

#if UNITY_EDITOR
                emitSymbolInfo = true;
#else
                // Builds cannot emit symbol info
                emitSymbolInfo = false;
#endif

                emittedModule = emittedAssembly.DefineDynamicModule(EMIT_ASSEMBLY_NAME, emitSymbolInfo);
            }
        }

        private static Type EmitCustomFormatter(Type formattedType, ModuleBuilder moduleBuilder, ISerializationPolicy policy)
        {
            Type parentType = typeof(EasyBaseFormatter<>).MakeGenericType(formattedType);
            TypeBuilder formatterTypeBuilder = moduleBuilder.DefineType(EMIT_ASSEMBLY_NAME + "." + formattedType.GetCompilableNiceFullName() + "Formatter", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class, parentType);
            TypeBuilder helperTypeBuilder = moduleBuilder.DefineType(EMIT_ASSEMBLY_NAME + "." + formattedType.GetCompilableNiceFullName() + "FormatterHelper", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);

            formatterTypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            formatterTypeBuilder.SetCustomAttribute(
                new CustomAttributeBuilder(
                    typeof(ObsoleteAttribute).GetConstructor(new[] { typeof(string) }),
                    new object[] { "The type " + formatterTypeBuilder.GetNiceName() + " has been dynamically emitted on demand and should not be directly referenced from user code, as its continued presence is not guaranteed." }));

            Dictionary<string, MemberInfo> serializableMembers = FormatterUtilities.GetSerializableMembersMap(formattedType, policy);
            Dictionary<MemberInfo, List<string>> memberNames = new Dictionary<MemberInfo, List<string>>();

            foreach (var entry in serializableMembers)
            {
                List<string> list;

                if (memberNames.TryGetValue(entry.Value, out list) == false)
                {
                    list = new List<string>();
                    memberNames.Add(entry.Value, list);
                }

                list.Add(entry.Key);
            }

            var dictField = formatterTypeBuilder.DefineField("SwitchLookup", typeof(Dictionary<string, int>), FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly);

            List<Type> neededSerializers = memberNames.Keys.Select(n => FormatterUtilities.GetContainedType(n)).Distinct().ToList();

            Dictionary<Type, MethodInfo> serializerReadMethods = new Dictionary<Type, MethodInfo>(neededSerializers.Count);
            Dictionary<Type, MethodInfo> serializerWriteMethods = new Dictionary<Type, MethodInfo>(neededSerializers.Count);
            Dictionary<Type, FieldBuilder> serializerFields = new Dictionary<Type, FieldBuilder>(neededSerializers.Count);

            foreach (var t in neededSerializers)
            {
                string name = t.Name + "Serializer";
                int counter = 1;

                while (serializerFields.Values.Any(n => n.Name == name))
                {
                    counter++;
                    name = t.Name + "Serializer" + counter;
                }

                Type serializerType = typeof(Serializer<>).MakeGenericType(t);

                serializerReadMethods.Add(t, serializerType.GetMethod("ReadValue", Flags.InstancePublicDeclaredOnly));
                serializerWriteMethods.Add(t, serializerType.GetMethod("WriteValue", Flags.InstancePublicDeclaredOnly, null, new[] { typeof(string), t, typeof(IDataWriter) }, null));
                serializerFields.Add(t, helperTypeBuilder.DefineField(name, serializerType, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly));
            }

            Type readDelegateType = typeof(ReadDataEntryMethodDelegate<>).MakeGenericType(formattedType);
            Type writeDelegateType = typeof(WriteDataEntriesMethodDelegate<>).MakeGenericType(formattedType);

            FieldBuilder readMethodFieldBuilder = helperTypeBuilder.DefineField("ReadMethod", readDelegateType, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly);
            FieldBuilder writeMethodFieldBuilder = helperTypeBuilder.DefineField("WriteMethod", writeDelegateType, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly);

            // We generate a static constructor for our formatter type that initializes our switch lookup dictionary
            {
                var addMethod = typeof(Dictionary<string, int>).GetMethod("Add", Flags.InstancePublic);
                var dictionaryConstructor = typeof(Dictionary<string, int>).GetConstructor(Type.EmptyTypes);

                ConstructorBuilder staticConstructor = formatterTypeBuilder.DefineTypeInitializer();
                ILGenerator gen = staticConstructor.GetILGenerator();

                gen.Emit(OpCodes.Newobj, dictionaryConstructor);                        // Create new dictionary

                int count = 0;

                foreach (var entry in memberNames)
                {
                    foreach (var name in entry.Value)
                    {
                        gen.Emit(OpCodes.Dup);                                          // Load duplicate dictionary value
                        gen.Emit(OpCodes.Ldstr, name);                                  // Load entry name
                        gen.Emit(OpCodes.Ldc_I4, count);                                // Load entry index
                        gen.Emit(OpCodes.Call, addMethod);                              // Call dictionary add
                    }

                    count++;
                }

                gen.Emit(OpCodes.Stsfld, dictField);                                    // Set static dictionary field to dictionary value
                gen.Emit(OpCodes.Ret);                                                  // Return
            }

            // We generate a static constructor for our formatter helper type that initializes our needed Serializer references
            {
                var serializerGetMethod = typeof(Serializer).GetMethod("Get", Flags.StaticPublic, null, new[] { typeof(Type) }, null);
                var typeOfMethod = typeof(Type).GetMethod("GetTypeFromHandle", Flags.StaticPublic, null, new Type[] { typeof(RuntimeTypeHandle) }, null);

                ConstructorBuilder staticConstructor = helperTypeBuilder.DefineTypeInitializer();
                ILGenerator gen = staticConstructor.GetILGenerator();

                foreach (var entry in serializerFields)
                {
                    gen.Emit(OpCodes.Ldtoken, entry.Key);                               // Load type token
                    gen.Emit(OpCodes.Call, typeOfMethod);                               // Call typeof method (this pushes a type value onto the stack)
                    gen.Emit(OpCodes.Call, serializerGetMethod);                        // Call Serializer.Get(Type type) method
                    gen.Emit(OpCodes.Stsfld, entry.Value);                              // Set static serializer field to result of get method
                }

                gen.Emit(OpCodes.Ret);                                                  // Return
            }

            // Now we need to actually create the serializer container type so we can generate the dynamic methods below without getting TypeLoadExceptions up the wazoo
            Type helperType = helperTypeBuilder.CreateType();

            // We generate the ReadDataEntry dynamic method and then call that from our own override method
            //  (it has to be layered through an anonymously hosted dynamic method so we get direct access to private members in other modules)
            {
                MethodInfo readDataEntryAbstractMethod = parentType.GetMethod("ReadDataEntry", Flags.InstancePrivate);
                MethodInfo readDelegateInvoke = readDelegateType.GetMethod("Invoke", Flags.InstancePublic);

                DynamicMethod dynamicReadMethod = GenerateDynamicReadMethod(formattedType, readDataEntryAbstractMethod, serializerFields, memberNames, serializerReadMethods);
                Delegate del1 = dynamicReadMethod.CreateDelegate(readDelegateType);

                /* Now set the static readonly field in the helper type to the delegate value
                 *
                 * The reason that we put the delegates in static fields instead of calling the delegate method directly from the emitted code,
                 * is what seems to be a bug in Unity's garbage collector. In short, the dynamic method will be moved around in memory by the
                 * garbage collector, without the method pointer in the below emitted override method being updated. Hence, calling the emitted
                 * override method after a garbage collection pass is likely to try to execute a random piece of memory as a method.
                 *     (This is what happens as far as we can determine.)
                 *
                 * This causes stacktrace-less and often dump-less crashes, which is not fun. The only way to resolve that issue is to permanently pin the dynamic
                 * method delegate in memory using System.Runtime.InteropServices.GCHandle.Alloc(). This sabotages the GC, though, and is bad practice.
                 * (Though, yes, that really does work to solve the issue, which is a little crazy.)
                 *
                 * That's why we use a static field with a reference to the method delegate instead.
                 */
                helperType.GetField(readMethodFieldBuilder.Name, Flags.StaticPublic).SetValue(null, del1);

                MethodBuilder overrideMethodBuilder1 = formatterTypeBuilder.DefineMethod(readDataEntryAbstractMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, readDataEntryAbstractMethod.ReturnType, readDataEntryAbstractMethod.GetParameters().Select(n => n.ParameterType).ToArray());
                readDataEntryAbstractMethod.GetParameters().ForEach(n => overrideMethodBuilder1.DefineParameter(n.Position + 1, n.Attributes, n.Name));
                ILGenerator gen1 = overrideMethodBuilder1.GetILGenerator();

                gen1.Emit(OpCodes.Ldsfld, readMethodFieldBuilder);                      // Load read method delegate from helper class field
                gen1.Emit(OpCodes.Ldarg, (short)1);                                     // Load value instance reference arg
                gen1.Emit(OpCodes.Ldarg, (short)2);                                     // Load entryName arg
                gen1.Emit(OpCodes.Ldarg, (short)3);                                     // Load entryType arg
                gen1.Emit(OpCodes.Ldarg, (short)4);                                     // Load reader arg
                gen1.Emit(OpCodes.Ldsfld, dictField);                                   // Load switch lookup dictionary
                gen1.Emit(OpCodes.Callvirt, readDelegateInvoke);                        // Invoke read delegate
                gen1.Emit(OpCodes.Ret);                                                 // Return

                formatterTypeBuilder.DefineMethodOverride(overrideMethodBuilder1, readDataEntryAbstractMethod);
            }

            // We generate the WriteDataEntries dynamic method dynamic method and then call that from our own override method
            //  (it has to be layered through an anonymously hosted dynamic method so we get direct access to private members in other modules)
            {
                MethodInfo writeDataEntriesAbstractMethod = parentType.GetMethod("WriteDataEntries", Flags.InstancePrivate);
                MethodInfo writeDelegateInvoke = writeDelegateType.GetMethod("Invoke", Flags.InstancePublic);

                DynamicMethod dynamicWriteMethod = GenerateDynamicWriteMethod(formattedType, writeDataEntriesAbstractMethod, serializerFields, memberNames, serializerWriteMethods);
                Delegate del2 = dynamicWriteMethod.CreateDelegate(writeDelegateType);

                // Now set the static readonly field in the helper type to the delegate value
                helperType.GetField(writeMethodFieldBuilder.Name, Flags.StaticPublic).SetValue(null, del2);

                MethodBuilder overrideMethodBuilder2 = formatterTypeBuilder.DefineMethod(writeDataEntriesAbstractMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, writeDataEntriesAbstractMethod.ReturnType, writeDataEntriesAbstractMethod.GetParameters().Select(n => n.ParameterType).ToArray());
                writeDataEntriesAbstractMethod.GetParameters().ForEach(n => overrideMethodBuilder2.DefineParameter(n.Position + 1, n.Attributes, n.Name));
                ILGenerator gen2 = overrideMethodBuilder2.GetILGenerator();

                gen2.Emit(OpCodes.Ldsfld, writeMethodFieldBuilder);                         // Load write method delegate from helper class field
                gen2.Emit(OpCodes.Ldarg, (short)1);                                         // Load value instance reference arg
                gen2.Emit(OpCodes.Ldarg, (short)2);                                         // Load reader arg
                gen2.Emit(OpCodes.Call, writeDelegateInvoke);                               // Invoke write delegate
                gen2.Emit(OpCodes.Ret);                                                     // Return

                formatterTypeBuilder.DefineMethodOverride(overrideMethodBuilder2, writeDataEntriesAbstractMethod);
            }

            return formatterTypeBuilder.CreateType();
        }

        private static DynamicMethod GenerateDynamicReadMethod(Type formattedType, MethodInfo readDataEntryAbstractMethod, Dictionary<Type, FieldBuilder> serializerFields, Dictionary<MemberInfo, List<string>> memberNames, Dictionary<Type, MethodInfo> serializerReadMethods)
        {
            DynamicMethod methodBuilder = new DynamicMethod("Dynamic_" + readDataEntryAbstractMethod, null, readDataEntryAbstractMethod.GetParameters().Select(n => n.ParameterType).Append(typeof(Dictionary<string, int>)).ToArray(), true);
            ILGenerator gen = methodBuilder.GetILGenerator();

            MethodInfo skipMethod = typeof(IDataReader).GetMethod("SkipEntry", Flags.InstancePublic);
            MethodInfo tryGetValueMethod = typeof(Dictionary<string, int>).GetMethod("TryGetValue", Flags.InstancePublic);

            readDataEntryAbstractMethod.GetParameters().ForEach(n => methodBuilder.DefineParameter(n.Position, n.Attributes, n.Name));
            methodBuilder.DefineParameter(5, ParameterAttributes.None, "switchLookup");

            LocalBuilder lookupResult = gen.DeclareLocal(typeof(int));

            Label defaultLabel = gen.DefineLabel();
            Label switchLabel = gen.DefineLabel();
            Label endLabel = gen.DefineLabel();
            Label[] switchLabels = memberNames.Select(n => gen.DefineLabel()).ToArray();

            gen.Emit(OpCodes.Ldarg_1);                                              // Load entryName string
            gen.Emit(OpCodes.Ldnull);                                               // Load null
            gen.Emit(OpCodes.Ceq);                                                  // Equality check
            gen.Emit(OpCodes.Brtrue, defaultLabel);                                 // If entryName is null, go to default case

            gen.Emit(OpCodes.Ldarg, (short)4);                                      // Load lookup dictionary argument
            gen.Emit(OpCodes.Ldarg_1);                                              // Load entryName string
            gen.Emit(OpCodes.Ldloca, (short)lookupResult.LocalIndex);               // Load address of lookupResult
            gen.Emit(OpCodes.Callvirt, tryGetValueMethod);                          // Call TryGetValue on the dictionary

            gen.Emit(OpCodes.Brtrue, switchLabel);                                  // If TryGetValue returned true, go to the switch case
            gen.Emit(OpCodes.Br, defaultLabel);                                     // Else, go to the default case

            gen.MarkLabel(switchLabel);                                             // Switch starts here
            gen.Emit(OpCodes.Ldloc, lookupResult);                                  // Load lookupResult
            gen.Emit(OpCodes.Switch, switchLabels);                                 // Perform switch on switchLabels

            int count = 0;

            foreach (var member in memberNames.Keys)
            {
                var memberType = FormatterUtilities.GetContainedType(member);

                var propInfo = member as PropertyInfo;
                var fieldInfo = member as FieldInfo;

                gen.MarkLabel(switchLabels[count]);                                 // Switch case for [count] starts here

                // Now we load the instance that we have to set the value on
                gen.Emit(OpCodes.Ldarg_0);                                          // Load value reference

                if (formattedType.IsValueType == false)
                {
                    gen.Emit(OpCodes.Ldind_Ref);                                    // Indirectly load value of reference
                }

                // Now we deserialize the value itself
                gen.Emit(OpCodes.Ldsfld, serializerFields[memberType]);             // Load serializer from serializer container type
                gen.Emit(OpCodes.Ldarg, (short)3);                                  // Load reader argument
                gen.Emit(OpCodes.Callvirt, serializerReadMethods[memberType]);      // Call Serializer.ReadValue(IDataReader reader)

                // The stack now contains the formatted instance and the deserialized value to set the member to
                // Now we set the value
                if (fieldInfo != null)
                {
                    gen.Emit(OpCodes.Stfld, fieldInfo);                             // Set field
                }
                else if (propInfo != null)
                {
                    gen.Emit(OpCodes.Callvirt, propInfo.GetSetMethod(true));        // Call property setter
                }
                else
                {
                    throw new NotImplementedException();
                }

                gen.Emit(OpCodes.Br, endLabel);                                     // Jump to end of method

                count++;
            }

            gen.MarkLabel(defaultLabel);                                            // Default case starts here
            gen.Emit(OpCodes.Ldarg, (short)3);                                      // Load reader argument
            gen.Emit(OpCodes.Callvirt, skipMethod);                                 // Call IDataReader.SkipEntry

            gen.MarkLabel(endLabel);                                                // Method end starts here
            gen.Emit(OpCodes.Ret);                                                  // Return method

            return methodBuilder;
        }

        private static DynamicMethod GenerateDynamicWriteMethod(Type formattedType, MethodInfo writeDataEntriesAbstractMethod, Dictionary<Type, FieldBuilder> serializerFields, Dictionary<MemberInfo, List<string>> memberNames, Dictionary<Type, MethodInfo> serializerWriteMethods)
        {
            DynamicMethod methodBuilder = new DynamicMethod("Dynamic_" + writeDataEntriesAbstractMethod.Name, null, writeDataEntriesAbstractMethod.GetParameters().Select(n => n.ParameterType).ToArray(), true);
            ILGenerator gen = methodBuilder.GetILGenerator();

            writeDataEntriesAbstractMethod.GetParameters().ForEach(n => methodBuilder.DefineParameter(n.Position + 1, n.Attributes, n.Name));

            foreach (var member in memberNames.Keys)
            {
                var memberType = FormatterUtilities.GetContainedType(member);

                gen.Emit(OpCodes.Ldsfld, serializerFields[memberType]);             // Load serializer instance for type
                gen.Emit(OpCodes.Ldstr, member.Name);                               // Load member name string

                // Now we load the value of the actual member
                if (member is FieldInfo)
                {
                    var fieldInfo = member as FieldInfo;

                    if (formattedType.IsValueType)
                    {
                        gen.Emit(OpCodes.Ldarg_0);                                  // Load value argument
                        gen.Emit(OpCodes.Ldfld, fieldInfo);                         // Load value of field
                    }
                    else
                    {
                        gen.Emit(OpCodes.Ldarg_0);                                  // Load value argument reference
                        gen.Emit(OpCodes.Ldind_Ref);                                // Indirectly load value of reference
                        gen.Emit(OpCodes.Ldfld, fieldInfo);                         // Load value of field
                    }
                }
                else if (member is PropertyInfo)
                {
                    var propInfo = member as PropertyInfo;

                    if (formattedType.IsValueType)
                    {
                        gen.Emit(OpCodes.Ldarg_0);                                  // Load value argument
                        gen.Emit(OpCodes.Call, propInfo.GetGetMethod(true));        // Call property getter
                    }
                    else
                    {
                        gen.Emit(OpCodes.Ldarg_0);                                  // Load value argument reference
                        gen.Emit(OpCodes.Ldind_Ref);                                // Indirectly load value of reference
                        gen.Emit(OpCodes.Callvirt, propInfo.GetGetMethod(true));    // Call property getter
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                gen.Emit(OpCodes.Ldarg_1);                                          // Load writer argument
                gen.Emit(OpCodes.Callvirt, serializerWriteMethods[memberType]);     // Call Serializer.WriteValue(string name, T value, IDataWriter writer)
            }

            gen.Emit(OpCodes.Ret);                                                  // Return method

            return methodBuilder;
        }

#endif
    }
}