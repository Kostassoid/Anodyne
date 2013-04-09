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

namespace Kostassoid.Anodyne.Common.Measure
{
    public class DigitalStorageSize
    {
        public long Bytes { get; private set; }
        public long Kilobytes { get { return Bytes / 1024; } }
        public long Megabytes { get { return Bytes / (1024 * 1024); } }

        private DigitalStorageSize(long bytes)
        {
            Bytes = bytes;
        }

        public static DigitalStorageSize FromBytes(long bytes)
        {
            return new DigitalStorageSize(bytes);
        }

        public static DigitalStorageSize FromKilobytes(long kilobytes)
        {
            return new DigitalStorageSize(kilobytes * 1024);
        }

        public static DigitalStorageSize FromMegabytes(long megabytes)
        {
            return new DigitalStorageSize(megabytes * 1024 * 1024);
        }

        protected bool Equals(DigitalStorageSize other)
        {
            return Bytes == other.Bytes;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DigitalStorageSize) obj);
        }

        public override int GetHashCode()
        {
            return Bytes.GetHashCode();
        }
    }
}