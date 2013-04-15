namespace Kostassoid.Anodyne.EventStore.Adapters.SimpleFile
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class SimpleTypeNameSerializationBinder : SerializationBinder
    {
        public SimpleTypeNameSerializationBinder(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Map(type, type.Name);
            }
        }

        readonly Dictionary<Type, string> _typeToName = new Dictionary<Type, string>();
        readonly Dictionary<string, Type> _nameToType = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public void Map(Type type, string name)
        {
            _typeToName.Add(type, name);
            _nameToType.Add(name, type);
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            string name;
            if (_typeToName.TryGetValue(serializedType, out name))
            {
                assemblyName = null;
                typeName = name;
            }
            else
            {
                assemblyName = serializedType.Assembly.FullName;
                typeName = serializedType.FullName;
            }
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (assemblyName == null)
            {
                Type type;
                if (_nameToType.TryGetValue(typeName, out type))
                {
                    return type;
                }
            }
            return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName), true);
        }
    }
}