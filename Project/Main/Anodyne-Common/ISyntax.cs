using System;
using System.ComponentModel;

namespace Kostassoid.Anodyne.Common
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ISyntax
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}