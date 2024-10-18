using ApiExperiment.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiExperiment.Services
{
    public class JsonServiceDataService
    {
        private readonly string _filePath;
        private readonly object _lock = new object();
        private readonly ILogger<JsonServiceDataService> _logger;

        public JsonServiceDataService(ILogger<JsonServiceDataService> logger)
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
            _filePath = Path.Combine(dataDirectory, "ServiceData.json");
            _logger.LogInformation($"Service data file path: {_filePath}");

            // Initialize the JSON file if it doesn't exist
            if (!File.Exists(_filePath))
            {
                _logger.LogInformation("Service data file does not exist. Creating new file.");
                var defaultData = new List<ServiceData>();
                WriteToFile(defaultData);
            }
            else
            {
                _logger.LogInformation("Service data file already exists.");
            }
        }

        public ServiceData GenerateRandomServiceData()
        {
            var services = new[] { "Server", "Telephone", "Computer", "Printer", "Router" };
            var streetNames = new[] { "Main St", "High St", "Maple Ave", "Oak Rd", "Pine Ln" };
            var cities = new[] { "Springfield", "Riverside", "Greenville", "Fairview", "Madison" };
            var states = new[] { "NY", "CA", "TX", "FL", "IL" };
            var accessLevels = new[] { "Public", "Confidential", "Secret", "TopSecret" };

            var random = new Random();

            string service = services[random.Next(services.Length)];
            string address = $"{random.Next(100, 9999)} {streetNames[random.Next(streetNames.Length)]}, {cities[random.Next(cities.Length)]}, {states[random.Next(states.Length)]} {random.Next(10000, 99999)}";
            string ipAddress = $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(1, 255)}";
            string ipGateway = $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(1, 255)}";

            string accessLevelString = accessLevels[random.Next(accessLevels.Length)];
            AccessLevel accessLevelEnum = Enum.Parse<AccessLevel>(accessLevelString);

            return new ServiceData
            {
                Service = service,
                Address = address,
                IPAddress = ipAddress,
                IPGateway = ipGateway,
                AccessLevel = accessLevelEnum
            };
        }


        public List<ServiceData> GetAllServiceData()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        return new List<ServiceData>();
                    }

                    var json = File.ReadAllText(_filePath);
                    var options = new JsonSerializerOptions
                    {
                        Converters = { new JsonStringEnumConverter() }
                    };
                    var data = JsonSerializer.Deserialize<List<ServiceData>>(json, options);
                    return data ?? new List<ServiceData>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading service data from file.");
                    throw;
                }
            }
        }

        public void UpdateServiceData(List<ServiceData> serviceDataList)
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
                    var json = JsonSerializer.Serialize(serviceDataList, options);
                    File.WriteAllText(_filePath, json);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error writing service data to file.");
                    throw;
                }
            }
        }

        public void AddServiceData(ServiceData newData)
        {
            ArgumentNullException.ThrowIfNull(newData);

            lock (_lock)
            {
                try
                {
                    var serviceDataList = GetAllServiceData();
                    serviceDataList.Add(newData);
                    WriteToFile(serviceDataList);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Error writing service data to JSON file.");
                    throw new Exception("Error writing service data.", ex);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error serializing service data to JSON file.");
                    throw new Exception("Error serializing service data.", ex);
                }
            }
        }

        public List<ServiceData> GetServiceDataByAccessLevel(AccessLevel accessLevel)
        {
            lock (_lock)
            {
                try
                {
                    var allServiceData = GetAllServiceData();
                    var filteredData = allServiceData
                        .Where(s => s.AccessLevel <= accessLevel)
                        .ToList();
                    return filteredData;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting service data by access level.");
                    throw;
                }
            }
        }

        public void DeleteServiceData(int index)
        {
            lock (_lock)
            {
                try
                {
                    var serviceDataList = GetAllServiceData();
                    if (index < 0 || index >= serviceDataList.Count)
                    {
                        throw new IndexOutOfRangeException("Service data index out of range.");
                    }

                    serviceDataList.RemoveAt(index);
                    UpdateServiceData(serviceDataList);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting service data.");
                    throw;
                }
            }
        }

        public void UpdateExistingServiceData(int index, ServiceData updatedData)
        {
            lock (_lock)
            {
                try
                {
                    var serviceDataList = GetAllServiceData();
                    if (index < 0 || index >= serviceDataList.Count)
                    {
                        throw new IndexOutOfRangeException("Service data index out of range.");
                    }

                    serviceDataList[index] = updatedData;
                    UpdateServiceData(serviceDataList);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating service data.");
                    throw;
                }
            }
        }

        private void WriteToFile(List<ServiceData> data)
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
