using UnityEngine;

public class ValueFuture<T> : MonoBehaviour where T : class
{
    public T Value { get; protected set; }

    public ValueFuture<T> Resolve(T value)
    {
        this.Value = value;

        return this;
    }

    public override bool Equals(object other)
    {
        if (other == null)
            return false;
        
        var fother = other as ValueFuture<T>;
        if (fother != null)
        {
            return Equals(fother.Value);
        }

        if (other is T tother)
        {
            return tother.Equals(Value);
        }

        return false;
    }
}