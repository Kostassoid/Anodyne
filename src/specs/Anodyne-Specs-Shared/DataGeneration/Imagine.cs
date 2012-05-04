// Copyright 2011-2012 Anodyne.
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

namespace Kostassoid.Anodyne.Specs.Shared.DataGeneration
{
    using Common;
    using System;

    public static class Imagine
    {
        internal static Random Random { get; private set; }

        private static int _seed;
        public static int RandomSeed { set
        {
            _seed = value;
            Reset();
        }}

        public static SimpleGenerator Any { get; private set; }

        static Imagine()
        {
            RandomSeed = (int)SystemTime.Now.Ticks;

            Any = new SimpleGenerator(() => Random);
        }

        public static void Reset()
        {
            Random = new Random(_seed);
        }

    }
}