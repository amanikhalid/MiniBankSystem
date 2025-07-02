using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MiniBankSystem
{
    internal class Program
    {
        // constants
        const double MinimumBalance = 100.0;
        const double USD_TO_OMR = 3.8;
        const double EUR_TO_OMR = 4.1;
        const string AccountsFilePath = @"C:\Users\CodeLine\source\repos\MiniBankSystem\accounts.txt";
        const string ReviewFilePath = @"C:\Users\CodeLine\source\repos\MiniBankSystem\reviews.txt";
        const string TransactionsFilePath = "transactions.txt";
        const string AdminID = "admin";
        const string AdminPassword = "Bank1122";


        //Global lists (parallel) - Data storage 
        static List<int> accountNumber = new List<int>();
        static List<string> accountNames = new List<string>();
        static List<double> accountBalances = new List<double>();
        static List<string> accountNationalIDs = new List<string>();
        static List<string> accountPasswords = new List<string>(); // New feature for passwords
        static List<bool> accountIsLocked = new List<bool>();      // New feature for account lock status
        static List<int> accountNumbers = new List<int>();

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
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine("Thank you for using the SafeBank System. Goodbye!");
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

                Console.Write("Enter password: ");
                string passwordInput = ReadPasswordWithMask();
                string hashedInput = HashPassword(passwordInput);

                if (accountPasswords[index] == hashedInput)
                {
                    // Success
                }
                else
                {
                    // Track failed attempts and possibly lock the account
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

                    if (parts.Length != 3) // check if the request is valid
                    {
                        Console.WriteLine("Invalid account request format.");
                        return;
                    }

                    string name = parts[0]; // get the name
                    string nationalID = parts[1]; // get the national ID
                    double balance = double.Parse(parts[2]);
                    // validate name and ID
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(nationalID))
                    {
                        Console.WriteLine("Invalid account request. Name and National ID cannot be empty.");
                        return;
                    }

                    int newAccountNumber = lastAccountNumber + 1; // generate new account number
                    accountNumbers.Add(newAccountNumber); // add account number to list
                    accountNames.Add(name); // add name to list
                    accountNationalIDs.Add(nationalID);
                    accountBalances.Add(balance); // add initial balance to list
                    accountNumbers.Add(newAccountNumber); // add account number to list
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

        static string ReadPasswordWithMask()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Backspace)
                {
                    password.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b"); // Remove the last asterisk
                }
            } while (keyInfo.Key != ConsoleKey.Enter);
            Console.WriteLine(); // Move to the next line after Enter is pressed
            return password.ToString();
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }

        }
    }
}


