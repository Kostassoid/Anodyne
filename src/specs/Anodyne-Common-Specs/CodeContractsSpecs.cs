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

namespace Kostassoid.Anodyne.Common.Specs
{
    using System;

    using CodeContracts;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class CodeContractsSpecs
    {

        [TestFixture]
        [Category("Unit")]
        public class when_checking_null_requirement_on_null_value
        {
            [Test]
            public void should_throw_argument_null_exception()
            {
                string value = null;

                // ReSharper disable ExpressionIsAlwaysNull
                Action action = () => Requires.NotNull(value, "value");
                action.ShouldThrow<ArgumentNullException>();
                // ReSharper restore ExpressionIsAlwaysNull
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_null_requirement_on_non_null_value
        {
            [Test]
            public void should_not_throw()
            {
                var value = String.Empty;

                Action action = () => Requires.NotNull(value, "value");
                action.ShouldNotThrow<ArgumentNullException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_null_or_empty_requirement_on_null_value
        {
            [Test]
            public void should_throw_argument_null_exception()
            {
                string value = null;

                // ReSharper disable ExpressionIsAlwaysNull
                Action action = () => Requires.NotNullOrEmpty(value, "value");
                action.ShouldThrow<ArgumentNullException>();
                // ReSharper restore ExpressionIsAlwaysNull
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_null_or_empty_requirement_on_empty_value
        {
            [Test]
            public void should_throw_argument_exception()
            {
                var value = String.Empty;
                Action action = () => Requires.NotNullOrEmpty(value, "value");
                action.ShouldThrow<ArgumentException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_null_or_empty_requirement_on_non_null_value
        {
            [Test]
            public void should_not_throw()
            {
                const string value = "zzz";
                Action action = () => Requires.NotNullOrEmpty(value, "value");
                action.ShouldNotThrow<ArgumentException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_true_assumption_on_false_value
        {
            [Test]
            public void should_throw_internal_exception()
            {
                Action action = () => Assumes.True(false, "oops");
                action.ShouldThrow<Assumes.InternalErrorException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_true_assumption_on_true_value
        {
            [Test]
            public void should_not_throw()
            {
                Action action = () => Assumes.True(true, "oops");
                action.ShouldNotThrow<Assumes.InternalErrorException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_failing_assumption
        {
            [Test]
            public void should_throw_internal_exception()
            {
                Action action = () => Assumes.Fail("oops");
                action.ShouldThrow<Assumes.InternalErrorException>();
            }
        }
    }
    // ReSharper restore InconsistentNaming
}
