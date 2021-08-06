using Application.ViewModels;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ResponseViewModel> Register(RegisterModel model,string clientlink);
        Task<ResponseViewModel> checkEmailExist(RegisterModel model);
        Task<ResponseViewModel> checkUserExist(RegisterModel model);
        Task<ResponseViewModel> Login(LoginModel model);
        Task<JsonToken> CreateToken(LoginModel model);
    }
}
