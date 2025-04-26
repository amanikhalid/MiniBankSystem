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

            bool running = true; // Main menu loop
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
            bool inUserMenu = true; // User menu loop

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
            Console.Clear();
            Console.WriteLine("Create Account Request");
            Console.WriteLine(" ");

            bool validName = false;
            bool validNationalID = false;
            bool validinitialBalance = false;
            string name = "";
            string nationalID = "";
            double initialBalance =0.0 ;

            try
            {
                while (!validName) // check if name is valid
                {
                    Console.WriteLine("Enter your name:");
                    name = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(name)) // check if name is empty
                    {
                        Console.WriteLine("Name cannot be empty. Please try again.");
                        validName = false;
                    }
                    else if (int.TryParse(name, out int result)) // check if name is number
                    {
                        Console.WriteLine("Name cannot be a number. Please try again.");
                        validName = false;
                    }
                    else if (name.Length < 3) // check if name is less than three characters
                    {
                        Console.WriteLine("Name must be at least 3 characters long. Please try again.");
                        validName = false;
                    }
                    else
                    {
                        validName = true;
                    }

                    // Get National ID and Validate it
                    while (!validNationalID)
                    {
                        Console.WriteLine("Enter your National ID (must be 5 to 6 characters): ");
                        nationalID = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(nationalID)) // check if national ID is empty
                        {
                            Console.WriteLine("National ID cannot be empty. Please try again.");
                            validNationalID = false;
                        }
                        else if (nationalID.Length < 7 || nationalID.Length > 4) // check if national ID is not 6 characters
                        {
                            Console.WriteLine("National ID must be 5 to 6 characters long. Please try again.");
                            validNationalID = false;
                        }
                        else if (!long.TryParse(nationalID, out long result)) // check if national ID is number
                        {
                            Console.WriteLine("National ID must be a number. Please try again.");
                            validNationalID = false;
                        }
                        else
                        {
                            validNationalID = true;
                        }
                    }
                    // Get Initial Balance and Validate it
                    while (!validinitialBalance)
                    {
                        Console.WriteLine("Enter your initial balance: ");
                        string input = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(input)) // check if initial balance is empty
                        {
                            Console.WriteLine("Initial balance cannot be empty. Please try again.");
                            validinitialBalance = false;
                        }
                        else if (!double.TryParse(input, out initialBalance)) // check if initial balance is number
                        {
                            Console.WriteLine("Initial balance must be a number. Please try again.");
                            validinitialBalance = false;
                        }
                        else if (initialBalance < MinimumBalance) // check if initial balance is less than minimum balance
                        {
                            Console.WriteLine($"Initial balance must be at least {MinimumBalance}. Please try again.");
                            validinitialBalance = false;
                        }
                        else
                        {
                            validinitialBalance = true;
                        }
                    }
                    string accountRequest = $"{name}|{nationalID}"; // format: "Name|NationalID"
                    createAccountRequests.Enqueue(accountRequest); // add request to queue
                    Console.WriteLine($"Account request for {name} with National ID {nationalID} has been submitted successfully.");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();



                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }

        //Process Next Account Request
        static void ProcessNextAccountRequest()
        {
            Console.Clear();
            Console.WriteLine("Processing Next Account Request");
            Console.WriteLine("");

            try
            {
                if (createAccountRequests.Count == 0) // check if there are any requests
                {
                    Console.WriteLine("No pending account requests.");
                    return;
                }
                else
                {
                    string accountRequest = createAccountRequests.Dequeue(); // get the next request
                    string[] parts = accountRequest.Split('|'); // split the request into name and national ID

                    if (parts.Length != 2) // check if the request is valid
                    {
                        Console.WriteLine("Invalid account request format.");
                        return;
                    }

                    string name = parts[0]; // get the name
                    string nationalID = parts[1]; // get the national ID

                    // validate name and ID
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(nationalID))
                    {
                        Console.WriteLine("Invalid account request. Name and National ID cannot be empty.");
                        return;
                    }

                    int newAccountNumber = lastAccountNumber + 1; // generate new account number
                    accountNumbers.Add(newAccountNumber); // add account number to list
                    accountNames.Add(name); // add name to list
                    accountBalances.Add(0.0); // add initial balance to list
                    lastAccountNumber = newAccountNumber; // update last account number

                    Console.WriteLine($"Account created successfully for {name} with Account Number: {newAccountNumber}");
                    Console.WriteLine($"National ID: {nationalID}");
                    Console.WriteLine($"Your initial balance is: {accountBalances[accountBalances.Count - 1]}");

                }

            }
            catch (Exception e)
            { Console.WriteLine($"Error: {e.Message}"); }

            {
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }


        // Deposit Money
        static void DepositMoney()
        {
            Console.Clear();
            Console.WriteLine("Deposit Money");
            Console.WriteLine(" ");

            int index = GetAccountIndex();
            if (index == -1) // Check if account exists

            {
                Console.WriteLine("Account not found");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return;

            }

            try
            {
                Console.WriteLine("Enter the amount to deposit:");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input)) // Check if input is empty
                {
                    Console.WriteLine("Deposit amount cannot be empty. Please try again.");
                    return;
                }

                if (!double.TryParse(input, out double amount)) // Check if input is a number
                {
                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                    return;
                }
                if (amount <= 0) // Check if amount is positive
                {
                    Console.WriteLine("Invalid amount. Please enter a positive number.");
                    return;
                }
                accountBalances[index] += amount; // Update balance
                Console.WriteLine($"Deposit successful. New balance : {accountBalances[index]}");
            }
            catch (Exception e) { Console.WriteLine($"Error: {e.Message}"); }
            {
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }


            // Withdraw Money
            static void WithdrawMoney()
        {
            Console.Clear();
            Console.WriteLine("Withdraw Money");
            Console.WriteLine(" ");

            int index = GetAccountIndex();
            if (index == -1) // Check if account exists
            {
                Console.WriteLine("Account not found");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return;
            }
            try { Console.WriteLine("Enter the amount to withdraw:");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input)) // Check if input is empty
                {
                    Console.WriteLine("Withdraw amount cannot be empty. Please try again.");
                    return;
                }

                if (!double.TryParse(input, out double amount)) // Check if input is a number
                {
                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                    return;
                }

                if (amount <= 0) // Check if amount is positive
                {
                    Console.WriteLine("Invalid amount. Please enter a positive number.");
                    return;
                }

                if (accountBalances[index] - amount < MinimumBalance) // Check if balance after withdrawal is above minimum
                {
                    Console.WriteLine($"Withdrawal denied. Minimum balance of {MinimumBalance} must be maintained.");
                    return;
                }

                accountBalances[index] -= amount; // Update balance
                Console.WriteLine($"Withdrawal successful. New balance: {accountBalances[index]}");
            }

            catch (Exception e)
            { Console.WriteLine($"Error: {e.Message}"); }

            {
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }
            // View Balance
            static void ViewBalance()
            {
            Console.Clear();
            Console.WriteLine("View Balance");
            Console.WriteLine(" ");

            int index = GetAccountIndex();
            if (index == -1) // Check if account exists
            {
                Console.WriteLine("Account not found");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return;
            }
            try
            {
                Console.WriteLine($"Account Number: {accountNumbers[index]}");
                Console.WriteLine($"Account Holder: {accountNames[index]}");
                Console.WriteLine($"Balance: {accountBalances[index]}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}"); 
            }

            {
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }

            // Submit Review
            static void SubmitReview()
            {
            Console.Clear();
            Console.WriteLine("Submit Review or Complaint");
            Console.WriteLine(" ");

            try
            {
                Console.WriteLine("Enter your review/complaint");
                string review = Console.ReadLine().Trim();

                if (string.IsNullOrWhiteSpace(review))
                {
                    Console.WriteLine("Review cannot be empty. Please try again");
                    return;
                }
                reviewStack.Push(review);
                Console.WriteLine("Your review has been submitted successfully");

            }

            catch (Exception e)
            { 
                Console.WriteLine(e.ToString()); return;
            }

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            }

            // Process Next Account Request
            static void ProcessAccountRequests()
            {
            Console.Clear();
            Console.WriteLine("Processing Account Requests");
            Console.WriteLine(" ");

            try
            {
                if (createAccountRequests.Count == 0) // Check if there are any requests
                {
                    Console.WriteLine("No pending account requests.");
                    return;
                }
                else
                {
                    int approvedCount = 0;
                    int rejectedCount = 0;

                    while (createAccountRequests.Count > 0) // Process all requests
                    {
                        string accountRequest = createAccountRequests.Dequeue(); // Get the next request
                        string[] parts = accountRequest.Split('|'); // Split the request into name and national ID
                        if (parts.Length != 2) // Check if the request is valid
                        {
                            Console.WriteLine("Invalid account request format.");
                            continue;
                        }

                        string name = parts[0]; // Get the name
                        string nationalID = parts[1]; // Get the national ID

                        Console.WriteLine($" Request:");
                        Console.WriteLine($"Name: {name}");
                        Console.WriteLine($"National ID: {nationalID}");
                        Console.WriteLine("Approve or Reject this request? (Y/N): ");

                        string choice = Console.ReadLine().Trim().ToUpper();

                        if (choice == "Y")
                        {
                            int newAccountNumber = lastAccountNumber + 1; // Generate new account number
                            accountNumbers.Add(newAccountNumber); // Add account number to list
                            accountNames.Add(name); // Add name to list
                            accountBalances.Add(0.0); // Add initial balance to list
                            lastAccountNumber = newAccountNumber; // Update last account number
                            Console.WriteLine($"Account created successfully for {name} with Account Number: {newAccountNumber}");
                            approvedCount++;
                        }
                        else if (choice == "N")
                        {
                            Console.WriteLine($"Request for {name} has been rejected.");
                            rejectedCount++;
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Request not processed.");
                        }

                        Console.WriteLine(" ");
                    }

                    SaveAccountsInformationToFile(); // Save account information after processing requests
                    Console.WriteLine($"Approved Requests: {approvedCount}");
                    Console.WriteLine($"Rejected Requests: {rejectedCount}");
                    Console.WriteLine("Account requests processed successfully.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }


            // Process Reviews
            static void ProcessReviews()
            {
            Console.Clear();
            Console.WriteLine("Process Submitted Reviews");
            Console.WriteLine(" ");

            try
            {
                if (reviewStack.Count == 0) // Check if there are any reviews
                {
                    Console.WriteLine("No submitted reviews.");
                    return;
                }
                else
                {
                    int processdCount = 0; 
                    int skippedCount = 0;

                    while (reviewStack.Count > 0) 
                    {
                        string review = reviewStack.Pop(); 

                        Console.WriteLine($" Review: {review}");
                        Console.WriteLine("Mark this review as processed? (Y/N)");
                        string choice = Console.ReadLine().Trim().ToUpper();
                        if (choice == "Y") 
                        {
                            Console.WriteLine("Review marked as processed");
                            processdCount++;
                        }
                        else if (choice == "N")
                        {
                            Console.WriteLine("Review skipped");
                            skippedCount++;
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Request not processed.");
                        }

                        Console.WriteLine();
                    }
                    SaveReviewsToFile();

                    Console.WriteLine("Summary:");
                    Console.WriteLine($"Processed:{processdCount} ");
                    Console.WriteLine($"Skipped: {skippedCount}");
                }
                        
            }
            catch (Exception e ) 
            {
                Console.WriteLine($"An error occurred while processing reviews: {e.Message}");

            }
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        // View All Accounts
        static void ViewAllAccounts()
        {
            Console.Clear();
            Console.WriteLine("All Bank Accounts");
            Console.WriteLine(" ");

            try
            {
                if (accountNumbers.Count == 0)
                {
                    Console.WriteLine("No accounts available to display.");
                }
                else
                {
                    Console.WriteLine("Account Number\tAccount Holder\tBalance");
                    Console.WriteLine("  ");

                    for (int i = 0; i < accountNumbers.Count; i++)
                    {
                        Console.WriteLine($"{accountNumbers[i]}\t\t{accountNames[i]}\t\t{accountBalances[i]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            {
                Console.WriteLine(" ");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        // View Pending Account Requests
        static void ViewPendingAccountRequests()
        {
            Console.Clear();
            Console.WriteLine("View Pending Account Requests");
            Console.WriteLine(" ");

            try
            {
                if (createAccountRequests.Count == 0)
                {
                    Console.WriteLine("No pending account requests.");
                }
                else
                {
                    Console.WriteLine("Pending Account Requests:");
                    Console.WriteLine(" ");

                    foreach (var request in createAccountRequests)
                    {
                        string[] parts = request.Split('|');
                        Console.WriteLine($"Name: {parts[0]}, National ID: {parts[1]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            {
                Console.WriteLine(" ");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        // Get Account Index
        static int GetAccountIndex()
        {
            Console.Clear();
            Console.WriteLine("Find Account by Account Number");
            Console.WriteLine(" ");

            bool validAccountNumber = false;
            int accountNumber = 0;
            int index = -1;

            try
            {
                while (!validAccountNumber)
                {
                    Console.WriteLine("Enter your account number:");
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Account number cannot be empty. Please try again.");
                        validAccountNumber = false;
                    }
                    else if (!int.TryParse(input, out accountNumber))
                    {
                        Console.WriteLine("Invalid format. Please enter numbers only.");
                        validAccountNumber = false;
                    }
                    else
                    {
                        index = accountNumbers.IndexOf(accountNumber);
                        if (index == -1)
                        {
                            Console.WriteLine("Account not found. Please try again.");
                            validAccountNumber = false;
                        }
                        else
                        {
                            validAccountNumber = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            

            {
                Console.WriteLine(" ");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            return index;
        }

        // Load Reviews
        static void LoadReviews()
        {
            Console.Clear();
            Console.WriteLine("Load Reviews");
            Console.WriteLine(" ");

            try
            {
                if (File.Exists(ReviewFilePath))
                {
                    using (StreamReader reader = new StreamReader(ReviewFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            reviewStack.Push(line);
                        }
                    }
                    Console.WriteLine("Reviews loaded successfully.");
                }
                else
                {
                    Console.WriteLine("Review file does not exist. No reviews to load.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading reviews: {ex.Message}");
            }
            

            {
                Console.WriteLine(" ");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        // Save Reviews to File
        static void SaveReviewsToFile()
        {
            Console.Clear();
            Console.WriteLine("Save Reviews to File");
            Console.WriteLine(" ");

            try
            {
                using (StreamWriter writer = new StreamWriter(ReviewFilePath))
                {
                    foreach (var review in reviewStack)
                    {
                        writer.WriteLine(review);
                    }
                }
                Console.WriteLine("Reviews saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving reviews: {ex.Message}");
            }
            
            {
                Console.WriteLine(" ");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }


        static void SaveAccountsInformationToFile()
        {
            Console.Clear();
            Console.WriteLine("Save Accounts Information to File");
            Console.WriteLine(" ");

            try
            {
                using (StreamWriter writer = new StreamWriter(AccountsFilePath))
                {
                    for (int i = 0; i < accountNumbers.Count; i++)
                    {
                        string dataLine = $"{accountNumbers[i]}|{accountNames[i]}|{accountBalances[i]}";
                        writer.WriteLine(dataLine);
                    }
                }
                Console.WriteLine("Accounts information saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving accounts information: {ex.Message}");
            }
            
            {
                Console.WriteLine(" ");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
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





    }
}

