using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace Kurisu.RealAgents
{
    public class DescAttribute : PropertyAttribute
    {
        public string Description { get; }
        public DescAttribute(string description)
        {
            Description = description;
        }
    }
    public class DescUtil
    {
        public static Dictionary<string, string> GetDesc(Type statesType)
        {
            FieldInfo[] fields = statesType.GetFields(BindingFlags.Public | BindingFlags.Static);
            return fields
                .Where(f => f.FieldType == typeof(string))
                .Where(f => f.GetCustomAttribute<DescAttribute>() != null)
                .ToDictionary(
                    f => f.GetValue(null).ToString(),
                    f => f.GetCustomAttribute<DescAttribute>().Description
                );
        }
    }
}
