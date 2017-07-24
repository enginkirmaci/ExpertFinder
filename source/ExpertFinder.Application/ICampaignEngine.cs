using Common.Entities;
using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Application.Interfaces
{
    public interface ICampaignEngine
    {
        IEnumerable<Campain> GetCampaigns();

        Campain GetCampaign(string slugUrl);

        Campain GetCampaignByOrderId(string orderid);

        //Task<TransactionResult> PayCampain(string UserId, PaymentViewModel model, string userIP);

        TransactionResult PaymentComplete(string paymentForms, string merchantOrderId);
    }
}