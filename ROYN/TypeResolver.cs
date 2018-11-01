using System;
using System.Linq;

namespace ROYN
{
    public class TypeResolver : IRoynTypeNameResolver
    {
        public virtual Type Resolve(string name, ResolveOptions option = ResolveOptions.FullName)
        {
            switch (option)
            {
                case ResolveOptions.Name:
                    return FindType(x => x.Name == name);

                case ResolveOptions.FullName:
                    return FindType(x => x.FullName == name);

                case ResolveOptions.AssemblyQualifiedName:
                    return FindType(x => x.AssemblyQualifiedName == name);

                default:
                    return FindType(x => x.Name == name);
            }
        }

        public virtual Type ResolveQualifiedName(string qualifiedName)
        {
            return FindType(x => x.AssemblyQualifiedName == qualifiedName);
        }

        /// <summary>
        /// Looks in all loaded assemblies for the given type.
        /// </summary>
        /// <param name="fullName">
        /// The full name of the type.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> found; null if not found.
        /// </returns>
        private static Type FindType(Func<Type, bool> predicate)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => predicate(t));
        }

        //private static Type FindType(Func<Type, bool> predicate)
        //{
        //    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //    {
        //        if (!assembly.IsDynamic)
        //        {
        //            Type[] exportedTypes = null;
        //            try
        //            {
        //                exportedTypes = assembly.GetExportedTypes();
        //            }
        //            catch (ReflectionTypeLoadException e)
        //            {
        //                exportedTypes = e.Types;
        //            }

        //            if (exportedTypes != null)
        //            {
        //                return exportedTypes.FirstOrDefault(x => predicate(x));
        //            }
        //        }
        //    }

        //    return null;
        //}

        public Type Resolve(TypeName typeName)
        {
            return Resolve(typeName.Name, typeName.ResolveOption);
        }
    }
}