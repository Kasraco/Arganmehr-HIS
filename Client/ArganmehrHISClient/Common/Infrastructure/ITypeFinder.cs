﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies(bool ignoreInactivePlugins = false);
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
    }

    public static class ITypeFinderExtensions
    {
        public static IEnumerable<Type> FindClassesOfType<T>(this ITypeFinder finder, bool onlyConcreteClasses = true, bool ignoreInactivePlugins = false)
        {
            return finder.FindClassesOfType(typeof(T), finder.GetAssemblies(ignoreInactivePlugins), onlyConcreteClasses);
        }

        public static IEnumerable<Type> FindClassesOfType(this ITypeFinder finder, Type assignTypeFrom, bool onlyConcreteClasses = true, bool ignoreInactivePlugins = false)
        {
            return finder.FindClassesOfType(assignTypeFrom, finder.GetAssemblies(ignoreInactivePlugins), onlyConcreteClasses);
        }

        public static IEnumerable<Type> FindClassesOfType<T>(this ITypeFinder finder, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return finder.FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }

    }
}
