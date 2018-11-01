using System;

namespace ROYN
{
    public interface IRoynTypeNameResolver
    {
        Type Resolve(string name, ResolveOptions option = ResolveOptions.FullName);
        Type Resolve(TypeName typeName);
    }
}