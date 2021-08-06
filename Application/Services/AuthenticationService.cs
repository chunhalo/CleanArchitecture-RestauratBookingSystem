using Application.Interfaces;
using Application.ViewModels;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public IAuthenticationRepo _authenticationRepo;
        public AuthenticationService(IAuthenticationRepo authenticationRepo)
        {
            _authenticationRepo = authenticationRepo;
        }

        public async Task<ResponseViewModel> checkUserExist(RegisterModel model)
        {
            ResponseViewModel responseViewModel = new ResponseViewModel();
            responseViewModel.response = await _authenticationRepo.checkUserExist(model);
            return responseViewModel;
        }

        public async Task<ResponseViewModel> checkEmailExist(RegisterModel model)
        {
            ResponseViewModel responseViewModel = new ResponseViewModel();
            responseViewModel.response = await _authenticationRepo.checkEmailExist(model);
            return responseViewModel;
        }

        public async Task<JsonToken> CreateToken(LoginModel model)
        {
            JsonToken jsonToken = new JsonToken();
            jsonToken = await _authenticationRepo.CreateToken(model);
            return jsonToken;
        }

        public async Task<ResponseViewModel> Login(LoginModel model)
        {
            ResponseViewModel responseViewModel = new ResponseViewModel();
            responseViewModel.response = await _authenticationRepo.Login(model);
            return responseViewModel;

            
        }

        public async Task<ResponseViewModel> Register(RegisterModel model, string clientlink)
        {
            ResponseViewModel responseViewModel = new ResponseViewModel();
            responseViewModel.response = await _authenticationRepo.Register(model,clientlink);
            return responseViewModel;
        }
    }
}
