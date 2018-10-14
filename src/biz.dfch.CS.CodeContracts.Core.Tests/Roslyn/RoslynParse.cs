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

using System.Collections.Generic;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    public class RoslynParse
    {
        public void Invoke()
        {
            var param1 = "arbitraryString";
            var param2 = 42;
            var param3 = new List<string>();

            var instance = new DoSomething();

            var result = instance.Invoke(param1, param2, param3);
        }
    }
}
