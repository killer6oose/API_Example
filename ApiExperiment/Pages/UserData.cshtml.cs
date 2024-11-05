using ApiExperiment.Models;
using ApiExperiment.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiExperiment.Pages
{
    public class UserDataModel : PageModel
    {
        private readonly JsonUserDataService _userDataService;

        public UserDataModel(JsonUserDataService userDataService)
        {
            _userDataService = userDataService;
            ErrorMessage = string.Empty;
            UserDataList = new List<UserData>();
        }

        public string ErrorMessage { get; set; }

        public List<UserData> UserDataList { get; set; }

        public void OnGet()
        {
            try
            {
                UserDataList = _userDataService.GetAllUserData();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                UserDataList = new List<UserData>();
            }
        }
    }
}
