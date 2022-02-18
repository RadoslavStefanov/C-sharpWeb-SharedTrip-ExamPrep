using SharedTrip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTrip.Contracts
{
    public interface ITripService
    {
        public (bool isCreated, IEnumerable<ErrorViewModel> errors) Create(AddViewModel model);
        (bool isValid, IEnumerable<ErrorViewModel> errors) ValidateAddModel(AddViewModel model);
        public IEnumerable<AllViewModel> GetAll();
        public DetailsViewModel GetDetails(string id);
        void AddUserToTrip(string tripId, string id);
    }
}
