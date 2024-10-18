using System.ComponentModel.DataAnnotations;
using ApiExperiment.Data;
using ApiExperiment.Models;

namespace ApiExperiment.Services
{
    public class UserDataService
    {
        private readonly ApplicationDbContext _context;

        public UserData GetUserData()
        {
            // Assuming there's only one UserData record
            return _context.UserData.FirstOrDefault() ?? new UserData();
        }

        public void UpdateUserData(UserData newData)
        {
            var existingData = _context.UserData.FirstOrDefault();
            if (existingData != null)
            {
                existingData.PhoneNum = newData.PhoneNum;
                existingData.UserEmail = newData.UserEmail;
                existingData.FullName = newData.FullName;
                existingData.Address = newData.Address;
                // Update other properties as needed

                _context.SaveChanges();
            }
            else
            {
                _context.UserData.Add(newData);
                _context.SaveChanges();
            }
        }
    }
}
