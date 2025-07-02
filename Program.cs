using System;
using System.Collections.Generic; //for List, Queue, Stack
using System.IO; //for file operations
using System.Security.Cryptography; //for hashing passwords
using System.Text; //for StringBuilder
using System.Linq; //for LINQ operations

namespace MiniBankSystem
{
    internal class Program
    {
        // constants
        const double MinimumBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string ReviewFilePath = "reviews.txt"; // File to store reviews and complaints
        const string AdminID = "admin"; // Admin ID for login
        const string AdminPassword = "11992"; // Admin password for login


        //Global lists (parallel)
        static List<int> accountNumber = new List<int>();
        static List<string> accountNames = new List<string>();
        static List<double> accountBalances = new List<double>();

        //static List<Queue<string>> transactions = new List<Queue<string>>();
        // Queues and Stacks
        //static Queue<(string name, string nationalID)> createAccountRequests = new Queue<(string name, string nationalID)>();
        static Queue<string> createAccountRequests = new Queue<string>(); //format: "Name|NationalID"
        static Stack<string> reviewStack = new Stack<string>();

        static List<string> passwordHashes = new List<string>(); // List to store hashed passwords
        static List<string> nationalIDs = new List<string>(); // List to store National IDs

        static string HashPassword(string password) // Method to hash passwords using SHA256
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Backspace)
                {
                    password.Append(keyInfo.KeyChar);
                    Console.Write("*"); // Display asterisk for each character
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b"); // Remove the last asterisk
                }
            } while (keyInfo.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password.ToString();
        }

        static List<string>> transactions = new List<string>>(); // List to store transaction history
        static List<string> phoneNumbers = new List<string>(); // List to store phone numbers
        static List<string> addresses = new List<string>(); // List to store addresses 
        static List<bool> hasActiveLoan = new List<bool>(); // List to track if the account has an active loan
        static List<double> loanAmounts = new List<double>(); // List to store loan amounts
        static List<double> loanInterestRate = new List<double>(); // List to store loan interest rates
        static List<int> feedbackRatings = new List<int>(); // List to store feedback ratings

        static Queue<int accountIndex, double amount, double interestRate> loanRequests = new Queue<int, double, double>(); // Queue to store loan requests
        static Queue<(int accountIndex, DateTime appointmentDate, string appointmentReason)> appointmentRequests = new Queue<(int, DateTime, string)>(); // Queue to store appointment requests
        static List<bool> hasAppointment = new List<bool>(); // List to track if the account has an appointment
        static List<int> failedLoginAttempts = new List<int>(); // List to track failed login attempts 
        static List<bool> isLocked = new List<bool>(); // List to track if the account is locked
        static List<string> accountNationalIDs = new List<string>(); // List to store National IDs for accounts
        static List<int> accountNumbers = new List<int>(); // List to store account numbers


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
                Console.Write("Select option: ");

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
                        SaveAccountsInformationToFile();
                        SaveReviews();

                        Console.Write("Do you want to create a backup before exit? (Y/N): ");
                        string backupChoice = Console.ReadLine().Trim().ToUpper();
                        if (backupChoice == "Y")
                            BackupAccountsInformationToFile();

                        running = false;
                        break;
                    default: Console.WriteLine("Invalid choice. Please try again"); break;
                }
            }

            Console.WriteLine("Thank you for using the SafeBank System. Goodbye!");
            Console.ReadKey();
        }
    
         
        // ------------------------------------------------------------------

        static string Login()
        {
            Console.Clear();
            Console.WriteLine("Login System");
            Console.WriteLine(" ");

            // Load existing account National IDs from the file
            if (File.Exists(AccountsFilePath))
            {
                var lines = File.ReadAllLines(AccountsFilePath);
                accountNationalIDs = lines.Select(line => line.Split('|')[1]).ToList(); // [1] = NationalID
            }
            else
            {
                Console.WriteLine("No accounts found. Please create an account first.");
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
                return null;
            }

            while (true)
            {
                Console.WriteLine("Enter your National ID:");
                string nationalID = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nationalID))
                {
                    Console.WriteLine("National ID cannot be empty. Please try again.");
                }
                else if (accountNationalIDs.Contains(nationalID))
                {
                    Console.WriteLine("Login successful!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return nationalID;
                }
                else
                {
                    Console.WriteLine("National ID not found. Please try again.");
                }
            }
        }

        // ------------------------------------------------------------------

        static void UserMenu()
        {
            bool inUserMenu = true;
            string loggedInNationalID = "";

            Console.Clear();
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Create Account Request (without login)");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("Select option: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                loggedInNationalID = Login();
            }
            else if (choice == "2")
            {
                CreateAccountRequests();
                return;
            }
            else if (choice == "0")
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                Console.ReadKey();
                return;
            }

            while (inUserMenu)
            {
                Console.Clear();
                Console.WriteLine("\nUser Menu");
                Console.WriteLine("1. Create Account Request");
                Console.WriteLine("2. Deposit Money");
                Console.WriteLine("3. Withdraw Money");
                Console.WriteLine("4. Check/View Balance");
                Console.WriteLine("5. Submit Review/Complaint");
                // Adding the new Features
                Console.WriteLine("6. Generate Monthly Statement"); 
                Console.WriteLine("7. Update My Contact Info");
                Console.WriteLine("8. Request a Loan");
                Console.WriteLine("9. View Transaction History"); 
                Console.WriteLine("10. View Transaction History");
                Console.WriteLine("11. Book Appointment");
                Console.WriteLine("12. LINQ Tools (Search/Sort)");

                Console.WriteLine("0. Return to Main Menu");
                Console.WriteLine("Select option: ");
                string userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1":
                        CreateAccountRequests();
                        break;
                    case "2":
                        DepositMoney(loggedInNationalID); // Pass NationalID
                        break;
                    case "3":
                        WithdrawMoney(loggedInNationalID); // Pass NationalID
                        break;
                    case "4":
                        ViewBalance(loggedInNationalID); // Pass NationalID
                        break;
                    case "5":
                        SubmitReview();
                        break;
                    case "6":
                        GenerateMonthlyStatement();
                        break;
                    case "7":
                        UpdateContactInfo(); // Pass NationalID
                        break;
                    case "8":
                        RequestLoan(); // Pass NationalID
                        break;
                    case "9":
                        ViewTransactions(); // Pass NationalID
                        break;
                    case "10":
                        ViewTransactionHistory();
                        break;
                    case "11":
                        BookAppointment(); // Pass NationalID
                        break;
                    case "12":
                        LINQTools(); // Pass NationalID
                        break;

                    case "0":
                        inUserMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ------------------------------------------------------------------

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
                //Adding the new Features
                Console.WriteLine("5. Process Loan Requests");
                Console.WriteLine("6. View Average User Feedback");
                Console.WriteLine("7. View Appointments");
                Console.WriteLine("8. Unlock Locked Account");

                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select option: ");
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
                    case "5":
                        ProcessLoanRequests();
                        break;
                    case "6":
                        ViewAverageUserFeedback();
                        break;
                    case "7":
                        ViewAppointments();
                        break;
                    case "8":
                        UnlockLockedAccount();
                        break;



                    case "0":
                        inAdminMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        static bool AdminLogin()
        {
            Console.Clear();
            Console.WriteLine("Admin Login");
            Console.WriteLine(" ");
            Console.Write("Enter Admin ID: ");
            string adminID = Console.ReadLine().Trim();
            if (adminID != AdminID)
            {
                Console.WriteLine("Invalid Admin ID. Please try again.");
                return false;
            }
            Console.Write("Enter Admin Password: ");
            string adminPassword = ReadPassword();
            if (adminPassword != AdminPassword)
            {
                Console.WriteLine("Invalid Admin Password. Please try again.");
                return false;
            }
            Console.WriteLine("Admin login successful!");
            return true;
        }

        static void LINQTools()
        {
            Console.Clear();
            Console.WriteLine("LINQ Tools");
            Console.WriteLine("1. Search Accounts by Name");
            Console.WriteLine("2. Sort Accounts by Balance");
            Console.WriteLine("3. Filter Accounts with Balance Above Minimum");
            Console.WriteLine("0. Return to User Menu");
            Console.Write("Select option: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    SearchAccountsByName();
                    break;
                case "2":
                    SortAccountsByBalance();
                    break;
                case "3":
                    FilterAccountsAboveMinimumBalance();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        static void SearchAccountsByName()
        {
            Console.Clear();
            Console.WriteLine("Search Accounts by Name");
            Console.Write("Enter name to search: ");
            string nameToSearch = Console.ReadLine().Trim();
            var results = accountNames
                .Select((name, index) => new { Name = name, Index = index })
                .Where(x => x.Name.IndexOf(nameToSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            if (results.Count > 0)
            {
                Console.WriteLine("Accounts found:");
                foreach (var result in results)
                {
                    Console.WriteLine($"Account Number: {accountNumbers[result.Index]}, Name: {result.Name}, Balance: {accountBalances[result.Index]}");
                }
            }
            else
            {
                Console.WriteLine("No accounts found with that name.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        static void SortAccountsByBalance()
        {
            Console.Clear();
            Console.WriteLine("Sort Accounts by Balance");
            var sortedAccounts = accountBalances
                .Select((balance, index) => new { Balance = balance, Index = index })
                .OrderByDescending(x => x.Balance)
                .ToList();
            Console.WriteLine("Accounts sorted by balance (highest to lowest):");
            foreach (var account in sortedAccounts)
            {
                Console.WriteLine($"Account Number: {accountNumbers[account.Index]}, Name: {accountNames[account.Index]}, Balance: {account.Balance}");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        static void FilterAccountsAboveMinimumBalance()
        {
            Console.Clear();
            Console.WriteLine("Filter Accounts with Balance Above Minimum");
            var filteredAccounts = accountBalances
                .Select((balance, index) => new { Balance = balance, Index = index })
                .Where(x => x.Balance > MinimumBalance)
                .ToList();
            if (filteredAccounts.Count > 0)
            {
                Console.WriteLine($"Accounts with balance above {MinimumBalance}:");
                foreach (var account in filteredAccounts)
                {
                    Console.WriteLine($"Account Number: {accountNumbers[account.Index]}, Name: {accountNames[account.Index]}, Balance: {account.Balance}");
                }
            }
            else
            {
                Console.WriteLine("No accounts found with balance above the minimum.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void UnlockLockedAccount()
        {
            Console.Clear();
            Console.WriteLine("Unlock Locked Account");
            Console.WriteLine(" ");
            try
            {
                Console.WriteLine("Enter the National ID of the account to unlock:");
                string nationalID = Console.ReadLine().Trim();
                int index = accountNationalIDs.IndexOf(nationalID);
                if (index == -1)
                {
                    Console.WriteLine("Account not found.");
                    return;
                }
                if (!isLocked[index])
                {
                    Console.WriteLine("This account is not locked.");
                    return;
                }
                isLocked[index] = false; // Unlock the account
                failedLoginAttempts[index] = 0; // Reset failed login attempts
                Console.WriteLine($"Account with National ID {nationalID} has been unlocked successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        static void ViewAppointments()
        {
            Console.Clear();
            Console.WriteLine("View Appointments");
            Console.WriteLine(" ");
            try
            {
                if (appointmentRequests.Count == 0)
                {
                    Console.WriteLine("No appointments found.");
                    return;
                }
                foreach (var appointment in appointmentRequests)
                {
                    Console.WriteLine($"Account Index: {appointment.accountIndex}, Date: {appointment.appointmentDate}, Reason: {appointment.appointmentReason}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        static void ViewAverageUserFeedback()
        {
            Console.Clear();
            Console.WriteLine("View Average User Feedback");
            Console.WriteLine(" ");
            try
            {
                if (feedbackRatings.Count == 0)
                {
                    Console.WriteLine("No feedback ratings found.");
                    return;
                }
                double averageRating = feedbackRatings.Average();
                Console.WriteLine($"Average User Feedback Rating: {averageRating:F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void SubmitReview()
        {
            Console.Clear();
            Console.WriteLine("Submit Review or Complaint");
            Console.WriteLine(" ");
            try
            {
                Console.WriteLine("Enter your review/complaint:");
                string review = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(review))
                {
                    Console.WriteLine("Review cannot be empty. Please try again.");
                    return;
                }
                reviewStack.Push(review);
                Console.WriteLine("Your review has been submitted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void ShowAverageUserFeedback()
        {
            Console.Clear();
            Console.WriteLine("Average User Feedback");
            Console.WriteLine(" ");
            try
            {
                if (feedbackRatings.Count == 0)
                {
                    Console.WriteLine("No feedback ratings available.");
                    return;
                }
                double averageRating = feedbackRatings.Average();
                Console.WriteLine($"Average User Feedback Rating: {averageRating:F2}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void BackupAccountsInformationToFile()
        {
            Console.Clear();
            Console.WriteLine("Backup Accounts Information to File");
            Console.WriteLine(" ");
            try
            {
                string backupFilePath = "accounts_backup.txt";
                using (StreamWriter writer = new StreamWriter(backupFilePath))
                {
                    for (int i = 0; i < accountNumbers.Count; i++)
                    {
                        string dataLine = $"{accountNumbers[i]}|{accountNationalIDs[i]}|{accountBalances[i]}";
                        writer.WriteLine(dataLine);
                    }
                }
                Console.WriteLine("Accounts information backed up successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error backing up accounts information: {ex.Message}");
            }
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void ViewTransactionHistory()
        {
            Console.Clear();
            Console.WriteLine("View Transaction History");
            Console.WriteLine(" ");
            try
            {
                if (transactions.Count == 0)
                {
                    Console.WriteLine("No transactions found.");
                    return;
                }
                foreach (var transaction in transactions)
                {
                    Console.WriteLine(transaction);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        static void BookAppointment()
        {
            Console.Clear();
            Console.WriteLine("Book Appointment");
            Console.WriteLine(" ");
            try
            {
                Console.WriteLine("Enter the reason for the appointment:");
                string reason = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(reason))
                {
                    Console.WriteLine("Reason cannot be empty. Please try again.");
                    return;
                }
                Console.WriteLine("Enter the date and time for the appointment (yyyy-MM-dd HH:mm):");
                DateTime appointmentDate;
                if (!DateTime.TryParse(Console.ReadLine(), out appointmentDate))
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                    return;
                }
                int accountIndex = GetAccountIndex(); // Get account index from user
                if (accountIndex == -1)
                {
                    Console.WriteLine("Account not found. Please try again.");
                    return;
                }
                appointmentRequests.Enqueue((accountIndex, appointmentDate, reason));
                hasAppointment[accountIndex] = true; // Mark that this account has an appointment
                Console.WriteLine("Appointment booked successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
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
                    bool validInitialBalance = false;
                    string name = "";
                    string nationalID = "";
                    double initialBalance = 0.0;

                    try
                    {
                        while (!validName) // Check if name is valid
                        {
                            Console.WriteLine("Enter your name:");
                            name = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                Console.WriteLine("Name cannot be empty. Please try again.");
                            }
                            else if (int.TryParse(name, out _))
                            {
                                Console.WriteLine("Name cannot be a number. Please try again.");
                            }
                            else if (name.Length < 3)
                            {
                                Console.WriteLine("Name must be at least 3 characters long. Please try again.");
                            }
                            else
                            {
                                validName = true;
                            }
                        }

                        while (!validNationalID) // Get and validate National ID
                        {
                            Console.WriteLine("Enter your National ID: ");
                            nationalID = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(nationalID))
                            {
                                Console.WriteLine("National ID cannot be empty. Please try again.");
                            }
                            else if (!long.TryParse(nationalID, out _))
                            {
                                Console.WriteLine("National ID must be a number. Please try again.");
                            }
                            else if (accountNationalIDs.Contains(nationalID))
                            {
                                Console.WriteLine("An account with this National ID already exists.");
                            }
                            else if (createAccountRequests.Any(r => r.Split('|')[1] == nationalID))
                            {
                                Console.WriteLine("An account request with this National ID is already pending.");
                            }
                            else
                            {
                                validNationalID = true;
                            }
                        }

                        while (!validInitialBalance) // Get and validate Initial Balance
                        {
                            Console.WriteLine("Enter your initial balance: ");
                            string input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input))
                            {
                                Console.WriteLine("Initial balance cannot be empty. Please try again.");
                            }
                            else if (!double.TryParse(input, out initialBalance))
                            {
                                Console.WriteLine("Initial balance must be a number. Please try again.");
                            }
                            else if (initialBalance < MinimumBalance)
                            {
                                Console.WriteLine($"Initial balance must be at least {MinimumBalance}. Please try again.");
                            }
                            else
                            {
                                validInitialBalance = true;
                            }
                        }

                        // After all validations passed
                        string accountRequest = $"{name}|{nationalID}|{initialBalance}";
                        createAccountRequests.Enqueue(accountRequest);

                        // Save to file
                        //File.AppendAllText(AccountsFilePath, $"{name}|{nationalID}|{initialBalance}\n");



                        Console.WriteLine($"Account request for {name} with National ID {nationalID} has been submitted successfully.");
                Console.WriteLine();

            }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    
                    {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }

       


                //Process Next Account Request
                static void ProcessNextAccountRequest()
                {
            {
                if (createAccountRequests.Count == 0)
                {
                    Console.WriteLine("No pending account requests.");
                    return;
                }

                string request = createAccountRequests.Dequeue();
                string[] parts = request.Split('|');
                string name = parts[0];
                string nationalID = parts[1];

                int newAccountNumber = lastAccountNumber + 1;

                Console.Write("Set a password for the new account: ");
                string password = ReadPassword();
                string hash = HashPassword(password);

                Console.Write("Enter your phone number: ");
                string phone = Console.ReadLine();

                Console.Write("Enter your address: ");
                string address = Console.ReadLine();

                accountNumbers.Add(newAccountNumber);
                accountNames.Add(name.Trim()); // Remove any leading/trailing space
                accountBalances.Add(0.0); // Initialize balance to 0.0
                passwordHashes.Add(hash);
                nationalIDs.Add(nationalID);
                transactions.Add(new List<string>()); // Initialize transaction history
                phoneNumbers.Add(phone);
                addresses.Add(address);
                hasAppointment.Add(false);
                hasActiveLoan.Add(false);
                loanAmounts.Add(0);
                loanInterestRate.Add(0);
                failedLoginAttempts.Add(0);
                isLocked.Add(false);

                lastAccountNumber = newAccountNumber;

                Console.WriteLine($"Account created for {name} with Account Number: {newAccountNumber}");
            }

        }

        static int Login()
        {
            Console.Clear();
            Console.WriteLine("Login System");
            Console.WriteLine(" ");
            while (true)
            {
                Console.Write("Enter your National ID: ");
                string nationalID = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(nationalID))
                {
                    Console.WriteLine("National ID cannot be empty. Please try again.");
                    continue;
                }
                int index = accountNationalIDs.IndexOf(nationalID);
                if (index == -1)
                {
                    Console.WriteLine("National ID not found. Please try again.");
                    continue;
                }
                Console.Write("Enter your password: ");
                string password = ReadPassword();
                string hashedPassword = HashPassword(password);
                if (hashedPassword != passwordHashes[index])
                {
                    failedLoginAttempts[index]++;
                    if (failedLoginAttempts[index] >= 3)
                    {
                        isLocked[index] = true;
                        Console.WriteLine("Account is locked due to too many failed login attempts.");
                        return -1; // Account is locked
                    }
                    Console.WriteLine("Invalid password. Please try again.");
                    continue;
                }
                // Reset failed attempts on successful login
                failedLoginAttempts[index] = 0;
                Console.WriteLine($"Login successful! Welcome, {accountNames[index]}.");
                return index; // Return the index of the logged-in account
            }
        }


        // Deposit Money
        static void DepositMoney(string nationalID)
                {
                    Console.Clear();
                    Console.WriteLine("Deposit Money");
                    Console.WriteLine(" ");

                    int index = accountNationalIDs.IndexOf(nationalID); // find index by national ID
                    if (index == -1)
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

                        if (string.IsNullOrWhiteSpace(input) || !double.TryParse(input, out double amount) || amount <= 0)
                        {
                            Console.WriteLine("Invalid amount. Please enter a positive number.");
                            return;
                        }

                        accountBalances[index] += amount;
                        Console.WriteLine($"Deposit successful. New balance : {accountBalances[index]}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }



                // Withdraw Money
                static void WithdrawMoney(string nationalID)
                {
                    Console.Clear();
                    Console.WriteLine("Withdraw Money");
                    Console.WriteLine(" ");

                    int index = accountNationalIDs.IndexOf(nationalID);
                    if (index == -1)
                    {
                        Console.WriteLine("Account not found");
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        return;
                    }

                    try
                    {
                        Console.WriteLine("Enter the amount to withdraw:");
                        string input = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(input) || !double.TryParse(input, out double amount) || amount <= 0)
                        {
                            Console.WriteLine("Invalid amount. Please enter a positive number.");
                            return;
                        }

                        if (accountBalances[index] - amount < MinimumBalance)
                        {
                            Console.WriteLine($"Withdrawal denied. Minimum balance of {MinimumBalance} must be maintained.");
                            return;
                        }

                        accountBalances[index] -= amount;
                        Console.WriteLine($"Withdrawal successful. New balance: {accountBalances[index]}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }

            // View Balance
            static void ViewBalance(string nationalID)
            {
                Console.Clear();
                Console.WriteLine("View Balance");
                Console.WriteLine(" ");

                int index = accountNationalIDs.IndexOf(nationalID);
                if (index == -1)
                {
                    Console.WriteLine("Account not found");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    return;
                }

                try
                {
                    Console.WriteLine($"Account Holder: {accountNames[index]}");
                    Console.WriteLine($"Balance: {accountBalances[index]}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
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
                catch (Exception e)
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
                            string dataLine = $"{accountNumbers[i]}|{accountNationalIDs[i]}|{accountBalances[i]}";
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
                Console.Clear();
                Console.WriteLine("Load Accounts Information from File");
                Console.WriteLine(" ");

                try
                {
                    if (File.Exists(AccountsFilePath))
                    {
                        using (StreamReader reader = new StreamReader(AccountsFilePath))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] parts = line.Split('|');
                                string number = parts[0];
                                string nationalID = parts[1];
                                double balance = double.Parse(parts[2]);

                                accountNumbers.Add(int.Parse(number));
                                accountNationalIDs.Add(nationalID);
                                accountBalances.Add(balance);
                            }
                        }
                        Console.WriteLine("Accounts information loaded successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Accounts file does not exist. No accounts to load.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading accounts information: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        static void generateNumber()
        {
            Random randomNumber = new Random();
            lastAccountNumber = randomNumber.Next(1, 9000); // Generate a random account number
            while (accountNumbers.Contains(lastAccountNumber)) // Ensure the number is unique
            {
                lastAccountNumber = randomNumber.Next(1, 9000);
            }
            accountNumbers.Add(lastAccountNumber); // Add the new account number to the list

        }

    }
    }


