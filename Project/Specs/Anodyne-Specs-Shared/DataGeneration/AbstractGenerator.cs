using System;

namespace Kostassoid.Anodyne.Specs.Shared.DataGeneration
{
    public abstract class AbstractGenerator
    {
        protected readonly Func<Random> Random;

        internal AbstractGenerator(Func<Random> random)
        {
            Random = random;
        }
    }
}