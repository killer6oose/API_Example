using KeycloakTesting.Models;
using System.Text.Json;

namespace KeycloakTesting.Services
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


        public List<UserData> GetAllUserData()
             {
            lock (_lock)
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    return JsonSerializer.Deserialize<List<UserData>>(json) ?? new List<UserData>();
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Error reading user data from JSON file.");
                    throw new Exception("Error reading user data.", ex);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing user data from JSON file.");
                    throw new Exception("Error deserializing user data.", ex);
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
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        public void UpdateUserData(List<UserData> updatedList)
        {
            ArgumentNullException.ThrowIfNull(updatedList);

            lock (_lock)
            {
                try
                {
                    WriteToFile(updatedList);
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


    }
}
