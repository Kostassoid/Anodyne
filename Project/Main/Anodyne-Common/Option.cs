namespace Kostassoid.Anodyne.Common
{
    public abstract class Option<T> where T : class
    {
        public static Option<T> None { get { return new None<T>(); }}
        public static Option<T> Some(T value) { return new Some<T>(value); }

        public abstract T Value { get; }
        public abstract bool IsSome { get; }
        public abstract bool IsNone { get; }

        public static implicit operator Option<T>(T value)
        {
            return value.AsOption();
        }

        public static implicit operator bool(Option<T> option)
        {
            return option.IsSome;
        }

        public static explicit operator T(Option<T> option)
        {
            return option.Value;
        }

    }

    public sealed class Some<T> : Option<T> where T : class
    {
        private readonly T _value;
        public Some(T value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException("value", "Some value was null, use None instead");
            }

            _value = value;
        }

        public override T Value { get { return _value; } }
        public override bool IsSome { get { return true; } }
        public override bool IsNone { get { return false; } }
    }

    public sealed class None<T> : Option<T> where T : class
    {
        public override T Value
        {
            get { throw new System.NotSupportedException("There is no value"); }
        }

        public override bool IsSome { get { return false; } }
        public override bool IsNone { get { return true; } }
    }

    public static class OptionEx
    {
        public static Option<T> AsOption<T>(this T value) where T : class
        {
            if (value == null) return new None<T>();

            return new Some<T>(value);
        }
    }
}