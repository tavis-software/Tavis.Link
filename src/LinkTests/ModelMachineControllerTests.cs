using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tavis;
using Xunit;

namespace LinkTests
{
    public class LoginFormModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string StatusMessage { get; set; }
    }

    public class Controller
    {
        private LoginFormModel _loginFormModel;
        public HttpResponseMachine Machine {get;set;}

        public Controller(LoginFormModel model)
        {
            _loginFormModel = model;
            Machine = new HttpResponseMachine();
            Machine.AddResponseHandler(LoginSuccessful, HttpStatusCode.OK, linkRelation: "login", contentType: null, profile: null);
            Machine.AddResponseHandler(LoginFailed, HttpStatusCode.Unauthorized, linkRelation: "login", contentType: null, profile: null);
            Machine.AddResponseHandler(LoginForbidden, HttpStatusCode.Forbidden, linkRelation: "login", contentType: null, profile: null);
            Machine.AddResponseHandler(FailedRequest, HttpStatusCode.BadRequest, linkRelation: "login", contentType: null, profile: null);
            Machine.AddResponseHandler(ResetForm, HttpStatusCode.OK, linkRelation: "reset", contentType: null, profile: null);

        }

        public async Task<HttpResponseMessage> LoginSuccessful(string linkrelation, HttpResponseMessage response)
        {
            _loginFormModel.StatusMessage = "Successfully logged in";
            
            return response;
        }

        public async Task<HttpResponseMessage> ResetForm(string linkrelation, HttpResponseMessage response)
        {
            _loginFormModel.UserName = "";
            _loginFormModel.Password = "";

            return response;
        }


        public async Task<HttpResponseMessage> LoginFailed(string linkrelation, HttpResponseMessage response)
        {
            _loginFormModel.StatusMessage = "Credentials invalid";

            return response;
        }

        public async Task<HttpResponseMessage> LoginForbidden(string linkrelation, HttpResponseMessage response)
        {
            _loginFormModel.StatusMessage = "Insufficient Permissions";

            return response;
        }

        public async Task<HttpResponseMessage> FailedRequest(string linkrelation, HttpResponseMessage response)
        {
            _loginFormModel.StatusMessage = "Unable to login -  status code " + response.StatusCode;

            return response;
        }  
    }
    public class ModelMachineControllerTests
    {
        [Fact]
        public async Task LoginSuccessful()
        {
            var loginFormModel = new LoginFormModel();
            var controller = new Controller(loginFormModel);

            controller.Machine.HandleResponseAsync("login", new HttpResponseMessage(HttpStatusCode.OK));
            Assert.Equal("Successfully logged in", loginFormModel.StatusMessage);
        
        }

        [Fact]
        public async Task LoginFailed()
        {
            var loginFormModel = new LoginFormModel();
            var controller = new Controller(loginFormModel);

            controller.Machine.HandleResponseAsync("login", new HttpResponseMessage(HttpStatusCode.Unauthorized));
            Assert.Equal("Credentials invalid", loginFormModel.StatusMessage);

        }

        [Fact]
        public async Task ResetForm()
        {
            var loginFormModel = new LoginFormModel()
            {
                UserName = "bob",
                Password = "foo"
            };
            var controller = new Controller(loginFormModel);

            controller.Machine.HandleResponseAsync("reset", new HttpResponseMessage(HttpStatusCode.OK));
            Assert.Equal("", loginFormModel.UserName);
            Assert.Equal("", loginFormModel.Password);
        }

    }
}
