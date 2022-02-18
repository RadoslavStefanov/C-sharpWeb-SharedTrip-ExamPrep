using SharedTrip.Contracts;
using SharedTrip.Data.Common;
using SharedTrip.Data.Model;
using SharedTrip.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BasicWebServer.Server.Identity;

namespace SharedTrip.Services
{
    public class TripService : ITripService
    {
        private readonly IRepository repo;
        public TripService(IRepository _repo)
        { repo = _repo; }

        public (bool isCreated, IEnumerable<ErrorViewModel> errors) 
        Create(AddViewModel model)
        {
            bool isCreated = false; 
            List<ErrorViewModel> errors = new List<ErrorViewModel>();
            var (isValid, validationError) = ValidateAddModel(model);

            if (!isValid)
            {return (isValid, validationError);}



            Trip trip = new Trip()
            {
                StartPoint = model.StartPoint,
                EndPoint = model.EndPoint,
                ImagePath = model.ImagePath,
                Seats  = model.Seats,
                Description = model.Description
            };

            DateTime date;

            DateTime.TryParseExact(
                model.DepartureTime,
                "dd.MM.yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
            trip.DepartureTime = date;

            try
            {
                repo.Add(trip);
                repo.SaveChanges();
                isCreated = true;
            }
            catch (Exception)
            {
                errors.Add(new ErrorViewModel("The database refused to creati this Trip!"));
            }

            return (isCreated, errors);
        }


        public (bool isValid, IEnumerable<ErrorViewModel> errors) ValidateAddModel(AddViewModel model)
        {
            bool isValid = true; List<ErrorViewModel> errors = new List<ErrorViewModel>();
            DateTime date;

            if (String.IsNullOrEmpty(model.StartPoint))
            {
                isValid = false;
                errors.Add(new ErrorViewModel("The starting point can't be empty!"));
            }
            if (String.IsNullOrEmpty(model.EndPoint))
            {
                isValid = false;
                errors.Add(new ErrorViewModel("The end point can't be empty!"));
            }
            if (!DateTime.TryParseExact(
                model.DepartureTime,
                "dd.MM.yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
            {
                isValid = false;
                errors.Add(new ErrorViewModel("The Departure Time cannot be empty or in a wrong format!"));
            }
            if (model.Seats < 2 || model.Seats > 6)
            {
                isValid = false;
                errors.Add(new ErrorViewModel("The Seats cannot be empty, less than 2 or more than 6!"));
            }
            if (String.IsNullOrEmpty(model.Description) || model.Description.Length > 80)
            {
                isValid = false;
                errors.Add(new ErrorViewModel("The Description cannot be empty or more than 80 characters long!"));
            }
            if (String.IsNullOrEmpty(model.ImagePath))
            {
                isValid = false;
                errors.Add(new ErrorViewModel("The ImagePath can't be empty!"));
            }

            return (isValid, errors);
        }

        public IEnumerable<AllViewModel> GetAll()
        {
             return repo.All<Trip>().Select(t => new AllViewModel()
              {
                  DepartureTime = t.DepartureTime.ToString("dd.MM.yyyy HH:mm"),
                  EndPoint = t.EndPoint,
                  Id = t.Id,
                  Seats = t.Seats,
                  StartPoint = t.StartPoint
              }).ToList();
        }

        public DetailsViewModel GetDetails(string id)
        {
            return repo.All<Trip>()
                .Where(t => t.Id == id)
                .Select(o => new DetailsViewModel()
                {
                    ImagePath = o.ImagePath,
                    StartPoint = o.StartPoint,
                    EndPoint = o.EndPoint,
                    DepartureTime = o.DepartureTime.ToString("dd.MM.yyyy HH:mm"),
                    Seats = o.Seats,
                    Description = o.Description,
                    Id = o.Id
                }).FirstOrDefault();
        }

        public void AddUserToTrip(string tripId, string userId)
        {
            var user = repo.All<User>()
                .FirstOrDefault(u => u.Id == userId );
            var trip = repo.All<Trip>()
                .FirstOrDefault(t => t.Id == tripId); 

            if (user == null || trip == null)
            {
                throw new ArgumentException("User or trip not found");
            }

            user.UserTrips.Add(new UserTrip()
            {
                TripId = tripId,
                Trip = trip,
                User = user,
                UserId = userId
            });

            repo.SaveChanges();
        }

    }
}
