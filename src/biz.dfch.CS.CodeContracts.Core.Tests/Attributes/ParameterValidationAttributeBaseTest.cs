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
using System.Linq;
using System.Reflection;
using System.Threading;
using biz.dfch.CS.CodeContracts.Core.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Attributes
{
    public static class InstanceCounter
    {
        private static int _counter = 0;
        public static int Count => _counter;

        public static void Increment()
        {
            Interlocked.Increment(ref _counter);
        }

        public static void Reset()
        {
            _counter = 0;
        }
    }

    public class ArbitraryParameterValidationAttribute : ParameterValidationAttributeBase
    {
        public ArbitraryParameterValidationAttribute()
        {
            InstanceCounter.Increment();
        }

        public ArbitraryParameterValidationAttribute(string message)
            : base(message)            
        {
            InstanceCounter.Increment();
        }
    }

    public class ArbitraryClassToBeValidated
    {
        public const string INVOKE_ATTRIBUTE_TEST_MESSAGE = "arbitrary message";
        public const string PARAMETER_NAME = "value";

        [return: ArbitraryParameterValidation]
        public void InvokeAttributeTest1([ArbitraryParameterValidation] string value)
        {
            return;
        }

        public void InvokeAttributeTest2([ArbitraryParameterValidation(Message = INVOKE_ATTRIBUTE_TEST_MESSAGE)] string value)
        {
            return;
        }

        public void InvokeAttributeTest3([ArbitraryParameterValidation(INVOKE_ATTRIBUTE_TEST_MESSAGE)] string value)
        {
            return;
        }
    }

    [TestClass]
    public class ParameterValidationAttributeBaseTest
    {
        [TestMethod]
        public void GettingAttributeWithDefaultMessagePropertyReturnsDefaultString()
        {
            // Arrange
            InstanceCounter.Reset();
            var instance = new ArbitraryClassToBeValidated();
            var methodInfo = instance.GetType().GetMethod(nameof(ArbitraryClassToBeValidated.InvokeAttributeTest1));
            Assert.IsNotNull(methodInfo);
            var parameterInfo = methodInfo.GetParameters().FirstOrDefault(e => e.Name == ArbitraryClassToBeValidated.PARAMETER_NAME);
            Assert.IsNotNull(parameterInfo);
            Assert.AreEqual(0, InstanceCounter.Count);
            var sut = parameterInfo.GetCustomAttribute<ArbitraryParameterValidationAttribute>();

            // Act
            var result = sut.Message;

            // Assert
            Assert.AreEqual(ValidationAttributeBase.MESSAGE_DEFAULT, result);
            Assert.AreEqual(1, InstanceCounter.Count);
        }

        [TestMethod]
        public void GettingAttributeWithSpecifiedMessagePropertyReturnsSpecifiedString()
        {
            // Arrange
            InstanceCounter.Reset();
            var instance = new ArbitraryClassToBeValidated();
            var methodInfo = instance.GetType().GetMethod(nameof(ArbitraryClassToBeValidated.InvokeAttributeTest2));
            Assert.IsNotNull(methodInfo);
            var parameterInfo = methodInfo.GetParameters().FirstOrDefault(e => e.Name == ArbitraryClassToBeValidated.PARAMETER_NAME);
            Assert.IsNotNull(parameterInfo);
            Assert.AreEqual(0, InstanceCounter.Count);
            var sut = parameterInfo.GetCustomAttribute<ArbitraryParameterValidationAttribute>();

            // Act
            var result = sut.Message;

            // Assert
            Assert.AreEqual(ArbitraryClassToBeValidated.INVOKE_ATTRIBUTE_TEST_MESSAGE, result);
            Assert.AreEqual(1, InstanceCounter.Count);
        }

        [TestMethod]
        public void GettingAttributeWithSpecifiedMessageInCtorReturnsSpecifiedString()
        {
            // Arrange
            InstanceCounter.Reset();
            var instance = new ArbitraryClassToBeValidated();
            var methodInfo = instance.GetType().GetMethod(nameof(ArbitraryClassToBeValidated.InvokeAttributeTest3));
            Assert.IsNotNull(methodInfo);
            var parameterInfo = methodInfo.GetParameters().FirstOrDefault(e => e.Name == ArbitraryClassToBeValidated.PARAMETER_NAME);
            Assert.IsNotNull(parameterInfo);
            Assert.AreEqual(0, InstanceCounter.Count);
            var sut = parameterInfo.GetCustomAttribute<ArbitraryParameterValidationAttribute>();

            // Act
            var result = sut.Message;

            // Assert
            Assert.AreEqual(ArbitraryClassToBeValidated.INVOKE_ATTRIBUTE_TEST_MESSAGE, result);
            Assert.AreEqual(1, InstanceCounter.Count);
        }
    }
}
