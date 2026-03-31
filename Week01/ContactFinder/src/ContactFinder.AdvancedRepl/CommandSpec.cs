using System;

namespace ContactFinder.AdvancedRepl
{
    public sealed record CommandSpec(string Usage, Func<string[], bool> Handler);


}
