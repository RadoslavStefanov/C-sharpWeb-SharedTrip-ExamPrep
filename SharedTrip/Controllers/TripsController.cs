using BasicWebServer.Server.Attributes;
using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using SharedTrip.Contracts;
using SharedTrip.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedTrip.Controllers
{
    public class TripsController : Controller
    {
        private readonly ITripService tripService;
        public TripsController(Request request, ITripService _tripService) 
            : base(request)
        {
            tripService = _tripService;
        }

        [Authorize]
        public Response Add()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public Response Add(AddViewModel model)
        {
            var(isValid, errors) = tripService.Create(model);

            if (!isValid)
            {
                return View(errors, "/Error");
            }

            return Redirect("/Trips/All");
        }

        [Authorize]
        public Response All()
        {
            List<AllViewModel> trips = tripService.GetAll().ToList();
            return View(trips);
        }

        [Authorize]
        public Response Details(string tripId)
        {
            DetailsViewModel trip = tripService.GetDetails(tripId);
            return View(trip);
        }

        [Authorize]
        public Response AddUserToTrip(string tripId)
        {
            try
            {
                tripService.AddUserToTrip(tripId, User.Id);
            }
            catch (ArgumentException aex)
            {
                return View(new List<ErrorViewModel>() { new ErrorViewModel(aex.Message) }, "/Error");
            }
            catch (Exception)
            {
                return View(new List<ErrorViewModel>() { new ErrorViewModel("Unexpected Error") }, "/Error");
            }

            return Redirect("/Trips/All");
        }
    }
}
