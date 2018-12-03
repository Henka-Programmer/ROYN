using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ROYN
{
    public class DynamicTypeBuilder
    {
        private static ModuleBuilder dynamicModule;
        private TypeBuilder typeBuilder;
        private static AssemblyBuilder dynamicAssembly;

        public DynamicTypeBuilder(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("type Name must be not empty", nameof(typeName));
            }

            if (typeName.Contains(" "))
            {
                throw new InvalidOperationException("type name must be valid identifier");
            }

            TypeName = $"ROYN_Dynamic{typeName}{Guid.NewGuid().ToString("N")}";
            dynamicAssembly = dynamicAssembly ?? AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName($"ROYN.DYNAMIC.TYPES"), AssemblyBuilderAccess.Run);
            dynamicModule = dynamicModule ?? dynamicAssembly.DefineDynamicModule("ROYN.DYNAMIC.ASSEMBLY.MODULE");
            typeBuilder = dynamicModule.DefineType(TypeName, TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
        }

        private Type definedType;
        private readonly object locker = new object();

        public Type CreateType()
        {
            if (definedType != null)
            {
                return definedType;
            }

          

            lock (locker)
            {
                foreach (var propertyDef in propertiesDefinitions)
                {
                    if (propertyDef.Value is Type type)
                    {
                        AddProperty(typeBuilder, propertyDef.Key, type);
                    }
                    else if (propertyDef.Value is TypeBuilder tb)
                    {
                        AddProperty(typeBuilder, propertyDef.Key, tb);
                    }
                }
            }

            return definedType = typeBuilder.CreateType();
        }

        private Dictionary<string, Type> propertiesDefinitions = new Dictionary<string, Type>();

        public string TypeName { get; }

        public DynamicTypeBuilder DefineProperty(PropertyInfo propertyInfo)
        {
           
                return DefineProperty(propertyInfo.Name, propertyInfo.PropertyType);
        }

        public DynamicTypeBuilder DefineProperty(string propertyName, Type propertyType)
        {
            if (definedType != null)
            {
                throw new InvalidOperationException("Couldn't Define Properties for a Generated Type, DefineProperty Method shouldn't be called after CreateType Method");
            }

            if (propertiesDefinitions.ContainsKey(propertyName))
            {
                throw new InvalidOperationException($"'{propertyName}' Property already defined!");
            }
            propertiesDefinitions.Add(propertyName, propertyType);
            return this;
        }

        public DynamicTypeBuilder DefineProperty(string propertyName, DynamicTypeBuilder propertyType)
        {
            propertiesDefinitions.Add(propertyName, propertyType.typeBuilder);
            return this;
        }

        protected static void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig;

            FieldBuilder field = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            PropertyBuilder property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType,
                new[] { propertyType });

            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_value", getSetAttr, propertyType,
                Type.EmptyTypes);
            ILGenerator getIl = getMethodBuilder.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, field);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_value", getSetAttr, null,
                new[] { propertyType });
            ILGenerator setIl = setMethodBuilder.GetILGenerator();
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, field);
            setIl.Emit(OpCodes.Ret);

            property.SetGetMethod(getMethodBuilder);
            property.SetSetMethod(setMethodBuilder);
        }
    }
}