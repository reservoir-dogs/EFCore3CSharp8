using System;
using System.Collections.Generic;
using System.Text;

namespace EFCore3CSharp8
{
    public class Foo
    {
        public long Id { get; set; }

        public string Required { get; set; } = string.Empty;

        public string? Nullable { get; set; }
    }
}
