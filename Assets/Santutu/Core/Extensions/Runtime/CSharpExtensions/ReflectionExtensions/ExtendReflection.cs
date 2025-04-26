using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Serialization;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions.ReflectionExtensions
{
    public static class ExtendReflection
    {
        public static T GetAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            var attribute = type.GetCustomAttribute<T>(inherit);

            if (attribute == null)
            {
                throw new NullReferenceException($"{type.Name} {typeof(T)}");
            }

            return attribute;
        }


        public static IEnumerable<T> GetAttributes<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return (IEnumerable<T>)type.GetCustomAttributes(typeof(T), inherit);
        }

        public static bool HasAttribute<T>(this Type type, bool inherit = true)
        {
            return type.GetCustomAttributes(typeof(T), inherit).Any();
        }

        public static object GetPropertyValue(this object obj, string propName)
        {
            var value = obj.GetType()
                           .GetProperty(propName)
                 ?
                .GetValue(obj);

            if (value == null)
            {
                throw new Exception($" Not Found {propName} in {obj.GetType()}");
            }

            return value;
        }


        public static bool TryGetAttribute(
            this FieldInfo fieldInfo,
            Type attrType,
            out object attr,
            bool inherit = true
        )
        {
            attr = fieldInfo.GetCustomAttribute(attrType, inherit);

            return attr != null;
        }

        public static bool TryGetAttribute<T>(
            this FieldInfo fieldInfo,
            out T attr,
            bool inherit = true
        ) where T : Attribute
        {
            attr = fieldInfo.GetCustomAttribute<T>(inherit);

            return attr != null;
        }

        public static string GetFormerlySerializedFieldName(this FieldInfo fieldInfo)
        {
            if (fieldInfo.TryGetAttribute<FormerlySerializedAsAttribute>(out var formerlySerializedAsAttr))
            {
                return formerlySerializedAsAttr.oldName;
            }

            return fieldInfo.Name;
        }


        public static bool TryGetAttribute<T>(this Type type, out T attribute, bool inherit = true) where T : Attribute
        {
            attribute = type.GetCustomAttribute<T>(inherit);
            return attribute != null;
        }


        public static T Attribute<T>(this Enum value, bool inherit = true) where T : Attribute
        {
            return value.GetAttribute<T>(inherit);
        }

        public static T GetAttribute<T>(this Enum value, bool inherit = true) where T : Attribute
        {
            return (T)value.GetType().GetField(value.ToString()).GetCustomAttribute(typeof(T), inherit);
        }


        public static List<T> Attributes<T>(this Enum value, bool inherit = true) where T : Attribute
        {
            return new List<T>(
                (IEnumerable<T>)value
                               .GetType()
                               .GetField(value.ToString())
                               .GetCustomAttributes(typeof(T), inherit)
            );
        }

        public static PropertyInfo[] GetProperties(this object obj)
        {
            return obj.GetType().GetProperties();
        }


        public static PropertyInfo GetProperty(this object obj, string propertyName)
        {
            return obj.GetType()
                      .GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static FieldInfo GetField(this object obj, string fieldName)
        {
            return obj.GetType()
                      .GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static PropertyInfo GetProperty(this object obj, Predicate<PropertyInfo> predicate)
        {
            var info = obj.GetType()
                          .GetProperties()
                          .FirstOrDefault(prop => predicate(prop));

            if (info == null)
            {
                throw new Exception($"Not Found collect condition prop in ${obj.GetType()}");
            }

            return info;
        }

        public static object GetPropertyValue(this object obj, Predicate<PropertyInfo> predicate)
        {
            return obj.GetProperty(predicate).GetValue(obj);
        }

        public static void SetPropertyValue(this object obj, Predicate<PropertyInfo> predicate, object value)
        {
            obj.GetProperty(predicate).SetValue(obj, value);
        }

        public static void SetPropertyValue(this object obj, string propName, object value)
        {
            var info = obj.GetType()
                          .GetProperty(propName);

            if (info == null)
            {
                throw new Exception($"Not Found ${propName} in ${obj.GetType()}");
            }

            info.SetValue(obj, value);
        }


        public static bool HasProperty(this object obj, string propName)
        {
            return obj.GetType().GetProperty(propName) != null;
        }

        public static bool HasProperty(this Type type, string propName)
        {
            return type.GetProperty(propName) != null;
        }

        public static bool IsDictionaryType(this PropertyInfo propInfo)
        {
            Type type = propInfo.PropertyType;

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public static T GetAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            var attr = propertyInfo.GetCustomAttribute<T>();
            if (attr == null)
            {
                throw new NullReferenceException($"{propertyInfo.Name} {typeof(T)}");
            }

            return attr;
        }


        public static bool IsGenericArgument(this PropertyInfo propInfo, int index, Type type)
        {
            return propInfo.PropertyType.GetGenericArguments()[index] == type;
        }

        public static object InvokeMethod(this object obj, string methodName, params object[] parameters)
        {
            return obj.GetType().GetMethod(methodName).Invoke(obj, parameters);
        }


        public static bool IsGenericType(this FieldInfo fieldInfo, Type genericClassType)
        {
            return fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == genericClassType;
        }

        public static IEnumerable<PropFieldInfo> GetPropFields(this object obj)
        {
            return obj.GetType().GetPropFields();
        }


        public static IEnumerable<PropFieldInfo> GetPropFields(this Type type)
        {
            var fields = type.GetFields().Cast<MemberInfo>().Select(m => new PropFieldInfo(m));
            return type.GetProperties().Cast<MemberInfo>().Select(m => new PropFieldInfo(m)).Concat(fields);
        }


        public static IEnumerable<PropFieldInfo> GetPropFields(this Type type, BindingFlags bindingFlags)
        {
            var fields = type.GetFields(bindingFlags).Cast<MemberInfo>().Select(m => new PropFieldInfo(m));
            return type.GetProperties(bindingFlags).Cast<MemberInfo>().Select(m => new PropFieldInfo(m)).Concat(fields);
        }

        public static PropFieldInfo GetPropField(this object obj, string name)
        {
            MemberInfo memberInfo = (obj.GetProperty(name) as MemberInfo ?? obj.GetField(name) as MemberInfo);
            return new PropFieldInfo(memberInfo);
        }

        public static object GetPropFieldValue(this object obj, string name)
        {
            return obj.GetPropField(name).GetValue(obj);
        }

        public static FieldInfo GetFieldInHierarchy(this Type type, string fieldName)
        {
            while (type != null)
            {
                var fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fieldInfo != null)
                {
                    return fieldInfo;
                }

                type = type.BaseType;
            }

            throw new Exception($"{fieldName}");
        }

        public static IEnumerable<FieldInfo> GetFieldInfosSelfAbove(
            this Type type,
            BindingFlags bindingAttr,
            Func<Type, bool> canTravel = null
        )
        {
            while (type != null)
            {
                if (!canTravel?.Invoke(type) ?? false)
                {
                    yield break;
                }

                foreach (var fieldInfo in type.GetFields(bindingAttr))
                {
                    yield return fieldInfo;
                }

                type = type.BaseType;
            }
        }

        public static bool HasAttribute<T>(this FieldInfo fieldInfo) where T : Attribute
        {
            return fieldInfo.GetCustomAttribute<T>() != null;
        }

        public static TField[] GetStaticFieldValues<T, TField>() where T : struct
        {
            return typeof(T)
                  .GetFields(BindingFlags.Public | BindingFlags.Static)
                  .Where(f => f.FieldType == typeof(TField))
                  .Select(f => f.GetValue(null))
                  .OfType<TField>()
                  .ToArray();
        }
    }
}