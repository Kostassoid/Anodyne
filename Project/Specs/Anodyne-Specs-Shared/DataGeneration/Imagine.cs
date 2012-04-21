using System;
using Kostassoid.Anodyne.Common;

namespace Kostassoid.Anodyne.Specs.Shared.DataGeneration
{
    public static class Imagine
    {
        internal static Random Random { get; private set; }

        private static int _seed;
        public static int RandomSeed { set
        {
            _seed = value;
            Reset();
        }}

        public static SimpleGenerator Any { get; private set; }

        static Imagine()
        {
            RandomSeed = (int)SystemTime.Now.Ticks;

            Any = new SimpleGenerator(() => Random);
        }

        public static void Reset()
        {
            Random = new Random(_seed);
        }

    }
}