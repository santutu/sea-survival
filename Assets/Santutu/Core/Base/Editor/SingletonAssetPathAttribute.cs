using NUnit.Framework;

namespace Santutu.Core.Base.Editor
{
    public class SingletonAssetPathAttribute : PropertyAttribute
    {
        public string Value;

        public SingletonAssetPathAttribute(string path)
        {
            Value = path;
        }

        public static implicit operator string(SingletonAssetPathAttribute attr)
        {
            return attr.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}