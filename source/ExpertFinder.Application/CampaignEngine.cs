using Common.Entities;
using Common.Entities.Enums;
using Common.Web.Data;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.Enums;
using ExpertFinder.Common.ViewModels.Campaigns;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpertFinder.Application
{
    public class CampaignEngine : ICampaignEngine
    {
        private readonly IUserEngine _userEngine;
        private readonly teklifcepteDBContext _dbContext;
        private readonly ICachingEx _cachingEx;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CampaignEngine(
            IUserEngine userEngine,
            teklifcepteDBContext dbContext,
            ICachingEx cachingEx,
            INotificationService notificationService,
            IConfiguration configuration,
            ILogger logger
            )
        {
            _userEngine = userEngine;
            _dbContext = dbContext;
            _cachingEx = cachingEx;
            _notificationService = notificationService;
            _configuration = configuration;
            _logger = logger;
        }

        public IEnumerable<Campain> GetCampaigns()
        {
            if (!_cachingEx.Exists("Campaigns"))
            {
                var result = _dbContext.Campain
                     .Where(i => i.IsActive == true && i.IsFree == false)
                     .OrderBy(i => i.Priority)
                     .ToList();

                _cachingEx.Add("Campaigns", result);
            }

            return _cachingEx.Get<List<Campain>>("Campaigns");
        }

        public Campain GetCampaign(string slugUrl)
        {
            var result = _dbContext.Campain
                     .FirstOrDefault(i => i.IsActive == true && i.IsFree == false && i.SlugUrl == slugUrl);

            return result;
        }

        public Campain GetCampaignByOrderId(string orderid)
        {
            var result = _dbContext.BoughtHistory
                .Include(i => i.Campain)
                .Where(i => i.Id == new Guid(orderid))
                .Select(i => i.Campain)
                .FirstOrDefault();

            return result;
        }

        async public Task<TransactionResult> PayCampain(string UserId, PaymentViewModel model, string userIP)
        {
            //try
            //{
            //    var paymentModel = new VPosTransactionResponseContract();
            //    paymentModel.VPosMessage = new KuveytTurkVPosMessage();
            //    paymentModel.VPosMessage.CardNumber = model.CardNumber.Trim().Replace(" ", string.Empty);
            //    paymentModel.VPosMessage.CardExpireDateMonth = model.MonthId.ToString("00");
            //    paymentModel.VPosMessage.CardExpireDateYear = model.YearId.ToString();
            //    if (model.CardType == (int)PaymentTypes.Visa)
            //        paymentModel.VPosMessage.CardType = "Visa";
            //    if (model.CardType == (int)PaymentTypes.MasterCard)
            //        paymentModel.VPosMessage.CardType = "MasterCard";
            //    paymentModel.VPosMessage.CardCVV2 = model.CVC.Trim();
            //    paymentModel.VPosMessage.CardHolderName = model.FullName.Trim();
            //    paymentModel.VPosMessage.Amount = (int)(model.Campaign.Price.Value * 100);
            //    paymentModel.VPosMessage.TransactionSecurity = 3; //3d secure
            //    paymentModel.VPosMessage.Description = model.Campaign.Title;

            //    var user = await _userEngine.CurrentUser(UserId);
            //    if (user != null)
            //    {
            //        var bought = new BoughtHistory()
            //        {
            //            Id = Guid.NewGuid(),
            //            CampainId = model.Campaign.Id,
            //            OrderAddress = model.OrderAddress,
            //            UserId = user.Id,
            //            CreatedDate = DateTime.Now,
            //            IsPaid = false
            //        };

            //        _dbContext.BoughtHistory.Add(bought);
            //        _dbContext.SaveChanges();

            //        paymentModel.VPosMessage.MerchantOrderId = bought.Id.ToString();
            //    }
            //    else
            //    {
            //        return new TransactionResult(message: "Kullanıcı bulunamadı. Lütfen tekrar deneyiniz!", type: TransactionType.Error);
            //    }

            //    var result = Payment(paymentModel, userIP, model.Campaign.SlugUrl);

            //    return new TransactionResult() { Type = TransactionType.Success, Message = result };
            //}
            //catch (Exception ex)
            //{
            //    return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            //}
            return null;
        }

        //private string Payment(VPosTransactionResponseContract model, string userIP, string campaignSlug)
        //{
        //var siteUrl = _configuration["Frontend:Domain"];
        //var uri = _configuration["KuveytTurk:UriToPost"];

        //KuveytTurkVPosMessage request = new KuveytTurkVPosMessage();
        //request.TransactionType = "Sale";
        //request.CardNumber = model.VPosMessage.CardNumber;
        //request.CardExpireDateMonth = model.VPosMessage.CardExpireDateMonth;
        //request.CardExpireDateYear = model.VPosMessage.CardExpireDateYear;
        //request.CardType = model.VPosMessage.CardType;
        //request.CardCVV2 = model.VPosMessage.CardCVV2;
        //request.CardHolderName = model.VPosMessage.CardHolderName;
        //request.CurrencyCode = "0949";
        //request.APIVersion = "1.0.0";
        //request.MerchantId = Convert.ToInt32(_configuration["KuveytTurk:MerchantID"]);
        //request.CustomerId = Convert.ToInt32(_configuration["KuveytTurk:CustomerId"]);
        //request.MerchantOrderId = model.VPosMessage.MerchantOrderId;
        //request.Amount = model.VPosMessage.Amount;
        //request.DisplayAmount = Helper.ToAmountString(model.VPosMessage.Amount);
        //request.OkUrl = siteUrl + "odemetamamlandi/";
        //request.FailUrl = siteUrl + "odeme/" + campaignSlug;
        //request.TransactionSecurity = model.VPosMessage.TransactionSecurity;
        //request.Description = model.VPosMessage.Description;
        //request.UserName = _configuration["KuveytTurk:UserName"].ToString();
        //string password = _configuration["KuveytTurk:Password"].ToString();
        //password = Helper.ComputeHash(password);
        //request.CustomerIPAddress = userIP;
        //request.CardHolderIPAddress = userIP;
        //string newHash = Helper.CreateHashString(request, password);
        //request.HashData = Helper.ComputeHash(newHash);
        //XmlSerializer x = new XmlSerializer(request.GetType());
        //StringWriter sw = new StringWriter();
        //x.Serialize(sw, request);

        //string str = sw.ToString();

        //str = str.Replace("encoding=\"utf-16\"", "encoding=\"ISO-8859-1\"");
        //string result = Helper.DataPost(uri, str);

        //_logger.LogInformation(result);

        //if (string.IsNullOrWhiteSpace(result))
        //    result = "Boş Cevap";
        //return result;
        //    return "";
        //}

        public TransactionResult PaymentComplete(string paymentForms, string merchantOrderId)
        {
            try
            {
                _logger.LogInformation(paymentForms);

                if (!string.IsNullOrWhiteSpace(merchantOrderId))
                {
                    var bought = _dbContext.BoughtHistory
                        .Include(i => i.Campain)
                        .FirstOrDefault(i => i.Id == new Guid(merchantOrderId));

                    if (bought != null && !bought.IsPaid)
                    {
                        bought.IsPaid = true;

                        _dbContext.SaveChanges();

                        _userEngine.ClearUserToken(bought.UserId);

                        _notificationService.AddNotification(bought.UserId, NotificationTypes.PaymentComplete, param: bought.Campain.Price.HasValue ? bought.Campain.Price.Value.ToString() : string.Empty, itemId: bought.Id.ToString());
                    }
                }

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                _logger.LogError("Payment Complete Error", ex);

                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }
    }
}