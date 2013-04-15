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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Domain.Events;

    public class SimpleFileEventStoreAdapter : IEventStoreAdapter
    {
        private readonly string _filepath;
        private readonly IEventSerializer _serializer;

        public SimpleFileEventStoreAdapter(string filepath)
        {
            _filepath = filepath;
            _serializer = new JsonNetEventSerializer();
        }

        public void Store(IEnumerable<IAggregateEvent> events)
        {
            lock (_filepath)
            {
                File.AppendAllLines(_filepath, events.Select(_serializer.Serialize));
            }
        }

        public IEnumerable<IAggregateEvent> LoadFor<TRoot>(object id)
        {
            lock (_filepath)
            {
                using (var reader = File.OpenText(_filepath))
                {
                    while (!reader.EndOfStream)
                    {
                        var serialized = reader.ReadLine();
                        var ev = _serializer.TryDeserialize(serialized, typeof(TRoot), id);
                        if (ev != null)
                            yield return ev;
                    }
                }
            }
        }
    }
}