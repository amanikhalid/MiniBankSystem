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
            bool inUserMenu = true;

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

            Queue<string> queue = new Queue<string>();// Create a new queue for each request
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
            if (index == -1) return; // Check if account exists
            try // Check if input is valid
            {
                Console.WriteLine("Enter the amount to deposit:");
                double amount = Convert.ToDouble(Console.ReadLine());

                if (amount <= 0) // Check if amount is positive
                {
                    Console.WriteLine("Invalid amount. Please enter a positive number.");
                    return;
                }

                accountBalances[index] += amount; // Update balance
                Console.WriteLine("Deposit successful.");

            }
            catch
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

        // Withdraw Money
        static void WithdrawMoney()
        {
            int index = GetAccountIndex();
            if (index == -1) return; // Check if account exists
            try
            {
                Console.WriteLine("Enter the amount to withdraw:");
                double amount = Convert.ToDouble(Console.ReadLine());
                if (amount <= 0) // Check if amount is positive

                {
                    Console.WriteLine("Invalid amount. Please enter a positive number.");
                    return;
                }
                if (accountBalances[index] - amount < MinimumBalance) // Check if withdrawal exceeds minimum balance
                {
                    Console.WriteLine($"Insufficient balance. Minimum balance of {MinimumBalance} required.");
                    return;
                }
                accountBalances[index] -= amount; // Update balance
                Console.WriteLine("Withdrawal successful.");
            }
            catch // Catch any exceptions

            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
        // View Balance
        static void ViewBalance()
        {
            int index = GetAccountIndex();
            if (index == -1) return; // Check if account exists
            Console.WriteLine($"Account Number: {accountNumbers[index]}"); // Display account number
            Console.WriteLine($"Account Holder: {accountNames[index]}"); // Display account holder name
            Console.WriteLine($"Your current balance is: {accountBalances[index]}");
        }

        // Submit Review
        static void SubmitReview()
        {
            Console.WriteLine("Enter your review/complaint:");
            string review = Console.ReadLine();
            reviewStack.Push(review);
            Console.WriteLine("Your review has been submitted successfully.");
        }

        // Process Next Account Request
        static void ProcessAccountRequests()
        {
            if (createAccountRequests.Count == 0)
            {
                Console.WriteLine("No pending account requests.");
                return;
            }
            ProcessNextAccountRequest();
            SaveAccountsInformationToFile(); // Save account information after processing
        }


        // Process Reviews
        static void ProcessReviews()
        {
            if (reviewStack.Count == 0)
            {
                Console.WriteLine("No reviews to process.");
                return;
            }
            string review = reviewStack.Pop();
            Console.WriteLine($"Processing review: {review}");
            SaveReviewsToFile(); // Save reviews after processing
        }
        // View All Accounts
        static void ViewAllAccounts()
        {
            Console.WriteLine("Account Number\tAccount Holder\tBalance");
            for (int i = 0; i < accountNumbers.Count; i++)
            {
                Console.WriteLine($"{accountNumbers[i]}\t\t{accountNames[i]}\t\t{accountBalances[i]}");
            }
        }
        // View Pending Account Requests
        static void ViewPendingAccountRequests()
        {
            if (createAccountRequests.Count == 0)
            {
                Console.WriteLine("No pending account requests.");
                return;
            }
            Console.WriteLine("Pending Account Requests:");
            foreach (var request in createAccountRequests)
            {
                string[] parts = request.Split('|');
                Console.WriteLine($"Name: {parts[0]}, National ID: {parts[1]}");
            }
        }
        // Get Account Index
        static int GetAccountIndex()
        {
            Console.WriteLine("Enter your account number:");
            try
            {
                int accountNumber = Convert.ToInt32(Console.ReadLine());
                int index = accountNumbers.IndexOf(accountNumber);
                if (index == -1)
                {
                    Console.WriteLine("Account not found.");
                    return -1;
                }
                return index;
            }
            catch
            {
                Console.WriteLine("Invalid input. Please enter a valid account number.");
                return -1;
            }
        }
        // Load Reviews
        static void LoadReviews()
        {
            try // Load reviews from file
            {
                if (File.Exists(ReviewFilePath))
                {
                    using (StreamReader reader = new StreamReader(ReviewFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null) // Read each line
                        {
                            reviewStack.Push(line);
                        }
                    }
                }
                Console.WriteLine("Reviews loaded successfully.");
            }
            catch (Exception ex) // Handle any exceptions
            {
                Console.WriteLine($"Error loading reviews: {ex.Message}");
            }
        }
        // Save Reviews to File
        static void SaveReviewsToFile()
        {
            try // Save reviews to file
            {
                using (StreamWriter writer = new StreamWriter(ReviewFilePath))
                {
                    foreach (var review in reviewStack) // Loop through all reviews
                    {
                        writer.WriteLine(review);
                    }
                }
                Console.WriteLine("Reviews saved successfully.");
            }
            catch (Exception ex) // Handle any exceptions
            {
                Console.WriteLine($"Error saving reviews: {ex.Message}");
            }
        }




        static void SaveAccountsInformationToFile()
        {
            try // Save account information to file
            {
                using (StreamWriter writer = new StreamWriter(AccountsFilePath))
                {
                    for (int i = 0; i < accountNumbers.Count; i++) // Loop through all accounts
                    {
                        string dataLine = $"{accountNumbers[i]}|{accountNames[i]}|{accountBalances[i]}";
                        writer.WriteLine(dataLine);
                    }
                }
                Console.WriteLine("Accounts information saved successfully.");

            }
            catch (Exception ex) // Handle any exceptions
            {
                Console.WriteLine($"Error saving accounts information: {ex.Message}");
            }


        }
        static void LoadAccountsInformationFromFile()
        {
            try // Load account information from file
            {
                if (File.Exists(AccountsFilePath))
                {
                    using (StreamReader reader = new StreamReader(AccountsFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null) // Read each line
                        {
                            string[] parts = line.Split('|');
                            int accountNumber = int.Parse(parts[0]);
                            string name = parts[1];
                            double balance = double.Parse(parts[2]);
                            accountNumbers.Add(accountNumber);
                            accountNames.Add(name);
                            accountBalances.Add(balance);
                            lastAccountNumber = Math.Max(lastAccountNumber, accountNumber); // Update last account number
                        }
                    }
                }
                Console.WriteLine("Accounts information loaded successfully.");
            }
            catch (Exception ex) // Handle any exceptions
            {
                Console.WriteLine($"Error loading accounts information: {ex.Message}");
            }
        }
        
        
       


}   } 
