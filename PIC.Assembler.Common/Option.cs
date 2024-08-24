namespace PIC.Assembler.Common;

public class Option<T> where T : class
{
    private readonly T? _t;

    private Option(T? t)
    {
        _t = t;
    }

    public static Option<T> Some(T t)
    {
        return new Option<T>(t);
    }

    public static Option<T> None()
    {
        return new Option<T>(null);
    }

    public bool HasValue()
    {
        return _t != null;
    }

    public T Get()
    {
        return _t ?? throw new InvalidOperationException();
    }

    public Option<T> OrElse(Option<T> otherOption)
    {
        return HasValue() ? this : otherOption;
    }

    public T OrElseGet(T other)
    {
        return HasValue() ? Get() : other;
    }
}
