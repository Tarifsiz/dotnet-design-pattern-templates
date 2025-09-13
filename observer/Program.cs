using System;

public class Program
{
    public static void Main()
    {
        const int threshold = 30_000;

        var emailPriceChangeNotifier = new EmailPriceChangeNotifier(threshold);
        var pushPriceChangeNotifier  = new PushPriceChangeNotifier(threshold);
        var goldPriceReader = new GoldPriceReader();
        goldPriceReader.AttachObserver(emailPriceChangeNotifier);
        goldPriceReader.AttachObserver(pushPriceChangeNotifier);

        for(int i = 0; i < 3; ++i)
        {
            goldPriceReader.ReadCurrentPrice();
        }

        Console.Read();
    }
}


public class GoldPriceReader : IObservable<decimal>
{
    private int _currentGoldPrice;
    private readonly List<IObserver<decimal>> _observers = new List<IObserver<decimal>>();

    public void ReadCurrentPrice()
    {
        _currentGoldPrice = new Random().Next(20_000, 50_000);

        NotifyObservers();
    }

    public void AttachObserver(IObserver<decimal> observer)
    {
        _observers.Add(observer);
    }

    public void DetachObserver(IObserver<decimal> observer)
    {
        _observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach(var observer in _observers)
        {
            observer.Update(_currentGoldPrice);
        }
    }
}

public class EmailPriceChangeNotifier : IObserver<decimal>
{
    private readonly decimal _notificationThreshold;

    public EmailPriceChangeNotifier(decimal notificationThreshold)
    {
        _notificationThreshold = notificationThreshold;
    }

    public void Update(decimal price)
    {
        if (price > _notificationThreshold)
        {
            Console.WriteLine(
                $"Sending an email saying that " +
                $"the gold price exceeded {_notificationThreshold} " +
                $"and is now {price}\n"
            );
        }
    }
}

public class PushPriceChangeNotifier : IObserver<decimal>
{
    private readonly decimal _notificationThreshold;

    public PushPriceChangeNotifier(decimal notificationThreshold)
    {
        _notificationThreshold = notificationThreshold;
    }
    public void Update(decimal price)
    {
           Console.WriteLine(
                $"Sending a push notification saying that " +
                $"the gold price exceeded {_notificationThreshold} " +
                $"and is now {price}\n"
            );
    }
}

public interface IObserver<TData>
{
    void Update(TData data);
}

public interface IObservable<TData>
{
    void AttachObserver(IObserver<TData> observer);
    void DetachObserver(IObserver<TData> observer);
    void NotifyObservers();
}
