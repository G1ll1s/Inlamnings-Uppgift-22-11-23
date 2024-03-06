using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net;

namespace AdoCRUD
{
            
    internal class Program
    {
        public static string Customer { get; set; }
                

        static void Main(string[] args)
        {
            //Oscar Gillberg
            //BUV23

            
            Console.Clear(); Console.WriteLine("\x1b[3J");
            AnsiConsole.Clear();
            bool end = false;
            while (!end)
            {

                var menu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose through the [green]Menu[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[#875faf](Move up and down to reveal more fruits)[/]")
                    .AddChoices(new[] {
                        "Add Customer",
                        "Delete Customer",
                        "Update Customer",
                        "Show Country Sales",
                        "Order",
                        "Show Customer list",
                        "Show Order list",
                        "Report Generator",
                        "End Program"
                    })
                    );

                switch (menu)
                {

                    case "Add Customer": Customer = AddCustomerUI(); break;
                    case "Delete Customer": DeleteCustomerMenu(); break;
                    case "Update Customer": UpdateCustomer(); break;
                    case "Show Country Sales": ShowCountrySales(); break;
                    case "Order": Order(); break;
                    case "Show Customer list": CustomerList(); break;
                    case "Show Order list": OrderList(); break;
                    case "Report Generator": ReportGenerator(); break;  
                    case "End Program": end = true; break;
                }

            }
        }
            static string AddCustomerUI() 
            {
                
                try
                {
                    // Här lägger man till värde som senare kommer hämtas 
                    Console.Write("Add company name: ");
                    string inputCompanyName = Console.ReadLine();
                    Console.Write("Add company ID, 5 letters: ");
                    Customer = Console.ReadLine();
                    Console.Write("Add contact name: ");
                    string inputContactName = Console.ReadLine();
                    Console.Write("Add Address: ");
                    string inputAddress = Console.ReadLine();
                    Console.Write("Add City: ");
                    string inputCity = Console.ReadLine();
                    Console.Write("Add Country: ");
                    string InputCountry = Console.ReadLine();

                    Customer = AddCustomer(inputCompanyName, inputContactName, inputAddress, inputCity, InputCountry);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"{ex.Message}");

                }
                return Customer;
            
            }

            static string AddCustomer(string inputCompanyName, string inputContactName, string inputAddress, string inputCity, string inputCountry) 
            {
                
                
                using (SqlConnection GatesOfHell = ConnectionString())//Gör en connection från min Connection Metod 
                {
                    try // 
                    {
                        string query = "insert into Customers(CustomerID ,CompanyName, ContactName, Address, City, Country) " +
                                        "Values (@CustomerID ,@CompanyName, @ContactName, @Address, @City, @Country)";
                        
                        GatesOfHell.Open();// öppnar upp

                        using (SqlCommand cmd = new SqlCommand("spInsertCustomer", GatesOfHell))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@CompanyName", inputCompanyName);
                            cmd.Parameters.AddWithValue("@CustomerID", Customer);
                            cmd.Parameters.AddWithValue("@ContactName", inputContactName);
                            cmd.Parameters.AddWithValue("@Address", inputAddress);
                            cmd.Parameters.AddWithValue("@City", inputCity);
                            cmd.Parameters.AddWithValue("@Country", inputCountry);

                            bool rowsAffected = cmd.ExecuteNonQuery() > 0;
                            if (rowsAffected)
                            {
                               Console.WriteLine("Customer added succesfully!");                                
                            }
                            else
                            {
                                Console.WriteLine("Faild to add customer!");
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                }
                Console.ReadKey();
                Console.Clear();

                return (Customer);
            }

            static void DeleteCustomerMenu() 
            {
                
                bool end = false;
                while (!end)
                {
                    var menu2 = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose which you want to [White]Delete[/]")
                        .PageSize(4)
                        .AddChoices(new[] {
                        "Delete by Customer ID",
                        "Delete by Company Name",
                        "Show Customer list",
                        "Back to main menu"
                        })
                        );

                    switch (menu2)
                    {
                        case "Delete by Customer ID": DelCustomerID(); break;
                        case "Delete by Company Name": DelCompanyName(); break;
                        case "Show Customer list":CustomerList(); break;    
                        case "Back to main menu": end = true; break;

                    }
                }
            }

            /// <summary>
            /// hej hop
            /// </summary>
            static void DelCustomerID()
            {
                Console.Write("Enter CustomerID to delete: ");
                string delInput = Console.ReadLine();
                              
                    using (SqlConnection GatesOfHell = ConnectionString())
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand("spDelCustomerIDandOrder", GatesOfHell);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;                   
                           
                            cmd.Parameters.AddWithValue("@CustomerID", delInput);
                                   

                            GatesOfHell.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"{delInput} was removed!");

                        }
                        catch (Exception ex) 
                        {
                            Console.WriteLine($"an error has occurred {ex.Message}");
                        }

                    }
               
