// -----------Behavioral Design Pattern-----------

// 1. Command Pattern → Encapsulates a request as an object, thereby allowing for parameterization of clients with queues, requests, and operations.

public interface ICommand
{
    void Execute();
}
// receiver
public class BankAccount
{
    private decimal _balance;
    public BankAccount(decimal initialBalance)
    {
        _balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        _balance += amount;
        Console.WriteLine($"Deposited: {amount}, New Balance: {_balance}");
    }
    public void Withdraw(decimal amount)
    {
        if (_balance >= amount)
        {
            _balance -= amount;
            Console.WriteLine($"Withdrew: {amount}, New Balance: {_balance}");
        }
        else
        {
            Console.WriteLine($"Insufficient funds for withdrawal of {amount}. Current Balance: {_balance}");
        }
    }
}

public class DepositCommand : ICommand
{
    private BankAccount _account;
    private decimal _amount;

    public DepositCommand(BankAccount account, decimal amount)
    {
        _account = account;
        _amount = amount;
    }

    public void Execute()
    {
        _account.Deposit(_amount);
    }
}

public class WithdrawCommand : ICommand
{
    private BankAccount _account;
    private decimal _amount;

    public WithdrawCommand(BankAccount account, decimal amount)
    {
        _account = account;
        _amount = amount;
    }

    public void Execute()
    {
        _account.Withdraw(_amount);
    }
}

public class ATM
{
    private ICommand _command;

    public void SetCommand(ICommand command)
    {
        _command = command;
    }
    public void ExecuteTransaction()
    {
        _command.Execute();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BankAccount account = new BankAccount(1000);
        ATM atm = new ATM();

        ICommand depositCommand = new DepositCommand(account, 200);
        ICommand withdrawCommand = new WithdrawCommand(account, 150);

        atm.SetCommand(depositCommand);
        atm.ExecuteTransaction(); // Deposited: 200, New Balance: 1200

        atm.SetCommand(withdrawCommand);
        atm.ExecuteTransaction(); // Withdrew: 150, New Balance: 1050

    }
}

// 2. Chain of Responsibility Pattern → Avoids coupling the sender of a request to its receiver by giving more than one object a chance to handle the request. Chain the receiving objects and pass the request along the chain until an object handles it.


public abstract class LeaveApprover
{
    private LeaveApprover _nextApprover;

    public LeaveApprover(LeaveApprover next)
    {
        _nextApprover = nextApprover;
    }

    public abstract void ApproveLeave(int days);

}

// team lead
public class TeamLead : LeaveApprover
{


    public override void ApproveLeave(int days)
    {
        if (days <= 2)
        {
            Console.WriteLine("Team Lead approved leave for " + days + " days.");
        }
        else if (_nextApprover != null)
        {
            _nextApprover.ApproveLeave(days);
        }
    }
}

public class Manager : LeaveApprover
{


    public override void ApproveLeave(int days)
    {
        if (days <= 7)
        {
            Console.WriteLine("Manager approved leave for " + days + " days.");
        }
        else if (_nextApprover != null)
        {
            _nextApprover.ApproveLeave(days);
        }
    }
}

public class Director : LeaveApprover
{
    public override void ApproveLeave(int days)
    {
        if (days <= 14)
        {
            Console.WriteLine("Director approved leave for " + days + " days.");
        }
        else
        {
            Console.WriteLine("Leave request for " + days + " days requires higher approval.");
        }
    }
}


class Program2
{
    public static void Main(string[] args)
    {
        LeaveApprover director = new Director();
        LeaveApprover manager = new Manager();
        LeaveApprover teamLead = new TeamLead();

        teamLead.SetNextApprover(manager);
        manager.SetNextApprover(director);

        teamLead.ApproveLeave(1);  // Team Lead approved leave for 1 days.
        teamLead.ApproveLeave(5);  // Manager approved leave for 5 days.
        teamLead.ApproveLeave(10); // Director approved leave for 10 days.
        teamLead.ApproveLeave(20); // Leave request for 20 days requires higher approval.
    }
}


