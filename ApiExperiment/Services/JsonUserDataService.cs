using ApiExperiment.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiExperiment.Services
{
    public class JsonUserDataService
    {
        private readonly string _filePath;
        private readonly object _lock = new object();
        private readonly ILogger<JsonUserDataService> _logger;

        public JsonUserDataService(ILogger<JsonUserDataService> logger)
        {
            _logger = logger;

            // Get the directory where the executable is located
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define a subdirectory for data storage
            string dataDirectory = Path.Combine(baseDirectory, "Data");

            // Ensure the directory exists
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
                _logger.LogInformation($"Created data directory at {dataDirectory}");
            }
            else
            {
                _logger.LogInformation($"Data directory already exists at {dataDirectory}");
            }

            // Define the path to the JSON file
            _filePath = Path.Combine(dataDirectory, "UserData.json");
            _logger.LogInformation($"User data file path: {_filePath}");

            // Initialize the JSON file if it doesn't exist
            if (!File.Exists(_filePath))
            {
                _logger.LogInformation("User data file does not exist. Creating new file.");
                var defaultData = new List<UserData>();
                WriteToFile(defaultData);
            }
            else
            {
                _logger.LogInformation("User data file already exists.");
            }
        }
        public UserData GetUserDataById(string id)
        {
            var allUserData = GetAllUserData();
            return allUserData.FirstOrDefault(u => u.Id == id);
        }
        public UserData GenerateRandomUserData()
        {
            var firstNames = new[] { "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Nancy", "Daniel", "Lisa", "Matthew", "Betty" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez" };
            var domains = new[] { "example.com", "test.com", "demo.com", "mail.com", "sample.net" };
            var streetNames = new[] { "Main St", "High St", "Maple Ave", "Oak Rd", "Pine Ln", "Cedar Blvd", "Elm St", "Walnut Dr", "Birch Ct", "Ash St", "Cherry Ave", "Dogwood Rd", "Fir Ln", "Grove Blvd", "Hickory St", "Ivy Dr", "Juniper Ct", "Kent Ave", "Linden Rd", "Magnolia Ln", "Nutmeg Blvd", "Olive St", "Poplar Ave", "Quince Rd", "Redwood Ln", "Sycamore Blvd" };
            var cities = new[] { "Springfield", "Riverside", "Greenville", "Fairview", "Madison", "Georgetown", "Arlington", "Ashland", "Burlington", "Clinton", "Dayton", "Franklin", "Hamilton", "Milton", "Newark", "Oxford", "Princeton", "Salem", "Trenton", "Winchester", "York", "Zanesville", "Lexington", "Bloomington", "Cleveland", "Dover" };
            var states = new[] { "NY", "CA", "TX", "FL", "IL", "PA", "OH", "GA", "NC", "MI", "NJ", "VA", "WA", "AZ", "MA", "TN", "IN", "MO", "MD", "WI", "CO", "MN", "SC", "AL", "LA" };
            var accessLevels = new[] { "Public", "Confidential", "Secret", "TopSecret" }; // Must match enum names exactly

            var random = new Random();

            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];
            string fullName = $"{firstName} {lastName}";
            string userEmail = $"{random.Next(1000, 9999)}@{domains[random.Next(domains.Length)]}";
            string phoneNum = $"({random.Next(200, 999)}) {random.Next(200, 999)}-{random.Next(1000, 9999)}";
            string address = $"{random.Next(100, 9999)} {streetNames[random.Next(streetNames.Length)]}, {cities[random.Next(cities.Length)]}, {states[random.Next(states.Length)]} {random.Next(10000, 99999)}";

            string accessLevelString = accessLevels[random.Next(accessLevels.Length)];
            AccessLevel accessLevelEnum = Enum.Parse<AccessLevel>(accessLevelString);

            return new UserData
            {
                FullName = fullName,
                UserEmail = userEmail,
                PhoneNum = phoneNum,
                Address = address,
                AccessLevel = accessLevelEnum
            };
        }

        public List<UserData> GetUserDataByAccessLevel(AccessLevel accessLevel)
        {
            lock (_lock)
            {
                try
                {
                    var allUserData = GetAllUserData();
                    var filteredData = allUserData
                        .Where(u => u.AccessLevel == accessLevel)
                        .ToList();
                    return filteredData;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting user data by access level.");
                    throw;
                }
            }
        }

        public List<UserData> GetAllUserData()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        return new List<UserData>();
                    }

                    var json = File.ReadAllText(_filePath);
                    var options = new JsonSerializerOptions
                    {
                        Converters = { new JsonStringEnumConverter() }
                    };
                    var data = JsonSerializer.Deserialize<List<UserData>>(json, options);
                    return data ?? new List<UserData>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading user data from file.");
                    throw;
                }
            }
        }

        public void UpdateUserData(List<UserData> userDataList)
        {
            lock (_lock)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Converters = { new JsonStringEnumConverter() }
                    };
                    var json = JsonSerializer.Serialize(userDataList, options);
                    File.WriteAllText(_filePath, json);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error writing user data to file.");
                    throw;
                }
            }
        }

        public void AddUserData(UserData newData)
        {
            ArgumentNullException.ThrowIfNull(newData);

            lock (_lock)
            {
                try
                {
                    var userDataList = GetAllUserData();
                    int newIdNumber = userDataList.Count + 1;
                    newData.Id = $"UD{newIdNumber}";
                    userDataList.Add(newData);
                    WriteToFile(userDataList);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Error writing user data to JSON file.");
                    throw new Exception("Error writing user data.", ex);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error serializing user data to JSON file.");
                    throw new Exception("Error serializing user data.", ex);
                }
            }
        }

        private void WriteToFile(List<UserData> data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
