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
    using System;
    using System.Text;

    public class SimpleGenerator : AbstractGenerator
    {
        const string DigitsSet = "0123456789";
        const string UpperAlphaSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string LowerAlphaSet = "abcdefghijklmnopqrstuvwxyz";
        const string SpecialCharSet = "#@$^*()";
        const string FullCharSet = DigitsSet + UpperAlphaSet + LowerAlphaSet + SpecialCharSet;

        internal SimpleGenerator(Func<Random> random):base(random)
        {
        }

        public string String(int minLength = 1, int maxLength = 20, string charSet = FullCharSet)
        {
            var builder = new StringBuilder();

            var setLength = charSet.Length;
            var length = Random().Next(minLength, maxLength + 1);

            while (length-- > 0)
            {
                builder.Append(charSet[Random().Next(setLength)]);
            }

            return builder.ToString();
        }

        public Guid Guid()
        {
            return new Guid(String(32, 32, DigitsSet + "ABCDEF"));
        }

        public int Int(int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return Random().Next(minValue, maxValue);
        }

        public string Email()
        {
            return String(1, 20, LowerAlphaSet) + "@" + String(1, 10, LowerAlphaSet) + ".com";
        }
    }
}