using System;

namespace PomeloCli;

static class TypeExtensions
{
    // https://itecnote.com/tecnote/c-check-if-a-class-is-derived-from-a-generic-class/
    public static bool IsSubclassOfRawGeneric(this Type? current, Type generic)
    {
        while (current != null && current != typeof(object))
        {
            var cur = current.IsGenericType ? current.GetGenericTypeDefinition() : current;
            if (generic == cur)
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }
}