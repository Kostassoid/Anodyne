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

namespace Kostassoid.Anodyne.EventStore
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Common.Extentions;
    using Domain.Events;
    using Newtonsoft.Json;

    public class SimpleFileEventStoreAdapter : IEventStoreAdapter
    {
        private readonly string _filepath;
        private readonly JsonSerializer _serializer;

        public SimpleFileEventStoreAdapter(string filepath)
        {
            _filepath = filepath;
            _serializer = new JsonSerializer();
        }

        public void Store(IEnumerable<IAggregateEvent> events)
        {
            lock (_filepath)
            {
                using (var writer = File.AppendText(_filepath))
                {
                    events.ForEach(e => Serialize(writer, e));
                    writer.Flush();
                    //File.AppendAllLines(writer, events.Select(Serialize));
                }
            }
        }

        private void Serialize(TextWriter writer, IAggregateEvent ev)
        {
            _serializer.Serialize(writer, new EventEnvelope(ev));
        }

        public IEnumerable<IAggregateEvent> LoadFor<TRoot>(object id)
        {
            throw new System.NotImplementedException();
        }
    }
}