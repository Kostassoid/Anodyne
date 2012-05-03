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

    using Kostassoid.Anodyne.Common.CodeContracts;

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
                Assert.Throws<ArgumentNullException>(() => Requires.NotNull(value, "value"));
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
                string value = "zzz";

                Assert.DoesNotThrow(() => Requires.NotNull(value, "value"));
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
                Assert.Throws<ArgumentNullException>(() => Requires.NotNullOrEmpty(value, "value"));
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
                string value = String.Empty;
                Assert.Throws<ArgumentException>(() => Requires.NotNullOrEmpty(value, "value"));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_null_or_empty_requirement_on_non_null_value
        {
            [Test]
            public void should_not_throw()
            {
                string value = "zzz";
                Assert.DoesNotThrow(() => Requires.NotNullOrEmpty(value, "value"));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_true_assumption_on_false_value
        {
            [Test]
            public void should_throw_internal_exception()
            {
                Assert.Throws<Assumes.InternalErrorException>(() => Assumes.True(false, "oops"));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_checking_true_assumption_on_true_value
        {
            [Test]
            public void should_not_throw()
            {
                Assert.DoesNotThrow(() => Assumes.True(true, "oops"));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_failing_assumption
        {
            [Test]
            public void should_throw_internal_exception()
            {
                Assert.Throws<Assumes.InternalErrorException>(() => Assumes.Fail("oops"));
            }
        }



    }
    // ReSharper restore InconsistentNaming

}
