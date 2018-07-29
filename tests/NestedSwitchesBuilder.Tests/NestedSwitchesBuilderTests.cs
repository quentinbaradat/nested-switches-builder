// NestedSwitchesBuilderTests.cs
//
// Copyright © 2018 Quentin Baradat
//
// Licensed under the Apache License, Version 2.0 <LICENSE-APACHE or
// http://www.apache.org/licenses/LICENSE-2.0> or the MIT license
// <LICENSE-MIT or http://opensource.org/licenses/MIT>, at your
// option. This file may not be copied, modified, or distributed
// except according to those terms.

using System;
using System.Linq;
using Xunit;

namespace NestedSwitchesBuilder.Tests
{
    public class NestedSwitchesBuilderTests
    {
        [Fact]
        public void Basic()
        {

            NestedSwitchesBuilder<char, Func<String>> builder = new NestedSwitchesBuilder<char, Func<String>>();
            builder.Add("ACF".AsEnumerable(), () => "acf");
            builder.Add("ACV".AsEnumerable(), () => "acv");
            builder.Add("AG".AsEnumerable(), () => "ag");
            builder.Add("EC".AsEnumerable(), () => "ec");
            var nestedSwitches = builder.Build();

            Assert.Equal("acf", nestedSwitches("ACF".AsEnumerable())());
            Assert.Equal("acv", nestedSwitches("ACV".AsEnumerable())());
            Assert.Equal("ag", nestedSwitches("AG".AsEnumerable())());
            Assert.Equal("ec", nestedSwitches("EC".AsEnumerable())());
        }

        [Fact]
        public void NotFound()
        {

            NestedSwitchesBuilder<char, Func<String>> builder = new NestedSwitchesBuilder<char, Func<String>>();
            builder.Add("ACF".AsEnumerable(), () => "acf");
            builder.Add("ACV".AsEnumerable(), () => "acv");
            builder.Add("AG".AsEnumerable(), () => "ag");
            builder.Add("EC".AsEnumerable(), () => "ec");
            var nestedSwitches = builder.Build();

            Assert.Null(nestedSwitches("ACE".AsEnumerable()));
            Assert.Null(nestedSwitches("AC".AsEnumerable()));
            Assert.Null(nestedSwitches("I".AsEnumerable()));
            Assert.Null(nestedSwitches("".AsEnumerable()));
        }
    }
}
