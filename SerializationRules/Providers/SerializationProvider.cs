﻿using System.IO;
using SerializationRules.Entities;

namespace SerializationRules.Providers
{
    public class SerializationProvider : ISerializationProvider
    {
        private readonly IPathProvider _pathProvider;
        private readonly IDefinitionsProvider _definitionsProvider;

        public SerializationProvider(IPathProvider pathProvider, IDefinitionsProvider definitionsProvider)
        {
            _pathProvider = pathProvider;
            _definitionsProvider = definitionsProvider;
        }

        public SerializationProvider():this(new PathProvider(), new DefinitionsProvider())
        {
            
        }

        public virtual void Serialize(ISerializableItem item)
        {
            // foreach serialization definition
            var serializationDefinitions = _definitionsProvider.
                GetSerializationDefinitions(item.Database);

            foreach (var serializationDefinition in serializationDefinitions)
            {
                //see if item needs to be serialized
                if (!serializationDefinition.Evaluate(item)) continue;
                // get the right folder

                var path = _pathProvider.GetPath(item, serializationDefinition.Path);
                //serialize it

                item.Dump(path);
            }
        }

        public virtual void Remove(ISerializableItem item, ISerializableItem oldParentSerializable)
        {

            foreach (var serializableDefinition in _definitionsProvider.GetSerializationDefinitions(item.Database))
            {
                var file = new FileInfo(_pathProvider.GetOldPath(item, oldParentSerializable, serializableDefinition.Path));
                if (file.Exists) file.Delete();

                var folder =
                    new DirectoryInfo(_pathProvider.GetOldFolderPath(item, oldParentSerializable,
                                                                     serializableDefinition.Path));
                if (folder.Exists) folder.Delete();

            }
        }


        public virtual bool IsSerialized(ISerializableItem item, ISerializationDefinition definition)
        {
            return new FileInfo(_pathProvider.GetPath(item, definition.Path)).Exists;
        }




    }
}