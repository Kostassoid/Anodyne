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
    using Common.Reflection;
    using Domain.Events;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class JsonNetEventSerializer : IEventSerializer
    {
        private readonly JsonSerializer _serializer;

        private readonly IList<Type> _eventTypes;

        public JsonNetEventSerializer()
        {
            _serializer = new JsonSerializer
                              {
                                  ContractResolver = new EventContractResolver(),
                                  TypeNameHandling = TypeNameHandling.Objects,
                                  TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                                  ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                  DefaultValueHandling = DefaultValueHandling.Populate,
                              };

            _serializer.Converters.Add(new EventCreationConverter());

            _eventTypes = AllTypes.BasedOn<IAggregateEvent>(From.AllAssemblies()).ToList();
        }

        public string Serialize(IUncommitedEvent ev)
        {
            var stringWriter = new StringWriter();

            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(ev.Id);
                writer.WritePropertyName("EventType");
                writer.WriteValue(ev.GetType().Name);
                writer.WritePropertyName("TargetType");
                writer.WriteValue(ev.Target.GetType().Name);
                writer.WritePropertyName("TargetId");
                writer.WriteValue(ev.Target.IdObject);
                writer.WritePropertyName("TargetVersion");
                writer.WriteValue(ev.TargetVersion);
                writer.WritePropertyName("Raw");
                _serializer.Serialize(writer, ev);
                writer.WriteEndObject();
            }

            return stringWriter.ToString();
        }

        public IAggregateEvent TryDeserialize(string serializedEvent, Type targetType, object targetId)
        {
            var envelope = JObject.Parse(serializedEvent);

            if (envelope["TargetType"].Value<string>() != targetType.Name ||
                envelope["TargetId"].Value<string>() != targetId.ToString())
                return null;

            var eventType = _eventTypes.First(t => t.Name == envelope["EventType"].Value<string>());

            var ev = envelope["Raw"].ToObject(eventType, _serializer);

            return (IAggregateEvent) ev;
        }
    }
}