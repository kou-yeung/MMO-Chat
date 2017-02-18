// Service Locator

class ServiceLocator<T>
{
    static T locator = default(T);

    static public void SetLocator(T locator)
    {
        ServiceLocator<T>.locator = locator;
    }
    static public T Locator
    {
        get { return ServiceLocator<T>.locator; }
    }
}

