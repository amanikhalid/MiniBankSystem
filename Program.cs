namespace MiniBankSystem
{
    internal class Program
    {
        //constants
        const double MinimumBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string ReviewFilePath = "reviews.txt";

        //Global lists (parallel)
        static List<int> accountNumbers = new List<int>();
        static List<string> accountNames = new List<string>();
        static List<double> accountBalances = new List<double>();

        //static List<Queue<string>> transactions = new List<Queue<string>>();
        // Queues and Stacks
        //static Queue<(string name, string nationalID)> createAccountRequests = new Queue<(string name, string nationalID)>();
        static Queue<string> createAccountRequests = new Queue<string>(); //format: "Name|NationalID"
        static Stack<string> reviewStack = new Stack<string>();

        // Account number generator
        static int lastAccountNumber;
        static void Main()
        {
            LoadAccountsInformationFromFile();
            LoadReviews();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("\nWelcome to the SafeBank System");
                Console.WriteLine("1. User Menu");
                Console.WriteLine("2. Admin Menu");
                Console.WriteLine("0. Exit");
                Console.WriteLine("Select option: ");

                string mainChoice = Console.ReadLine();

                switch (mainChoice)
                {
                    case "1":
                        UserMenu();
                        break;
                    case "2":
                        AdminMenu();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

            Console.WriteLine("Thank you for using the SafeBank System. Goodbye!");
  
        }

        // User Menu:
        static void UserMenu()
        {
            bool inUserMenu= true;

            while (inUserMenu)
            {
                Console.Clear();
                Console.WriteLine("\nUser Menu");
                Console.WriteLine("1. Create Account Request");
                Console.WriteLine("2. Deposit Money");
                Console.WriteLine("3. Withdraw Money");
                Console.WriteLine("4. Check/View Balance");
                Console.WriteLine("5. Submit Review/Complaint");
                Console.WriteLine("0. Return to Main Menu");
                Console.WriteLine("Select option: ");
                string userChoice = Console.ReadLine();
                switch (userChoice)
                {
                    case "1":
                        CreateAccountRequests();
                        break;
                    case "2":
                        DepositMoney();
                        break;
                    case "3":
                        WithdrawMoney();
                        break;
                    case "4":
                        ViewBalance();
                        break;
                    case "5":
                        SubmitReview();
                        break;
                    case "0":
                        inUserMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
 
        }

        // Admin Menu:
        static void AdminMenu()
        {
            bool inAdminMenu = true;
            while (inAdminMenu)
            {
                Console.Clear();
                Console.WriteLine("\nAdmin Menu");
                Console.WriteLine("1. Process Account Requests");
                Console.WriteLine("2. View Submitted Reviews");
                Console.WriteLine("3. View All Accounts");
                Console.WriteLine("4. View Pending Account Requests");
                Console.WriteLine("0. Return to Main Menu");
                Console.WriteLine("Select option: ");
                string adminChoice = Console.ReadLine();
                switch (adminChoice)
                {
                    case "1":
                        ProcessAccountRequests();
                        break;
                    case "2":
                        ProcessReviews();
                        break;
                    case "3":
                        ViewAllAccounts();
                        break;
                    case "4":
                        ViewPendingAccountRequests();
                        break;
                    case "0":
                        inAdminMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

        }

        // Create Account Request
        static void CreateAccountRequests()
        {
            Console.WriteLine("Enter your Full Name:");
            string name = Console.ReadLine();

            Console.WriteLine("Enter your valid National ID");
            string nationalID = Console.ReadLine();

            string accountRequest = name + "|" + nationalID;

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(accountRequest);

            //CreateAccountRequests.Enqueue((name, nationalID));
            createAccountRequests.Enqueue(accountRequest);

            Console.WriteLine("Your account request has been submitted successfully");
        }

        //Process Next Account Request
        static void ProcessNextAccountRequest()
        {
            if (createAccountRequests.Count == 0)
            {
                Console.WriteLine("No Pending account request");
                return;
            }
            //var (name, nationalID) = createAccountRequests.Dequeue();
            string accountRequest = createAccountRequests.Dequeue();
            string[] parts = accountRequest.Split('|');
            string name = parts[0];
            string nationalID = parts[1];

            int newAccountNumber = lastAccountNumber +1;

            accountNumbers.Add(newAccountNumber);
            accountNames.Add(name);
            accountBalances.Add(MinimumBalance);
            lastAccountNumber = newAccountNumber;
            Console.WriteLine($"Account is successfully created for {name} with account number: {newAccountNumber}");
        }

        // Deposit Money
        static void DepositMoney()
        {
            int index = GetAccountIndex();
            if (index == -1) return;
            try
            {
                Console.WriteLine("Enter the amount to deposit:");
                double amount = Convert.ToDouble(Console.ReadLine());

                if (amount <= 0)
                {
                    Console.WriteLine("Invalid amount. Please enter a positive number.");
                    return;
                }

                accountBalances[index] += amount;
                Console.WriteLine("Deposit successful.");

            }
            catch
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }
}
