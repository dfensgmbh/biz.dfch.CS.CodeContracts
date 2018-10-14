/**
 * Copyright 2018 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using biz.dfch.CS.CodeContracts.Core.Tests.Benchmark;
using biz.dfch.CS.CodeContracts.Core.Tests.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Attributes
{
    public class ArbitraryClass
    {
        // it is slightly faster to perform a direct cast (or even 2) than to use "as string"

        public bool InvokeReturnTrue<T>(T value)
        {
            return true;
        }

        public bool InvokeDirectCast<T>(T value)
        {
            return null != (string) (object) value;
        }

        public bool InvokeAsOperator<T>(T value)
        {
            var valueAsString = value as string;
            return null != valueAsString;
        }
    }

    public class StringLengthAttributeBenchmark : BenchmarkBase
    {
        [Benchmark]
        public static void Direct()
        {
            var result = 42;
            return;
        }

        [Benchmark]
        public static void InvokeReturnTrue()
        {
            var value = "arbitraryString";
            var flag = 0;
            var result = new ArbitraryClass().InvokeReturnTrue(value);
            flag = result ? 42 : 667;
        }

        [Benchmark]
        public static void InvokeDirectCast()
        {
            var value = "arbitraryString";
            var flag = 0;
            var result = new ArbitraryClass().InvokeDirectCast(value);
            flag = result ? 42 : 667;
        }

        [Benchmark]
        public static void InvokeAsOperator()
        {
            var value = "arbitraryString";
            var flag = 0;
            var result = new ArbitraryClass().InvokeAsOperator(value);
            flag = result ? 42 : 667;
        }
    }

    [TestClass]
    public class StringLengthAttributeBenchmarkTest
    {
        [TestMethod]
        public void Run()
        {
            var summary = BenchmarkRunner.Run<StringLengthAttributeBenchmark>(new BenchmarkBase.AllowNonOptimized());
            var report = summary.GetMarkdownReport();

            Trace.WriteLine(report);
        }
    }
}