                Console.ReadKey();
                Console.Clear();
            }

            static void DelCompanyName()
            {
                Console.Write("Enter Company Name to delete: ");
                string delInput = Console.ReadLine();


                using (SqlConnection GateOfHell = ConnectionString())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("spDelCompanyName", GateOfHell);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                          
                        cmd.Parameters.AddWithValue("@CompanyName", delInput);
                        GateOfHell.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        Console.WriteLine($"{delInput} was removed!");
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine($"An error occurred {ex.Message}");
                    }      
                }
                Console.ReadKey();
                Console.Clear();
            }

            static void UpdateCustomer() 
            {


                Console.Write("Search for CustomerID: ");
                string customerID = Console.ReadLine();
                Console.Write("Change address: ");
                string address = Console.ReadLine();
                Console.Write("Change City: ");
                string city = Console.ReadLine();
                Console.Write("Change Country: ");
                string country = Console.ReadLine();

                using (SqlConnection GatesOfHell = ConnectionString()) 
                {
                    try 
                    {
                        GatesOfHell.Open();
                        using (SqlCommand cmd = new SqlCommand("spUpdateCustomer",GatesOfHell))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                                        
                            cmd.Parameters.AddWithValue("@Address", address);
                            cmd.Parameters.AddWithValue("@City", city);
                            cmd.Parameters.AddWithValue("@Country",country);
                            cmd.Parameters.AddWithValue("CustomerID", customerID);

                            cmd.ExecuteNonQuery();
                        }                    
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine($"{ex.Message}");
                    }                
                }
            }


            static void CustomerList() 
            {                              

                Console.WriteLine("What do you want to sort by");
                Console.WriteLine("CustomerID, Company Name, Contact Name, Address, City or Country");
                string orderByColumn = Console.ReadLine();

                using (SqlConnection GatesOfHell = ConnectionString())
                {
                    try
                    {
                        GatesOfHell.Open();
                        using (SqlCommand cmd = new SqlCommand("spCustomerList", GatesOfHell))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@OrderByColumn", orderByColumn);


                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                var table = new Table()
                                .Border(TableBorder.HeavyEdge)
                                .Title("[#ff005f]Customers[/]")
                                    .AddColumn(new TableColumn("[#5faf00]CustomerID[/]").Centered())
                                    .AddColumn(new TableColumn("[#8700af]Company Name[/]").Centered())
                                    .AddColumn(new TableColumn("[#875fff]Contact Name[/]").Centered())
                                    .AddColumn(new TableColumn("[#af8700]Address[/]").Centered())
                                    .AddColumn(new TableColumn("[#5faf00]City[/]").Centered())
                                    .AddColumn(new TableColumn("[#00ff00]Country[/]").Centered());


                                while (reader.Read())
                                {
                                    table.AddRow(reader["CustomerID"].ToString(),
                                         reader["Company Name"].ToString(),
                                         reader["Contact Name"].ToString(),
                                         reader["Address"].ToString(),
                                         reader["City"].ToString(),
                                         reader["Country"].ToString());
                                }
                                // Render the table
                                AnsiConsole.Render(table);
                                Console.WriteLine("Press random key to continue.");
                                
                                //reader.Close();
                                //Console.ReadKey();
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                    }
                    Console.ReadKey();
                    Console.Clear();
                }               
                
            }

            static void ShowCountrySales() 
            {
                Console.WriteLine("Enter country: ");
                string shipCountry = Console.ReadLine();

                using (SqlConnection GatesOfHell = ConnectionString())
                {
                    GatesOfHell.Open();

                    using (SqlCommand cmd = new SqlCommand("spShitCountry", GatesOfHell ))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ShipCountry", shipCountry);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var table = new Table();
                            table.AddColumn("[#8700d7]Ship Country[/]", c => c.Width(15));
                            table.AddColumn("[#8700d7]Name[/]", c => c.Width(30));
                            table.AddColumn("[#8700d7]Sales[/]", c => c.Width(15));

                            while (reader.Read())
                            {
                                table.AddRow(
                                    reader.GetString(0),
                                    reader.GetString(1),
                                    reader.GetDecimal(2).ToString("0.00")
                                );
                            }

                            AnsiConsole.Write(table);
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                }


            }

            static void Order() 
            {
               
                
                AnsiConsole.WriteLine($"Latest Customer ID: {Customer}");
                
               
                // Query to retrieve product information
                string productQuery = "SELECT ProductID, ProductName, UnitPrice " +
                                      "FROM Products " +
                                      "Order by ProductName";

                // Load products into a list
                List<(int productId, string productName, decimal unitPrice)> productChoices = new List<(int, string, decimal)>();
                using (SqlConnection GatesOfHell = ConnectionString())
                {
                    using (SqlCommand command = new SqlCommand(productQuery, GatesOfHell))
                    {
                        GatesOfHell.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int productId = reader.GetInt32(0);
                                string productName = reader.GetString(1);
                                decimal unitPrice = reader.GetDecimal(2);
                                decimal formattedUnitPrice = Math.Round(unitPrice, 2);

                                productChoices.Add((productId, productName, formattedUnitPrice));
                            }
                        }
                    }
                }

                var productSelection = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<ValueTuple<int, string, decimal>>()
                        .Title("Select Products to Add to the Order")
                        .PageSize(40) // Set the page size for better display
                        .AddChoices(productChoices.Select(Choose => new ValueTuple<int, string, decimal>(Choose.Item1, Choose.Item2, Choose.Item3)))

                );

                foreach (var product in productSelection)
                {
                    Console.WriteLine($"Selected product: {product.Item1} {product.Item2} {product.Item3}");
                }

                using (SqlConnection GatesOfHell = ConnectionString())
                {
                    GatesOfHell.Open();

                    foreach (var selectedProductId in productSelection)
                    {
                        using (SqlCommand command = new SqlCommand("AddOrderAndDetails", GatesOfHell))
                        {
                            try
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@CustomerId", Customer);
                                command.Parameters.AddWithValue("@ProductId", selectedProductId.Item1);
                                command.Parameters.AddWithValue("@OrderDate", DateTime.Now);

                                Console.Write("Enter Quantity: ");
                                int quantity = int.Parse(Console.ReadLine());

                                Console.Write("Add Unit Price: ");
                                decimal unitPrice = decimal.Parse(Console.ReadLine());

                                 command.Parameters.AddWithValue("@Quantity", quantity);
                                 command.Parameters.AddWithValue("@UnitPrice", unitPrice);


                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex) 
                            {
                                                               
                                
                            }
                        }
                    }

                    GatesOfHell.Close();
                }
            if (Customer == null)
            {
                AnsiConsole.WriteLine("You have not added a Customer, \nGo to Add Customer first and add a new customer before you make an order.");
                Console.ReadLine();
                Console.Clear();
            }
            else
            {

                AnsiConsole.WriteLine($"A new order has been placed: {string.Join(", ", productSelection)}");
                AnsiConsole.WriteLine("Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
            }
            }

            static void OrderList() 
            {
                    string query = @"SELECT od.OrderID, p.ProductName, c.CustomerID, c.CompanyName 
                                    FROM [Order Details] od 
                                    JOIN Products p 
                                    ON od.ProductID = p.ProductID 
                                    JOIN Orders o 
                                    ON od.OrderID = o.OrderID 
                                    JOIN Customers c ON o.CustomerID = c.CustomerID ORDER BY od.OrderID ";

                        using (SqlConnection GatesOfHell = ConnectionString())
                        {
                            SqlCommand cmd = new SqlCommand(query, GatesOfHell);
                            GatesOfHell.Open();

                            SqlDataReader reader = cmd.ExecuteReader();

                            // Skapa en tabell i Spectre.Console
                            var table = new Table();
                            table.AddColumn("Order ID");
                            table.AddColumn("Product Name");
                            table.AddColumn("Customer ID");
                            table.AddColumn("Company Name");

                            // Lägg till rader till tabellen
                            while (reader.Read())
                            {
                                table.AddRow(reader["OrderID"].ToString(), 
                                    reader["ProductName"].ToString(), 
                                    reader["CustomerID"].ToString(), 
                                    reader["CompanyName"].ToString());
                            }

                            // Skriv ut tabellen
                            AnsiConsole.Render(table);
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadKey();
                            AnsiConsole.Clear();

                
                        }




            }

            static void ReportGenerator() 
            {
                var console = AnsiConsole.Create(new AnsiConsoleSettings());


                // Connect to SQL Server and retrieve data

                using (SqlConnection GatesOFHell = ConnectionString())
                {
                    GatesOFHell.Open();



                    Console.WriteLine("Modify your SQL-query");
                    Console.WriteLine("Columns: CustomerID, CompanyName, ContactName, ContactTitle, Address,\n City, Region, PostalCode, Country, Phone, Fax.");
                    Console.WriteLine();
                    Console.WriteLine("Write which column you want to add, seperate with comma:");
                    var colm = Console.ReadLine();
                    Console.WriteLine("Choose how you want to sort the columns or leave it empty?:");
                    var sort = Console.ReadLine();
                    Console.WriteLine("Choose your Where condition or leave it empty:");
                    var cond = Console.ReadLine();

                    // Build SQL query based on user input
                    string sqlCommand = $"SELECT {colm} from Customers";
                    if (!string.IsNullOrWhiteSpace(cond))
                    {
                        sqlCommand += $" WHERE {cond}";
                    }
                    if (!string.IsNullOrWhiteSpace(sort))
                    {
                        sqlCommand += $" ORDER BY {sort}";
                    }

                     try
                     {
                        // Execute SQL query
                        using (SqlCommand command = new SqlCommand(sqlCommand, GatesOFHell))
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            // Print column headers
                            Table table = new Table()
                            .Border(TableBorder.HeavyEdge)
                            .Title("[#ff005f]Customer Data[/]");
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                table.AddColumn(reader.GetName(i));
                            }

                            // Print data rows
                            while (reader.Read())
                            {
                                var row = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(reader[i].ToString());
                                }
                                table.AddRow(row.ToArray());
                            }

                            // Render table to console
                            console.Write(table);
                        }
                     } 
                     catch (Exception ex) 
                     {
                        Console.WriteLine(ex.Message);
                        Console.ReadLine(); 
                        
                     }
                    Console.Clear();
                }


            }

            static SqlConnection ConnectionString()
            {
                string connectionString =
                @"Data Source=G1ll1s-Nr1\SQLEXPRESS ;Initial Catalog=Northwind33;"
                + "Integrated Security=true; TrustServerCertificate = true;";
                return new SqlConnection(connectionString);

            }
        
    }
}