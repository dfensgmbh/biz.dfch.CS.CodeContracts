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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using biz.dfch.CS.CodeContracts.Core.Tests.Assertions;
using biz.dfch.CS.CodeContracts.Core.Tests.Benchmark;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Diagnostics
{
    public abstract class ValidationBaseAttribute : Attribute
    {
        public string Message;
    }

    public abstract class ParameterValidationBaseAttribute : ValidationBaseAttribute
    {
    }

    public abstract class MethodValidationBaseAttribute : ValidationBaseAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class NotNullAttribute : ParameterValidationBaseAttribute, ICodeContractValidation
    {
        public Exception Validate<T>(T value)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class ValidateIntAttribute : ParameterValidationBaseAttribute
    {
        public int MinValue;
        public int MaxValue;
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class ValidateStringAttribute : ParameterValidationBaseAttribute
    {
        public int MinLength;
        public int MaxLength;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MethodNotNullAttribute : MethodValidationBaseAttribute
    {
    }

    [AttributeUsage(AttributeTargets.ReturnValue)]
    public class ReturnValueNotNullAttribute : MethodValidationBaseAttribute
    {
        public readonly Type Validator;

        public ReturnValueNotNullAttribute(Type type)
        {
            Validator = type;
        }
    }

    public interface IValidateSomething
    {
        bool Validate(object value);
    }

    public class ValidateSomething : IValidateSomething
    {
        public bool Validate(object value)
        {
            throw new NotImplementedException();
        }
    }

    public interface IValidateSomethingGeneric<in T>
    {
        bool Validate(T value);
    }

    public class ValidateSomething<T> : IValidateSomethingGeneric<T>
    {
        public bool Validate(T value)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.ReturnValue)]
    public class ReturnValueAttribute : MethodValidationBaseAttribute
    {
        public readonly Type Validator;

        public ReturnValueAttribute(Type type)
        {
            Validator = type;
        }
    }

    public interface IDoSomething
    {
        [return: ReturnValue(typeof(ValidateSomething<string>), Message = "tralala1")]
        string Invoke([NotNull] [ValidateString(Message = "tralala2")]
            string param1, [ValidateInt] int param2, [NotNull] object param3);
    }

    public class DoSomething : IDoSomething
    {
        public string Invoke(string param1, int param2, object param3)
        {
            return param1;
        }
    }

    public static class TypeExtensions
    {
        private static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this Type type)
        {
            var attributeType = typeof(T);
            return type.GetCustomAttributes(attributeType, true)
                .Union(type.GetInterfaces()
                    .SelectMany(interfaceType => interfaceType.GetCustomAttributes(attributeType, true)))
                .Distinct().Cast<T>();
        }
    }

    public class PassThroughInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            var hasMethodNullAttribute = invocation.Method.ReturnTypeCustomAttributes
                .GetCustomAttributes(typeof(ReturnValueNotNullAttribute), true).Any();
            if (!hasMethodNullAttribute) return;

            if (null == invocation.ReturnValue)
            {
                throw new ArgumentNullException("ReturnValue", "Return value of method must not be null.");
            }
        }
    }

    public class ChangingInterceptor : IInterceptor
    {
        public const string NEW_VALUE = "tralala";

        public void Intercept(IInvocation invocation)
        {
            invocation.SetArgumentValue(0, NEW_VALUE);
            invocation.Proceed();
        }
    }

    [Config(typeof(BenchmarkBase.AllowNonOptimized))]
    public class CastleWindowsInterceptorBenchmark : BenchmarkBase
    {
        private WindsorContainer container;

        [GlobalSetup]
        public void GlobalSetup()
        {
            container = new WindsorContainer();
            // ? is there an easier way to specify interceptions
            // or do we have to do this for every interface / class ?
            container.Register(
                Component.For<IInterceptor>()
                    .ImplementedBy<PassThroughInterceptor>()
                    .Named(nameof(PassThroughInterceptor))
            );
            container.Register(
                Component.For<IDoSomething>()
                    .ImplementedBy<DoSomething>()
                    .Interceptors(InterceptorReference.ForKey(nameof(PassThroughInterceptor)))
                    .Anywhere
            );

            // ? is this needed and if yes, what for ?
            container.Register(Types.FromThisAssembly());
            container.Register(Classes.FromThisAssembly());
            container.Register(Types.FromAssemblyContaining<Core.IAmHere>());
            container.Register(Classes.FromAssemblyContaining<Core.IAmHere>());
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            container.Dispose();
        }

        [Benchmark]
        public void Direct()
        {
            var param1 = default(string);
            var param2 = 42;
            var param3 = new List<string>();

            var something = container.Resolve<IDoSomething>();
            var result = something.Invoke(param1, param2, param3);
            Assert.AreEqual(param1, result);
        }
    }

    public interface IHaveConstraints
    {
        [return: ReturnValueNotNull(typeof(ValidateSomething<string>), Message = "Return value must not be null.")]
        string Invoke([NotNull(Message = "Input string parameter 'value' must not be null.")] string value);
    }

    public class HaveConstraints : IHaveConstraints
    {
        public string Invoke(string value)
        {
            #region PRE auto-generated code

            var result20F9595F9Ecf49F2Adedfa71A6F56922 = default(string);
            try
            {
                Assert.IsNotNull(value, "Input string parameter 'value' must not be null.");
                result20F9595F9Ecf49F2Adedfa71A6F56922 = PrivateLocal();
                string PrivateLocal()
                {

                    #endregion

                    // original method code
                    var result = value + value;
                    return value;

                    #region POST auto-generated code

                }
            }
            finally
            {
                // based on [return: ReturnValueNotNull]
                Assert.IsNotNull(result20F9595F9Ecf49F2Adedfa71A6F56922, "Return value must not be null.");
            }
            return result20F9595F9Ecf49F2Adedfa71A6F56922;

            #endregion
        }
    }

    [TestClass]
    public class CastleWindowsInterceptorBenchmarkTest
    {
        [TestMethod]
        public void RunCastleWindowsInterceptorBenchmark()
        {
            var summary = BenchmarkRunner.Run<CastleWindowsInterceptorBenchmark>();
            var report = summary.GetMarkdownReport();

            Trace.WriteLine(report);
        }
    }
}
