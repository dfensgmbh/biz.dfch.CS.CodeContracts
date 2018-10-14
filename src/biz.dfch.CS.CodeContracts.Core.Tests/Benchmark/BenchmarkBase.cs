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

using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Validators;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Benchmark
{
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
}