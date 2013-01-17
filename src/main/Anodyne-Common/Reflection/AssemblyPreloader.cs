namespace Kostassoid.Anodyne.Common.Reflection
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Extentions;

    public static class AssemblyPreloader
    {
        public static void Preload(IEnumerable<FileInfo> files)
        {
            files.ForEach(fi => Assembly.LoadFrom(fi.FullName));
        }
    }
}