﻿using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using Should;

namespace GraphQL.Tests.Execution
{
    public class ResolveFieldContextTests
    {
        private readonly ResolveFieldContext _context;

        public ResolveFieldContextTests()
        {
            _context = new ResolveFieldContext();
            _context.Arguments = new Dictionary<string, object>();
        }

        [Fact]
        public void argument_converts_int_to_long()
        {
            int val = 1;
            _context.Arguments["a"] = val;
            var result = _context.Argument<long>("a");
            result.ShouldEqual(1);
        }

        [Fact]
        public void argument_converts_long_to_int()
        {
            long val = 1;
            _context.Arguments["a"] = val;
            var result = _context.Argument<int>("a");
            result.ShouldEqual(1);
        }

        [Fact]
        public void argument_returns_boxed_string_uncast()
        {
            _context.Arguments["a"] = "one";
            var result = _context.Argument<object>("a");
            result.ShouldEqual("one");
        }

        [Fact]
        public void argument_returns_long()
        {
            long val = 1000000000000001;
            _context.Arguments["a"] = val;
            var result = _context.Argument<long>("a");
            result.ShouldEqual(1000000000000001);
        }

        [Fact]
        public void argument_returns_enum()
        {
            _context.Arguments["a"] = SomeEnum.Two;
            var result = _context.Argument<SomeEnum>("a");
            result.ShouldEqual(SomeEnum.Two);
        }

        [Fact]
        public void argument_returns_enum_from_string()
        {
            _context.Arguments["a"] = "two";
            var result = _context.Argument<SomeEnum>("a");
            result.ShouldEqual(SomeEnum.Two);
        }

        [Fact]
        public void argument_returns_enum_from_number()
        {
            _context.Arguments["a"] = 1;
            var result = _context.Argument<SomeEnum>("a");
            result.ShouldEqual(SomeEnum.Two);
        }

        [Fact]
        public void throw_error_if_argument_doesnt_exist()
        {
            Expect.Throws<ExecutionError>(() => _context.Argument<string>("wat"));
        }

        [Fact]
        public void argument_returns_list_from_array()
        {
            _context.Arguments = "{a: ['one', 'two']}".ToInputs();
            var result = _context.Argument<List<string>>("a");
            result.ShouldNotBeNull();
            result.Count.ShouldEqual(2);
            result[0].ShouldEqual("one");
            result[1].ShouldEqual("two");
        }

        enum SomeEnum
        {
            One,
            Two
        }
    }
}
