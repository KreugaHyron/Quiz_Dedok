using Newtonsoft.Json;
namespace QuizApp
{
    public class User //класс користувача
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
    }
    public class Question //класс питань
    {
        public string Text { get; set; }
        public List<string> Answers { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public bool IsCorrect(List<string> userAnswers)
        {
            return CorrectAnswers.All(ans => userAnswers.Contains(ans)) &&
                   userAnswers.All(ans => CorrectAnswers.Contains(ans));
        }
    }
    public class Quiz //класс вікторин
    {
        public string Topic { get; set; }
        public List<Question> Questions { get; set; }
        public Quiz(string topic)
        {
            Topic = topic;
            Questions = new List<Question>();
        }
    }
    public class Result //класс результатів
    {
        public string Username { get; set; }
        public int Score { get; set; }
    }
    public class QuizManager //класс менеджера з вікторин
    {
        private List<Quiz> quizzes = new List<Quiz>();
        public List<Result> Results { get; private set; } = new List<Result>();
        private const string QuizFile = "quizzes.json"; //створення файлів
        private const string ResultFile = "results.json";
        public QuizManager()
        {
            LoadQuizzes();
            LoadResults();
           
        }

        //перелік вікторин,доступних користувачеві
        public void ListQuizzes()
        {
            if (quizzes.Count == 0)
            {
                Console.WriteLine("Немає доступних вікторин.");
                return;
            }

            Console.WriteLine("Доступні вікторини:");
            for (int i = 0; i < quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {quizzes[i].Topic}");
            }
        }
        //початок будь-якої вікторини
        public void StartQuiz(string topic, User user)
        {
            var quiz = quizzes.FirstOrDefault(q => q.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase));
            if (quiz == null)
            {
                Console.WriteLine("Вікторина не знайдена. Перевірте назву вікторини і повторіть спробу.");
                return;
            }
            int score = 0;
            foreach (var question in quiz.Questions)
            {
                Console.WriteLine(question.Text);
                Console.WriteLine("Варіанти відповідей: " + string.Join(", ", question.Answers));
                var userAnswers = Console.ReadLine().Split(',').Select(ans => ans.Trim()).ToList();
                if (question.IsCorrect(userAnswers))
                {
                    score++;
                    
                }
                AddResult(user.Username, score);
                Console.WriteLine($"Your score: {score}");
            }
        }
        //результатів
        private void AddResult(string username, int score)
        {
            Results.Add(new Result { Username = username, Score = score });
            SaveResults();
            UpdateLeaderBoard();
        }
        private void UpdateLeaderBoard()
        {
            var topResults = Results
                .GroupBy(r => r.Username)
                .Select(g => new Result
                {
                    Username = g.Key,
                    Score = g.Max(r => r.Score)
                })
                .OrderByDescending(r => r.Score)
                .Take(20)
                .ToList();
            var json = JsonConvert.SerializeObject(topResults);
            File.WriteAllText("top_results.json", json);
        }
        //показ результатів
        public void ShowResults(string username)
        {
            Console.WriteLine($"Results for {username}:");
            Console.WriteLine($"Topic: ");
            var userResults = Results.Where(r => r.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            foreach (var result in userResults)
            {
                Console.WriteLine($"Score: {result.Score}");
            }
            if (!userResults.Any())
            {
                Console.WriteLine("No results found.");
            }
        }
        //показати ТОП з результатів
        public void ShowTopResults()
        {
            Console.WriteLine("Top Results:");
            var topResults = Results.OrderByDescending(r => r.Score).GroupBy(r => r.Username);
            foreach (var group in topResults)
            {
                Console.WriteLine($"{group.Key}: {group.Max(r => r.Score)}");
            }
            if (!topResults.Any())
            {
                Console.WriteLine("No results available.");
            }
        }
        //завантаження і збереження вікторин в окремий файл
        private void LoadQuizzes()
        {
            if (File.Exists(QuizFile))
            {
                var json = File.ReadAllText(QuizFile);
                quizzes = JsonConvert.DeserializeObject<List<Quiz>>(json) ?? new List<Quiz>();
            }
            else
            {
                Console.WriteLine("Файл з вікторинами не знайдено.");
            }
        }
        private void SaveQuizzes()
        {
            var json = JsonConvert.SerializeObject(quizzes);
            File.WriteAllText(QuizFile, json);
        }
        //завантаження і збереження результатів
        private void LoadResults()
        {
            if (File.Exists(ResultFile))
            {
                var json = File.ReadAllText(ResultFile);
                Results = JsonConvert.DeserializeObject<List<Result>>(json) ?? new List<Result>();
            }
        }
        private void SaveResults()
        {
            var json = JsonConvert.SerializeObject(Results);
            File.WriteAllText(ResultFile, json);
        }
    }
    public class UserManager : IUserManager //менеджер з користувачів
    {
        private List<User> users = new List<User>();
        private const string UserFile = "users.json";
        public UserManager()
        {
            LoadUsers();
        }
        public bool UserExists(string username)
        {
            return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
        //реестрація
        public User Register(string username, string password, DateTime birthDate)
        {
            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Username already exists."); 
                return null;
            }
            var user = new User { Username = username, Password = password, BirthDate = birthDate };
            users.Add(user);
            SaveUsers();
            return user;
        }
        //логін
        public User Login(string username, string password)
        {
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);
            return user;
        }
        //зміна пароля(за потреби користувача)
        public void ChangePassword(string username, string newPassword)
        {
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                user.Password = newPassword;
                SaveUsers();
                Console.WriteLine("Password changed successfully.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }
        //завантаження та збереження всіх користувачів в одному файлі
        private void LoadUsers()
        {
            if (File.Exists(UserFile))
            {
                var json = File.ReadAllText(UserFile);
                users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
        }
        private void SaveUsers()
        {
            var json = JsonConvert.SerializeObject(users);
            File.WriteAllText(UserFile, json);
        }
    }
    //інтерфейси
    public interface IQuiz
    {
        void StartQuiz(User user);
    }
    public interface IUserManager
    {
        User Register(string username, string password, DateTime birthDate);
        User Login(string username, string password);
        void ChangePassword(string username, string newPassword);
    }
}