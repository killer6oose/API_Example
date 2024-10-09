using Microsoft.AspNetCore.Mvc.RazorPages;
using KeycloakTesting.Services;
using KeycloakTesting.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace KeycloakTesting.Pages
{
    public class IndexModel : PageModel
    {
        private readonly JsonUserDataService _userDataService;

        public IndexModel(JsonUserDataService userDataService)
        {
            _userDataService = userDataService;
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