// 3.Iterator Pattern → Provides a way to access the elements of an aggregate object sequentially without exposing its underlying representation.

public interface IIterator
{
    bool HasNext();
    string Next();
}

public class StudentIterator : IIterator
{
    public List<string> students;
    private int index = 0;

    public StudentIterator(List<string> students)
    {
        this.students = students;
    }

    public bool HasNext()
    {
        return index < students.Count;
    }

    public string Next()
    {

        return students[index++];

    }
}

public class StudentCollection
{
    private List<string> students = new List<string>();

    public void AddStudent(string name)
    {
        students.Add(name);
    }

    public IIterator GetIterator()
    {
        return new StudentIterator(students);
    }
}

class Program3
{
    public static void Main(string[] args)
    {
        StudentCollection studentCollection = new StudentCollection();
        studentCollection.AddStudent("Alice");
        studentCollection.AddStudent("Bob");
        studentCollection.AddStudent("Charlie");

        IIterator iterator = studentCollection.GetIterator();

        while (iterator.HasNext())
        {
            string student = iterator.Next();
            Console.WriteLine(student);
        }
    }
}

// 4. Mediator Pattern → Defines an object that encapsulates how a set of objects interact. This pattern promotes loose coupling by keeping objects from referring to each other explicitly, and it lets you vary their interaction independently.

public interface IMediator
{
    void SendMessage(string message, User sender);
}

public class ChatMediator : IMediator
{
    private List<User> users = new List<User>();

    public void AddUser(User user)
    {
        users.Add(user);
    }

    public void SendMessage(string message, User sender)
    {
        foreach (var user in users)
        {
            if (user != sender)
            {
                user.Receive(message);
            }
        }
    }
}

public class User
{
    private string mediator;
    public string Name { get; private set; }
    public User(string name, string mediator)
    {
        Name = name;
        this.mediator = mediator;
    }

    public void Send(string message)
    {
        Console.WriteLine($"{Name} sends: {message}");
        mediator.SendMessage(message, this);
    }

    public void Receive(string message)
    {
        Console.WriteLine($"{Name} receives: {message}");
    }

}

class Program4
{
    public static void Main(string[] args)
    {
        ChatMediator mediator = new ChatMediator();

        User user1 = new User("Alice", mediator);
        User user2 = new User("Bob", mediator);
        User user3 = new User("Charlie", mediator);

        mediator.AddUser(user1);
        mediator.AddUser(user2);
        mediator.AddUser(user3);

        user1.Send("Hello, everyone!");
        user2.Send("Hi Alice!");
        user3.Send("Hey folks!");
    }
}

// 5. Observer Pattern → Defines a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically.

public interface IObserver
{
    void Update(string message);
}

public class Subscriber : IObserver
{
    private string name;

    public Subscriber(string name)
    {
        this.name = name;
    }

    public void Update(string message)
    {
        Console.WriteLine($"{name} received message: {message}");
    }
}


public class YoutubeChannel
{
    private List<IObserver> subscribers = new List<IObserver>();

    public void Subscribe(IObserver subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(IObserver subscriber)
    {
        subscribers.Remove(subscriber);
    }

    public void NotifySubscribers(string message)
    {
        foreach (var subscriber in subscribers)
        {
            subscriber.Update(message);
        }
    }
}

class Program5
{
    public static void Main(string[] args)
    {
        YoutubeChannel channel = new YoutubeChannel();

        Subscriber subscriber1 = new Subscriber("Alice");
        Subscriber subscriber2 = new Subscriber("Bob");

        channel.Subscribe(subscriber1);
        channel.Subscribe(subscriber2);

        channel.NotifySubscribers("New video uploaded!");

        channel.Unsubscribe(subscriber1);

        channel.NotifySubscribers("Another video uploaded!");
    }
}

