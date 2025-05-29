using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IocConfig.Extensions
{
    public static class MiscExtensions
    {

        private static readonly Type[] s_predefinedTypes = new Type[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(TimeSpan), typeof(Guid) };
        private static readonly Type[] s_predefinedGenericTypes = new Type[] { typeof(Nullable<>) };


        public static bool IsEntityFrameworkProvider(this IQueryProvider provider)
        {
            return provider.GetType().FullName == "System.Data.Objects.ELinq.ObjectQueryProvider";
        }

        public static bool IsPredefinedSimpleType(this Type type)
        {
            if ((type.IsPrimitive && (type != typeof(IntPtr))) && (type != typeof(UIntPtr)))
            {
                return true;
            }
            if (type.IsEnum)
            {
                return true;
            }

            return s_predefinedTypes.Any(t => t == type);
        }

    }
}
