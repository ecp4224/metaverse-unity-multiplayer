using System;

public class PendingValueFuture<T> : ValueFuture<T> where T : class
{
    public Func<T> onCheck;

    private void Update()
    {
        if (Value == null)
        {
            var result = onCheck();
            if (result != null)
            {
                Resolve(result);
            }
        }
    }
}