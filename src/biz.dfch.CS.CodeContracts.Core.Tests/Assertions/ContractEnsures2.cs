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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using biz.dfch.CS.Testing.Attributes;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Assertions
{
    public static class ContractEnsures2
    {
        public static T ResultFunc<T>(Func<T> func, Func<T, bool> postCondition)
        {
            var result = default(T);

            try
            {
                result = func.Invoke();
                return result;
            }
            finally
            {
                var b = postCondition.Invoke(result);
                System.Diagnostics.Contracts.Contract.Assert(b, "Postcondition failed: " + postCondition);
            }
        }

        public static T ResultExpr<T>(Func<T> func, Expression<Func<T, bool>> postCondition)
        {
            var result = default(T);

            try
            {
                result = func.Invoke();
                return result;
            }
            finally
            {
                var b = postCondition.Compile().Invoke(result);
                System.Diagnostics.Contracts.Contract.Assert(b, "Postcondition failed: " + postCondition);
            }
        }

        public static T ResultCompExpr<T>(Func<T> func, Expression<Func<T, bool>> postCondition)
        {
            var result = default(T);

            try
            {
                result = func.Invoke();
                return result;
            }
            finally
            {
                var b = postCondition.Compile().Invoke(result);
                System.Diagnostics.Contracts.Contract.Assert(b, "Postcondition failed: " + postCondition);
            }
        }
    }

    [TestClass]
    public class ContractEnsuresTest
    {
        public class ArbitraryClass
        {
            public string ReturnsParam1WithDirect(string param1, object param2, int param3)
            {
                var result = default(string);
                result = param1;
                return result;
            }

            public string ReturnsParam1WithTryf(string param1, object param2, int param3)
            {
                var result = default(string);
                try
                {
                    result = param1;
                    return result;
                }
                finally
                {
                    if (null == result) throw new ArgumentNullException();
                }
            }

            public string ReturnsParam1WithFunc(string param1, object param2, int param3)
            {
                return ContractEnsures2.ResultFunc(PrivateLocal, r => !string.IsNullOrWhiteSpace(r));

                string PrivateLocal()
                {
                    return param1;
                }
            }

            public string ReturnsParam1WithExpr(string param1, object param2, int param3)
            {
                return ContractEnsures2.ResultExpr(PrivateLocal, result => !string.IsNullOrWhiteSpace(result));

                string PrivateLocal()
                {
                    return param1;
                }
            }
        }

        [TestMethod]
        public void ContractEnsuresResultFuncReturnsCorrectResult()
        {
            // Arrange
            string arbitraryString = nameof(arbitraryString);
            var sut = new ArbitraryClass();

            // Act
            var result = sut.ReturnsParam1WithFunc(arbitraryString, new object(), 42);

            // Assert
            Assert.AreEqual(arbitraryString, result);
        }

        [ExpectedContractException(MessagePattern = @"Postcondition.*IsNullOrWhiteSpace")]
        [TestMethod]
        public void ContractEnsuresResultFuncThrowsContractException()
        {
            // Arrange
            string emptyString = "";
            var sut = new ArbitraryClass();

            // Act
            var result = sut.ReturnsParam1WithFunc(emptyString, new object(), 42);

            // Assert
            Assert.AreEqual(emptyString, result);
        }

        [TestMethod]
        public void ContractEnsuresResultExprReturnsCorrectResult()
        {
            // Arrange
            string arbitraryString = nameof(arbitraryString);
            var sut = new ArbitraryClass();

            // Act
            var result = sut.ReturnsParam1WithExpr(arbitraryString, new object(), 42);

            // Assert
            Assert.AreEqual(arbitraryString, result);
        }

        [ExpectedContractException(MessagePattern = @"Postcondition.*IsNullOrWhiteSpace")]
        [TestMethod]
        public void ContractEnsuresResultExprThrowsContractException()
        {
            // Arrange
            string emptyString = "";
            var sut = new ArbitraryClass();

            // Act
            var result = sut.ReturnsParam1WithExpr(emptyString, new object(), 42);

            // Assert
            Assert.AreEqual(emptyString, result);
        }

    }

    public static class SummaryExtensions
    {
        public static string GetMarkdownReport(this Summary summary)
        {
            System.Diagnostics.Contracts.Contract.Requires(null != summary);
            //Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
            return ContractEnsures.Result(PrivateLocal, r => !string.IsNullOrWhiteSpace(r));
            string PrivateLocal()
            {
                var fileName = Path.Combine
                (
                    summary.ResultsDirectoryPath,
                    string.Format("{0}-report-github.md", summary.Title)
                );

                var report = File.ReadAllText(fileName);
                return report;
            }
        }
    }

    [Config(typeof(AllowNonOptimized))]
    public abstract class BenchmarkBase
    {
        // https://github.com/dotnet/BenchmarkDotNet/issues/579#issuecomment-345464911
        public class AllowNonOptimized : ManualConfig
        {
            public AllowNonOptimized()
            {
                Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS
                Add(GenericBenchmarksValidator.DontFailOnError);
                Add(ExecutionValidator.DontFailOnError);
                Add(InProcessValidator.DontFailOnError);
                Add(ReturnValueValidator.DontFailOnError);

                Add(DefaultConfig.Instance.GetLoggers().ToArray()); // manual config has no loggers by default
                Add(DefaultConfig.Instance.GetExporters().ToArray()); // manual config has no exporters by default
                Add(DefaultConfig.Instance.GetColumnProviders().ToArray()); // manual config has no columns by default
            }
        }
    }

    [Config(typeof(AllowNonOptimized))]
    public class BenchmarkTest : BenchmarkBase
    {
        [Benchmark]
        public static void Direct()
        {
            var result = 42;
            return;
        }

        [Benchmark]
        public static void TryFinally()
        {
            var result = default(int);
            try
            {
                result = 42;
                return;
            }
            finally
            {
                if (0 == result)
                {
                    throw new ArgumentNullException();
                }
            }
        }
    }

    [Config(typeof(AllowNonOptimized))]
    public class ContractEnsuresBenchmark : BenchmarkBase
    {
        static string @string = "arbitraryString";
        static object @object = new object();
        static int @int = 42;

        [Benchmark]
        public static void ReturnsParam1WithExpr()
        {
            var sut = new ContractEnsuresTest.ArbitraryClass();
            var result = sut.ReturnsParam1WithExpr(@string, @object, @int);
            if (null == result) throw new ArgumentNullException();
        }

        [Benchmark]
        public static void ReturnsParam1WithFunc()
        {
            var sut = new ContractEnsuresTest.ArbitraryClass();
            var result = sut.ReturnsParam1WithFunc(@string, @object, @int);
            if (null == result) throw new ArgumentNullException();
        }

        [Benchmark]
        public static void ReturnsParam1WithTryf()
        {
            var sut = new ContractEnsuresTest.ArbitraryClass();
            var result = sut.ReturnsParam1WithTryf(@string, @object, @int);
            if (null == result) throw new ArgumentNullException();
        }

        [Benchmark]
        public static void ReturnsParam1WithDirect()
        {
            var sut = new ContractEnsuresTest.ArbitraryClass();
            var result = sut.ReturnsParam1WithDirect(@string, @object, @int);
            if (null == result) throw new ArgumentNullException();
        }
    }

    [Config(typeof(AllowNonOptimized))]
    public class TryCatchFinallyBenchmark : BenchmarkBase
    {
        private static string DoSomethingPrivate(int x, int y)
        {
            return x.ToString() + y;
        }

        [Benchmark]
        public static void RunTryFinallyWithoutException()
        {
            var isNotNull = default(bool);

            try
            {
                var x = 0;
                var y = 42;

                var result = DoSomethingPrivate(x, y);

                isNotNull = null != result;
            }
            finally
            {
                if (!isNotNull) throw new Exception();
            }
        }

        [Benchmark]
        public static void RunTryCatchFinallyWithoutException()
        {
            var isNotNull = default(bool);

            try
            {
                var x = 0;
                var y = 42;

                var result = DoSomethingPrivate(x, y);

                isNotNull = null != result;
            }
            catch (Exception ex)
            {
                isNotNull = !isNotNull;
                throw;
            }
            finally
            {
                if (!isNotNull) throw new Exception();
            }
        }

        [Benchmark]
        public static void RunTryCatchWithoutException()
        {
            var isNotNull = default(bool);

            try
            {
                var x = 0;
                var y = 42;

                var result = DoSomethingPrivate(x, y);

                isNotNull = null != result;
            }
            catch (Exception ex)
            {
                if (!isNotNull) throw new Exception();
                throw;
            }
        }

        [Benchmark]
        public static void RunDirectWithoutException()
        {
            var isNotNull = default(bool);

            var x = 0;
            var y = 42;

            var result = DoSomethingPrivate(x, y);

            isNotNull = null != result;
            if (!isNotNull) throw new Exception();
        }
    }

    [Config(typeof(AllowNonOptimized))]
    public class LocalFunctionBenchmark : BenchmarkBase
    {
        private static string DoSomethingPrivate(int x, int y)
        {
            return x.ToString() + y;
        }

        private static string DoSomethingPrivateWithLocalFunction(int x, int y)
        {
            return LocalFunction();

            string LocalFunction()
            {
                return x.ToString() + y;
            }
        }

        [Benchmark]
        public static void RunWithoutLocalFunction()
        {
            var isNotNull = default(bool);

            var x = 0;
            var y = 42;

            var result = DoSomethingPrivate(x, y);

            isNotNull = null != result;
            if (!isNotNull) throw new Exception();
        }

        [Benchmark]
        public static void RunWitLocalFunction()
        {
            var isNotNull = default(bool);

            var x = 0;
            var y = 42;

            var result = DoSomethingPrivateWithLocalFunction(x, y);

            isNotNull = null != result;
            if (!isNotNull) throw new Exception();
        }
    }

    [TestClass]
    public class Diagnostics
    {
        [TestMethod]
        public void RunBenchmarkTest()
        {
            var summary = BenchmarkRunner.Run<BenchmarkTest>(new BenchmarkBase.AllowNonOptimized());
            var report = summary.GetMarkdownReport();

            Trace.WriteLine(report);
        }

        [TestMethod]
        public void RunContractEnsuresBenchmark()
        {
            var summary = BenchmarkRunner.Run<ContractEnsuresBenchmark>(new BenchmarkBase.AllowNonOptimized());
            var report = summary.GetMarkdownReport();

            Trace.WriteLine(report);
        }

        [TestMethod]
        public void RunTryCatchFinallyBenchmark()
        {
            var summary = BenchmarkRunner.Run<TryCatchFinallyBenchmark>(new BenchmarkBase.AllowNonOptimized());
            var report = summary.GetMarkdownReport();

            Trace.WriteLine(report);
        }

        [TestMethod]
        public void RunPrivateMethodBenchmark()
        {
            var summary = BenchmarkRunner.Run<LocalFunctionBenchmark>(new BenchmarkBase.AllowNonOptimized());
            var report = summary.GetMarkdownReport();

            Trace.WriteLine(report);
        }

        [TestMethod]
        public void RunSomething()
        {
            var isTrue = true;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            Contract.Requires(null != isTrue);
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

            var message = "tralala";
            new NotNullAttribute().Validate<string>(message);
            new StringLengthAttribute() { MinLength = 10, MaxLength = 20, }.Validate(message);
        }

        private void SomethingPrivate([StringLength(MinLength = 10, MaxLength = 20)] string message, int value, [NotNull] object obj)
        {
            #region CodeContractsPre
            //new
            #endregion

            return;
        }
    }

    public interface ICodeContractValidation
    {
        Exception Validate<T>(T value);
    }

    public abstract class CodeContractValidationAttributeBase : Attribute, ICodeContractValidation
    {
        protected Type Type;
        protected object Value;

        public virtual Exception Validate<T>(T value)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class NotNullAttribute : CodeContractValidationAttributeBase
    {
        public override Exception Validate<T>(T value)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class StringLengthAttribute : CodeContractValidationAttributeBase
    {
        public int MinLength;
        public int MaxLength;

        public override Exception Validate<T>(T value)
        {
            throw new NotImplementedException();
        }
    }

    public static class Contract
    {
        public static void Requires(bool condition)
        {
            return;
        }

        public static T Result<T>()
        {
            return default(T);
        }
    }
}