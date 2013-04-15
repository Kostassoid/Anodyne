// Copyright 2011-2013 Anodyne.
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//  
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace Kostassoid.Anodyne.EventStore.Adapters.SimpleFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters;
    using Common.Extentions;
    using Common.Reflection;
    using Domain.Base;
    using Domain.Events;
    using Newtonsoft.Json;

    public class JsonNetEventSerializer : IEventSerializer
    {
        private readonly JsonSerializer _serializer;

        private readonly IList<Type> _eventTypes;
        private readonly IList<Type> _valueObjectTypes;

        public JsonNetEventSerializer()
        {
            _eventTypes = AllTypes.BasedOn<IAggregateEvent>(From.AllAssemblies()).ToList();
            _valueObjectTypes = AllTypes.BasedOn<IValueObject>(From.AllAssemblies()).ToList();

            _serializer = new JsonSerializer
                              {
                                  ContractResolver = new EventContractResolver(),
                                  TypeNameHandling = TypeNameHandling.All,
                                  TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                                  ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                  DefaultValueHandling = DefaultValueHandling.Populate,
                                  Binder = new SimpleTypeNameSerializationBinder(
                                      _eventTypes
                                      .Union(_valueObjectTypes)
                                      .Union(typeof(StoredEvent).AsEnumerable()))
                              };

            _serializer.Converters.Add(new EventCreationConverter());
        }

        public string Serialize(IUncommitedEvent ev)
        {
            var stringWriter = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter))
            {
                _serializer.Serialize(writer, new StoredEvent(ev));
            }

            return stringWriter.ToString();
        }

        public IAggregateEvent TryDeserialize(string serializedEvent, Type targetType, object targetId)
        {
            var stringReader = new StringReader(serializedEvent);
            using (var reader = new JsonTextReader(stringReader))
            {
                var storedEvent = _serializer.Deserialize<StoredEvent>(reader);

                if (storedEvent.TargetType != targetType.Name || storedEvent.TargetId.ToString() != targetId.ToString())
                    return null;

                return (IAggregateEvent)storedEvent.Raw;
            }
        }
    }
}