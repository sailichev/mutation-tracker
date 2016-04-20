using System;
using System.Linq;
using System.Reflection;

namespace MutationTracker
{
    public static class Tracker
    {
        private static Type GetEquatableInterface(this Type @this)
        {
            return typeof(IEquatable<>).MakeGenericType(@this);
        }

        private static Func<object, object, bool> GetEquatableMethod(this Type @this)
        {
            var @interface = @this.GetEquatableInterface();

            if (@interface.IsAssignableFrom(@this))
            {
                var @method = @interface.GetMethods().Single();

                return (first, second) => first == null
                                            ? second == null
                                            : (bool)@method.Invoke(first, new[] { second });
            }

            if (@this.IsGenericType && @this.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return @this.GetGenericArguments().Single().GetEquatableMethod();
            }

            return null;
        }

        private static Func<bool> GetEquatableMethodCurried(this Type @this, object currentValue, Func<object> freshValueGetter)
        {
            var @method = @this.GetEquatableMethod();

            if (@method == null)
                return null;

            return () => method(currentValue, freshValueGetter());
        }

        private static Func<bool> GetIsUnchangedMethod(this FieldInfo @this, object target)
        {
            return @this.FieldType.GetEquatableMethodCurried(@this.GetValue(target), () => @this.GetValue(target));
        }

        public static Func<bool> TrackFields(this object @this)
        {
            return @this
                .GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) // private [backing] fields of the base class are not obtained this way
                .Where(field => !field.IsInitOnly)
                .Select(field => field.GetIsUnchangedMethod(@this))
                .Where(method => method != null)
                .ToArray()
                .Where(unchanged => !unchanged())
                .Any;
        }
    }
}