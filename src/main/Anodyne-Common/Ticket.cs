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

namespace Kostassoid.Anodyne.Common
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Issues a unique url-friendly string (a ticket) with embedded expiration date
    /// </summary>
    public class Ticket
    {
        private const string DateTimeEncodingFormat = "yyyyMMddHHmm";
        private const int DateTimeEncodedLength = 12; // should be enough to hold a datetime as number (in hex)

        /// <summary>
        /// Generates a ticket using expiration date and unique key
        /// </summary>
        /// <param name="expiration">Expiration date</param>
        /// <param name="key">Unique key</param>
        /// <returns>A new ticket</returns>
        public static string GenerateUsing(DateTime expiration, Guid key)
        {
            var clearTicket = key.ToString("N") + Convert.ToInt64(expiration.ToString(DateTimeEncodingFormat)).ToString("x" + DateTimeEncodedLength);

            var encodedTicket = ByteArrayToSafeUrlString(HexStringToByteArray(clearTicket));

            return encodedTicket;
        }

        /// <summary>
        /// Generates ticket with defined lifetime
        /// </summary>
        /// <param name="lifeTime">Desired lifetime</param>
        /// <returns>A new ticket</returns>
        public static string Generate(TimeSpan lifeTime)
        {
            return GenerateUsing(SystemTime.Now + lifeTime, Guid.NewGuid());
        }

        /// <summary>
        /// Checks if the ticket has expired
        /// </summary>
        /// <param name="ticket">A ticket to validate</param>
        /// <param name="now">Datetime to check against (usually Now)</param>
        /// <returns>True if expired</returns>
        public static bool HasExpired(string ticket, DateTime now)
        {
            var decodedBytes = UrlStringToByteArray(ticket);
            var decodedTicket = ByteArrayToHexString(decodedBytes);

            var datetimePart = Convert.ToInt64(decodedTicket.Substring(decodedTicket.Length - DateTimeEncodedLength, DateTimeEncodedLength), 16).ToString();

            try
            {
                var dateTime = DateTime.ParseExact(datetimePart, DateTimeEncodingFormat, CultureInfo.InvariantCulture);
                return now > dateTime;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Checks if the ticket has expired according to SystemTime.Now
        /// </summary>
        /// <param name="ticket">A ticket to validate</param>
        /// <returns>True if expired</returns>
        public static bool HasExpired(string ticket)
        {
            return HasExpired(ticket, SystemTime.Now);
        }

        private static byte[] HexStringToByteArray(String hexString)
        {
            var numberChars = hexString.Length;
            var bytes = new byte[numberChars/2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i/2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return bytes;
        }

        private static string ByteArrayToHexString(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length*2);
            foreach (var b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private static string ByteArrayToSafeUrlString(byte[] bytes)
        {
            var str = Convert.ToBase64String(bytes.ToArray(), Base64FormattingOptions.None);
            return str.Replace("=", String.Empty).Replace('+', '-').Replace('/', '_');
        }

        private static byte[] UrlStringToByteArray(string input)
        {
            var str = input.Replace('-', '+').Replace('_', '/');
            str = str.PadRight(str.Length + (4 - str.Length%4)%4, '=');
            return Convert.FromBase64String(str);
        }
    }
}