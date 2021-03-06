using System;
using System.Windows.Input;
using Common.Logging;
using InstantReview.Droid;
using InstantReview.Login;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO.IsolatedStorage;
using System.Net;
using InstantReview.Views;

namespace InstantReview.ViewModels
{
    public class LoginPageViewModel : BaseViewModel ,ILoginPageViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger<LoginPageViewModel>();

        public event EventHandler<EventArgs> LoginSuccessful;
        private readonly IDialogService dialogService;
        private readonly IConnectionHandler _connectionHandler;
        private readonly INavigation navigation;
        private readonly IPageFactory pageFactory;
        private readonly RegisterPageViewModel registerPageViewModel;

        public LoginPageViewModel(IConnectionHandler connectionHandler, IDialogService dialogService, INavigation navigation, IPageFactory pageFactory, RegisterPageViewModel registerPageViewModel)
        {
            this._connectionHandler = connectionHandler;
            this.dialogService = dialogService;
            this.navigation = navigation;
            this.pageFactory = pageFactory;
            this.registerPageViewModel = registerPageViewModel;
            registerPageViewModel.RegisterSuccessful += RegisterPageViewModelOnRegisterSuccessful;
        }

        private void RegisterPageViewModelOnRegisterSuccessful(object sender, EventArgs e)
        {
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }

        public ICommand LoginCommand => new Command(StartLoginProcess);

        public ICommand RegisterCommand => new Command(NavigateToRegisterPage);

        private async void NavigateToRegisterPage()
        {
            await navigation.PushAsyncSingle(CreateRegisterPage());
        }

        private Page CreateRegisterPage()
        {
            return pageFactory.CreatePage<RegisterPage, RegisterPageViewModel>(registerPageViewModel);
        }

        private async void StartLoginProcess()
        {
            var success = false;
            try
            {
                var response = await _connectionHandler.Login(Username, Password);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Response was not OK. Aborting. Reason: "+ responseBody);
                }

                var token = JsonConvert.DeserializeObject<Response>(await response.Content.ReadAsStringAsync());

                var isActive = _connectionHandler.CheckTokenValidity(token.token);

                if (isActive)
                {
                    _connectionHandler.SaveUsagePrivileges(token.token);
                    dialogService.ShowLoginToast();
                    success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error("Error while logging in!", e);
            }
            finally
            {
                Password = string.Empty;
                OnPropertyChanged(nameof(Password));
            }

            if (success)
            {
                Log.Debug("Logged in successfully!");
                Username = string.Empty;
                OnPropertyChanged(nameof(Username));
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
        }

        

        protected class Response
        {
            public string token;
        }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}