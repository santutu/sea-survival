using System;
using System.Linq;
using System.Reflection;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions.ReflectionExtensions
{
    public class PropFieldInfo
    {
        public MemberInfo MemberInfo { get; }
        public string Name => MemberInfo.Name;
        public PropertyInfo PropInfo => (PropertyInfo)MemberInfo;
        public FieldInfo FieldInfo => (FieldInfo)MemberInfo;

        public PropFieldInfo(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }


        public bool TryGetAttribute<T>(out T attribute) where T : Attribute
        {
            attribute = MemberInfo.GetCustomAttribute<T>();
            return attribute != null;
        }

        public T GetAttribute<T>() where T : Attribute
        {
            return MemberInfo.GetCustomAttribute<T>();
        }

        public bool HasAttribute<T>()
        {
            return MemberInfo.GetCustomAttributes(typeof(T), false).Any();
        }

        public bool IsField => MemberInfo.MemberType == MemberTypes.Field;

        public bool IsProp => MemberInfo.MemberType == MemberTypes.Property;


        public T GetValue<T>(object subject)
        {
            if (IsField)
            {
                return (T)FieldInfo.GetValue(subject);
            }

            if (IsProp)
            {
                return (T)PropInfo.GetValue(subject);
            }

            throw new Exception("unreachable code.");
        }


        public object GetValue(object subject)
        {
            if (IsField)
            {
                return FieldInfo.GetValue(subject);
            }

            if (IsProp)
            {
                return PropInfo.GetValue(subject);
            }

            throw new Exception("unreachable code.");
        }


        public Type PropFieldType => IsProp ? PropInfo.PropertyType : FieldInfo.FieldType;

        public void SetValue(object obj, object value)
        {
            if (IsField)
            {
                FieldInfo.SetValue(obj, value);
                return;
            }

            if (IsProp)
            {
                PropInfo.SetValue(obj, value);
                return;
            }

            throw new Exception("unreachable code.");
        }
    }
}