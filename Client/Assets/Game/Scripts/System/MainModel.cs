using System;

public class MainModel
{
    public event Action<int> TestAction;

    public int CurrValue;

    public void OnTest(int value)
    {
        CurrValue = value;
        TestAction?.Invoke(value);
    }
}
