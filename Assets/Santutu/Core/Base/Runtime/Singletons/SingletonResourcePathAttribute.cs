using System;
using System.IO;

namespace Santutu.Core.Base.Runtime.Singletons
{
    public class SingletonResourcePathAttribute : Attribute
    {
        private readonly string _loadPath;
        private readonly string _resourcesFolderPath;

        public string LoadPath => _loadPath;

        public string SavePath => Path.Join(_resourcesFolderPath, _loadPath + ".asset");

        public SingletonResourcePathAttribute(string resourcesFolderPath, string loadPath)
        {
            _resourcesFolderPath = resourcesFolderPath;
            _loadPath = loadPath;
        }

        public static implicit operator string(SingletonResourcePathAttribute attr)
        {
            return attr._loadPath;
        }


        public override string ToString()
        {
            return _loadPath;
        }
    }
}