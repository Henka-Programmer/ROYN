using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ROYN
{
    public class DynamicTypeBuilder
    {
        private ModuleBuilder dynamicModule;
        private TypeBuilder typeBuilder;
        private AssemblyBuilder dynamicAssembly;

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

            TypeName = $"ROYN_Dynamic{typeName}";
        }

        private Type DynamicType;
        private readonly object locker = new object();

        public Type CreateType()
        {
            if (DynamicType != null)
            {
                return DynamicType;
            }

            dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ROYN.DYNAMIC.TYPES"), AssemblyBuilderAccess.Run);
            dynamicModule = dynamicAssembly.DefineDynamicModule("ROYN.DYNAMIC.ASSEMBLY.MODULE");
            typeBuilder = dynamicModule.DefineType(TypeName, TypeAttributes.Public);

            lock (locker)
            {
                foreach (var propertyDef in propertiesDefinitions)
                {
                    if (propertyDef.Value is Type)
                    {
                        AddProperty(typeBuilder, propertyDef.Key, (Type)propertyDef.Value);
                    }
                    else if (propertyDef.Value is DynamicTypeBuilder tb)
                    {
                        AddProperty(typeBuilder, propertyDef.Key, tb.CreateType());
                    }
                }
            }

            return DynamicType = typeBuilder.CreateType();
        }

        private Dictionary<string, dynamic> propertiesDefinitions = new Dictionary<string, dynamic>();

        public string TypeName { get; }

        public DynamicTypeBuilder DefineProperty(PropertyInfo propertyInfo)
        {
            return DefineProperty(propertyInfo.Name, propertyInfo.PropertyType);
        }

        public DynamicTypeBuilder DefineProperty(string propertyName, Type propertyType)
        {
            if (DynamicType != null)
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
            propertiesDefinitions.Add(propertyName, propertyType);
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