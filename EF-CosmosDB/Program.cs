using EF_CosmosDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EF_CosmosDB
{
    class Program
    {
        static void Main(string[] args)
        {
            using (CosmosContext context = new CosmosContext())
            {
                context.Database.EnsureCreated();
                User currentUser = null;

                MainMenu(currentUser, context);
            }
        }

        private static void MainMenu(User currentUser, CosmosContext context)
        {
            while (currentUser == null)
            {
                Console.Clear();
                Console.WriteLine("Status: Logged Out");
                Console.WriteLine("1. Log in to an existing account.");
                Console.WriteLine("2. Register new account.");
                Console.WriteLine("3. Exit.");
                var userinput = Console.ReadKey();
                switch (userinput.Key)
                {
                    case ConsoleKey.D1:
                        {
                            Console.Clear();
                            currentUser = LoginWindow(currentUser, context);
                            if (currentUser != null)
                            {
                                UserLoggedInWindow(currentUser, context);
                            }
                            break;
                        }
                    case ConsoleKey.D2:
                        {
                            Console.Clear();
                            RegisterWindow(currentUser, context);
                            break;
                        }
                    case ConsoleKey.D3:
                        {
                            Console.Clear();
                            Environment.Exit(0);
                            break;
                        }
                }
            }
        }

        private static void UserLoggedInWindow(User currentUser, CosmosContext context)
        {
            while (currentUser != null)
            {
                Console.Clear();
                Console.WriteLine("Status: Logged In");
                Console.WriteLine("1. Add a task.");
                Console.WriteLine("2. Delete a task.");
                Console.WriteLine("3. Log out.");
                var userinput = Console.ReadKey();
                switch (userinput.Key)
                {
                    case ConsoleKey.D1:
                        {
                            Console.Clear();
                            Console.WriteLine("Please write a note, then hit enter to continue.");
                            string noteInput = Console.ReadLine().Trim();
                            EnterToContinue();
                            MakeSureInputValid(ref noteInput);
                            AddTaskToUser(noteInput, currentUser, context);
                            break;
                        }
                    case ConsoleKey.D2:
                        {
                            Console.Clear();
                            DeleteTaskFromUser(currentUser, context);
                            break;
                        }
                    case ConsoleKey.D3:
                        {
                            Console.Clear();
                            currentUser = null;
                            MainMenu(currentUser, context);
                            break;
                        }
                }
            }
        }
        private static User LoginWindow(User currentUser, CosmosContext context)
        {
            Console.WriteLine("Type in your username and hit enter to continue\n" +
                "");
            Console.WriteLine("Username: ");

            string usernameInput = Console.ReadLine().Trim();

            var findUsername = from user in context.Users
                               where user.Username == usernameInput
                               select user;

            if (findUsername.ToList().Any())
            {
                currentUser = findUsername.First();
                Console.WriteLine($"Found user {currentUser.Username}");
                Console.WriteLine("Enter password");

                var numberOfTries = 3;
                for (int i = 1; i < numberOfTries + 1; i++)
                {
                    string passwordInput = Console.ReadLine().Trim();
                    if (passwordInput == currentUser.Password)
                    {
                        Console.Clear();
                        Console.WriteLine("Password correct, you are logged in.");
                        EnterToContinue();
                        return currentUser;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Wrong password.");
                        Console.WriteLine($"Number of tries left {numberOfTries - i}");
                    }
                }
                return null;
            }
            else
            {
                Console.WriteLine("Didnt find a registered account with that username.");
                EnterToContinue();
                return null;
            }
        }
        private static void RegisterWindow(User currentUser, CosmosContext context)
        {
            Console.WriteLine("Registerwindow");
            Console.WriteLine("Username: ");

            string usernameInput = Console.ReadLine().Trim();
            MakeSureInputValid(ref usernameInput);

            var findUser = from user in context.Users
                           where user.Username == usernameInput
                           select user;
            if (findUser.ToList().Any())
            {
                Console.WriteLine("There's already a registered account with this username.");
                EnterToContinue();
            }
            else
            {
                Console.WriteLine("The username is available.");
                Console.WriteLine("Enter a password");

                string passwordInput = Console.ReadLine().Trim();
                MakeSureInputValid(ref passwordInput);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = usernameInput,
                    Password = passwordInput
                };
                context.Users.Add(user);
                context.SaveChanges();
                Console.WriteLine("Account registered, you can now log in.");
                EnterToContinue();
            }
        }
        private static void AddTaskToUser(string noteInput, User currentUser, CosmosContext context)
        {
            Task task = new Task
            {
                Id = Guid.NewGuid().ToString(),
                User = currentUser,
                DateOfEntry = DateTime.Now,
                Note = noteInput,
            };
            if (currentUser.ToDoList == null)
            {
                currentUser.ToDoList = new List<Task> { task };
            }
            else
            {
                currentUser.ToDoList.Add(task);
            }
            context.SaveChanges();
            Console.WriteLine("Your task has been added.");
            EnterToContinue();
        }
        private static void DeleteTaskFromUser(User currentUser, CosmosContext context)
        {
            bool keepLooping = true;
            while (keepLooping)
            {
                if (currentUser.ToDoList.Any())
                {
                    Console.Clear();
                    Console.WriteLine("Select the task you want to delete.");
                    var notes = new List<string>();
                    foreach (var task in currentUser.ToDoList)
                    {
                        Console.WriteLine($"[{task.Id.Remove(3)}] {task.Note}");
                        notes.Add(task.Id.Remove(3));
                    }
                    Console.WriteLine();
                    Console.WriteLine("[Exit] Type 'exit' to go back.");
                    string taskIDToDelete = Console.ReadLine().Trim();
                    if (notes.Contains(taskIDToDelete))
                    {
                        Console.WriteLine($"Deleting [{taskIDToDelete}] from your todolist.");
                        foreach (var task in currentUser.ToDoList)
                        {
                            if (task.Id.Remove(3).Equals(taskIDToDelete))
                            {
                                currentUser.ToDoList.Remove(task);
                                context.Tasks.Remove(task);
                                context.SaveChanges();
                                Console.WriteLine("Success");
                                break;
                            }
                        }
                        EnterToContinue();
                    }
                    else if (taskIDToDelete == "exit" || taskIDToDelete == "Exit")
                    {
                        keepLooping = false;
                    }
                    else
                    {
                        Console.WriteLine($"Can't find ID: {taskIDToDelete}");
                        EnterToContinue();
                    }
                }
                else
                {
                    Console.WriteLine("You have 0 tasks in your todolist.");
                    EnterToContinue();
                    keepLooping = false;
                }
            }
        }
        private static string MakeSureInputValid(ref string stringInput)
        {
            while (stringInput == null || stringInput.Trim().Length < 5)
            {
                Console.WriteLine("You have to write something, and more than 5 characters.");
                try
                {
                    stringInput = Console.ReadLine().Trim();
                    EnterToContinue();

                    if (stringInput == null || stringInput.Trim().Length < 5)
                    {
                        throw new Exception("Invalid input.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("Success");
            return stringInput;
        }

        private static void EnterToContinue()
        {
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    {
                        break;
                    }
            }
        }
    }
}
