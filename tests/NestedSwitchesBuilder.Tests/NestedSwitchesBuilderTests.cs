
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
using System.Collections.Generic;
using Xunit;

namespace NestedSwitchesBuilder.Tests
{
    public class NestedSwitchesBuilderTests
    {
        [Fact]
        public void Basic()
        {

            NestedSwitchesBuilder<char, Func<String>> builder = new NestedSwitchesBuilder<char, Func<String>>();
            builder.Add(new List<char>() { 'A', 'C', 'F' }, () => "acf");
            builder.Add(new List<char>() { 'A', 'C', 'V' }, () => "acv");
            builder.Add(new List<char>() { 'A', 'G' }, () => "ag");
            builder.Add(new List<char>() { 'E', 'C' }, () => "ec");
            var nestedSwitches = builder.Build();

            Assert.Equal("acf", nestedSwitches(new List<char>() { 'A', 'C', 'F' })());
            Assert.Equal("acv", nestedSwitches(new List<char>() { 'A', 'C', 'V' })());
            Assert.Equal("ag", nestedSwitches(new List<char>() { 'A', 'G' })());
            Assert.Equal("ec", nestedSwitches(new List<char>() { 'E', 'C' })());
        }

        [Fact]
        public void NotFound()
        {

            NestedSwitchesBuilder<char, Func<String>> builder = new NestedSwitchesBuilder<char, Func<String>>();
            builder.Add(new List<char>() { 'A', 'C', 'F' }, () => "acf");
            builder.Add(new List<char>() { 'A', 'C', 'V' }, () => "acv");
            builder.Add(new List<char>() { 'A', 'G' }, () => "ag");
            builder.Add(new List<char>() { 'E', 'C' }, () => "ec");
            var nestedSwitches = builder.Build();

            Assert.Null(nestedSwitches(new List<char>() { 'A', 'C', 'E' }));
            Assert.Null(nestedSwitches(new List<char>() { 'A', 'C' }));
            Assert.Null(nestedSwitches(new List<char>() { 'I' }));
            Assert.Null(nestedSwitches(new List<char>()));
        }
    }
}
