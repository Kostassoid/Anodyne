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

namespace Kostassoid.Anodyne.Common.CodeContracts
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Internal state consistency checks that throw an internal error exception when they fail.
    /// </summary>
    public static class Assumes
    {
        /// <summary>
        /// Validates some expression describing the acceptable condition evaluates to true.
        /// </summary>
        /// <param name="condition">The expression that must evaluate to true to avoid an internal error exception.</param>
        /// <param name="message">The message to include with the exception.</param>
        [Pure, DebuggerStepThrough]
        public static void True(bool condition, string message = null)
        {
            if (!condition)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Validates some expression describing the acceptable condition evaluates to true.
        /// </summary>
        /// <param name="condition">The expression that must evaluate to true to avoid an internal error exception.</param>
        /// <param name="unformattedMessage">The unformatted message.</param>
        /// <param name="args">Formatting arguments.</param>
        [Pure, DebuggerStepThrough]
        public static void True(bool condition, string unformattedMessage, params object[] args)
        {
            if (!condition)
            {
                Fail(string.Format(CultureInfo.CurrentCulture, unformattedMessage, args));
            }
        }

        /// <summary>
        /// Throws an internal error exception.
        /// </summary>
        /// <param name="message">The message.</param>
        [Pure, DebuggerStepThrough]
        public static void Fail(string message = null)
        {
            if (message != null)
            {
                throw new InternalErrorException(message);
            }
            else
            {
                throw new InternalErrorException();
            }
        }

        /// <summary>
        /// An internal error exception that should never be caught.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "This exception should never be caught.")]
        [Serializable]
        private class InternalErrorException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            internal InternalErrorException()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            internal InternalErrorException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
            /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
            /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
            /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
            protected InternalErrorException(
                SerializationInfo info,
                StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}