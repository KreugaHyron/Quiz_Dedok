using QuizApp;
class Program
{
    static void Main(string[] args)
    {
        var userManager = new UserManager();
        var quizManager = new QuizManager();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Quiz App!"); //предголовне меню
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: "); //вибір для користувача
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    RegisterUser(userManager);
                    break;

                case "2":
                    var user = LoginUser(userManager);
                    if (user != null)
                    {
                        UserMenu(quizManager, user, userManager);
                    }
                    break;

                case "3":
                    return; // Вихід з програми
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }
            Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися до меню.");
            Console.ReadKey();
        }
    }
    static void RegisterUser(UserManager userManager) //процесс реестрації нового користувача
    {
        Console.Clear();
        Console.Write("Введіть нікнейм: ");
        var username = Console.ReadLine();
        Console.Write("Введіть пароль: ");
        var password = Console.ReadLine();
        Console.Write("Введіть дату народження (рік-місяць-день): ");
        var birthDate = DateTime.Parse(Console.ReadLine());
        userManager.Register(username, password, birthDate); //!додати перевірку
        if (userManager.UserExists(username))
        {
            Console.WriteLine("Користувач з таким ім'ям вже існує. Спробуйте інший логін.");
            Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися до меню.");
            Console.ReadKey();
            return;
        }
        else
        {
            Console.WriteLine("Реестрація пройшла успішно. Натисніть клавішу,щоб перейти до предголовного меню.");
            Console.ReadKey();
        }
    }
    static User LoginUser(UserManager userManager) //процесс логіну вже існуючого(або новостворенного) користувача
    {
        Console.Clear();
        Console.Write("Enter username: ");
        var username = Console.ReadLine();
        Console.Write("Enter password: ");
        var password = Console.ReadLine();
        var user = userManager.Login(username, password);
        if (user != null)
        {
            Console.WriteLine($"Вітаю, {user.Username}!");
            Console.WriteLine("Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Неправильне ім'я або пароль. Натисніть кнопку,щоб повренутися до предменю.");
            Console.ReadKey();
        }
        return user;
    }
    static void UserMenu(QuizManager quizManager, User user, UserManager userManager) //вигляд головного меню
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("User Menu");
            Console.WriteLine("1. Start Quiz");
            Console.WriteLine("2. View My Results");
            Console.WriteLine("3. View Top Results");
            Console.WriteLine("4. Change Password");
            Console.WriteLine("5. Logout");
            Console.Write("Choose an option: ");
            var quizChoice = Console.ReadLine();
            switch (quizChoice)
            {
                case "1":
                    quizManager.ListQuizzes();
                    Console.Write("Enter quiz topic: ");
                    var topic = Console.ReadLine();
                    quizManager.StartQuiz(topic, user);
                    break;

                case "2":
                    quizManager.ShowResults(user.Username);
                    break;

                case "3":
                    quizManager.ShowTopResults();
                    break;

                case "4":
                    ChangePassword(userManager, user.Username);
                    break;

                case "5":
                    return; // Вихід з методу
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }
    static void ChangePassword(UserManager userManager, string username)
    {
        Console.Clear();
        Console.Write("Введіть новий пароль: ");
        var newPassword = Console.ReadLine();
        userManager.ChangePassword(username, newPassword);
    }
}