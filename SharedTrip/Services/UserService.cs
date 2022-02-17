using SharedTrip.Contracts;
using SharedTrip.Data.Common;
using SharedTrip.Data.Model;
using SharedTrip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharedTrip.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository repo;

        public UserService(IRepository _repo)
        { repo = _repo; }

        public string GetUsername(string userId)
        {
            return repo.All<User>()
                .FirstOrDefault(u => u.Id == userId)?.Username;
        }

        public string Login(LoginViewModel model)
        {
            var user = repo.All<User>()
                .Where(u => u.Username == model.Username)
                .Where(u => u.Password == CalculateHash(model.Password))
                .SingleOrDefault();

            return user?.Id;
        }

        private User GetUserByUsername(string username)
        {
            return repo.All<User>().FirstOrDefault(u => u.Username == username);
        }

        public (string userId, bool isCorrect) IsLoginCorrect(LoginViewModel model)
        {
            bool isCorrect = false;
            string userId = String.Empty;

            var user = GetUserByUsername(model.Username);

            if (user != null)
            {
                isCorrect = user.Password == CalculateHash(model.Password);
            }

            if (isCorrect)
            {
                userId = user.Id;
            }

            return (userId, isCorrect);
        }
        private string CalculateHash(string password)
        {
            byte[] passworArray = Encoding.UTF8.GetBytes(password);

            using (SHA256 sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(passworArray));
            }
        }


        public (bool registered, IEnumerable<ErrorViewModel> errors) Register(RegisterViewModel model)
        {
            bool registered = false; List<ErrorViewModel> error = new List<ErrorViewModel>();

            var (isValid, validationError) = ValidateRegisterModel(model);

            if (!isValid)
            {
                return (isValid, validationError);
            }

            ICollection<UserTrip> userTrips = new List<UserTrip>();

            User user = new User()
            {
                Email = model.Email,
                Password = CalculateHash(model.Password),
                Username = model.UserName,
                UserTrips = userTrips
            };


            try
            {
                repo.Add(user);
                repo.SaveChanges();
                registered = true;
            }
            catch (Exception)
            {
                error.Add(new ErrorViewModel("Couldn't save user in DB!"));
            }

            return (registered, error);
        }

        private (bool isValid, IEnumerable<ErrorViewModel> errors) ValidateRegisterModel(RegisterViewModel model)
        {
            bool isValid = true;
            List<ErrorViewModel> errors = new List<ErrorViewModel>();

            if (model == null)
            {
                isValid = false;
                errors.Add(new ErrorViewModel("Register model is required!"));
            }

            if (String.IsNullOrWhiteSpace(model.UserName) || model.UserName.Length < 5 || model.UserName.Length > 20)
            {
                isValid = false;
                errors.Add(new ErrorViewModel($"Username must be between 5 and 20 charecters! you entered ${model.UserName}"));
            }

            if (String.IsNullOrWhiteSpace(model.Email))
            {
                isValid = false;
                errors.Add(new ErrorViewModel("The provided Email is not valid!"));
            }

            if (String.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6 || model.Password.Length > 20)
            {
                isValid = false;
                errors.Add(new ErrorViewModel("Password must be between 6 and 20 charecters!"));
            }

            return (isValid, errors);
        }

    }
}
