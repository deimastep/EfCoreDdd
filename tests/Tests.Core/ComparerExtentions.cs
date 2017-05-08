namespace PurchaseApproval.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using KellermanSoftware.CompareNetObjects;
    using Xunit.Abstractions;

    public static class ComparerExtentions
    {
        public static bool EqualByProperties(this object o1, object o2, ITestOutputHelper console = null)
        {
            return EqualByPropertiesExcept(o1, o2, console);
        }

        public static bool EqualByPropertiesExcept(this object o1, object o2, params string[] ignoreList)
        {
            return EqualByPropertiesExcept(o1, o2, null, ignoreList);
        }

        public static bool EqualByPropertiesExcept(this object o1, object o2, ITestOutputHelper console, params string[] ignoreList)
        {
            var comparer = new ComparisonConfig
            {
                CompareChildren = true,
                IgnoreObjectTypes = true,
                MaxDifferences = int.MaxValue,
                MembersToIgnore = ignoreList.ToList(),
            };
            return Compare(o1, o2, comparer, console);
        }

        public static bool EqualByPropertiesOnlyFor(this object o1, object o2, params string[] includeList)
        {
            return EqualByPropertiesOnlyFor(o1, o2, null, includeList);
        }

        public static bool EqualByPropertiesOnlyFor(this object o1, object o2, ITestOutputHelper console, params string[] includeList)
        {
            var comparer = new ComparisonConfig
            {
                CompareChildren = true,
                IgnoreObjectTypes = true,
                MaxDifferences = int.MaxValue,
                MembersToInclude = includeList.ToList()
            };
            return Compare(o1, o2, comparer, console);
        }

        private static bool Compare(object o1, object o2, ComparisonConfig comparerConfig, ITestOutputHelper console)
        {
            var result = new CompareLogic(comparerConfig).Compare(o1, o2);
            if (console != null && !result.AreEqual)
            {
                console.WriteLine(result.DifferencesString);
            }
#if DEBUG
            if (!result.AreEqual)
            {
                System.Diagnostics.Debug.WriteLine(result.DifferencesString);
            }
#endif
            return result.AreEqual;
        }

        /// <summary>
        /// Get all public properties in hierarchy
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if (!type.IsInterface)
            {
                return type.GetProperties();
            }

            return new[] { type }
                   .Concat(type.GetInterfaces())
                   .SelectMany(i => i.GetProperties());
        }

        /// <summary>
        /// Get all public properties for current type but not whole hierarchy
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPublicPropertiesOnlyForType(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        }
    }
}
