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

namespace Kostassoid.Anodyne.Common.Extentions
{
    using System;

    public static class DateTimeEx
    {
        private const int PrecisionInMilliseconds = 1000;

        public static bool SoftEquals(this DateTime dateTime, DateTime compareTo, int precision = PrecisionInMilliseconds)
        {
            return Math.Abs((dateTime - compareTo).TotalMilliseconds) < precision;
        }

        public static DateTime StripMilliseconds(this DateTime dateTime)
        {
            return dateTime.AddMilliseconds(-dateTime.Millisecond).AddTicks(-dateTime.Ticks);
        }
    }
}