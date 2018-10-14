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
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.CodeContracts;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    using System.IO;
    using System.IO;

    public interface IDoSomethingSpecial
    {
        void Invoke();
    }

    public abstract class CSharpSyntaxWalkerSourceBase
    {
        public virtual void Reset()
        {
            return;
        }
    }

    public class CSharpSyntaxWalkerSource : CSharpSyntaxWalkerSourceBase, IDoSomethingSpecial
    {
        public void Invoke()
        {
            var o = new biz.dfch.CS.CodeContracts.Core.Tests.Roslyn.CSharpSyntaxWalkerSource.NestedClass();

            return;
        }

        private class NestedClass : IDoSomethingSpecial
        {
            public void Invoke()
            {
                throw new NotImplementedException();
            }

            private class NestedNestedClass
            {
                public void Test()
                {
                    return;
                }
            }
        }
    }
}
