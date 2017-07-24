using Common.Entities;
using Common.Entities.Enums;
using ExpertFinder.Common.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UstaEvi.com.Views.Shared.Components.Modal
{
    public class Modal : ViewComponent
    {
        public Modal()
        {
        }

        async public Task<IViewComponentResult> InvokeAsync(TransactionResult Transaction)
        {
            var model = new ModalViewModel()
            {
                ReturnUrl = Transaction.ReturnUrl,
                Message = Transaction.Message
            };

            switch (Transaction.Type)
            {
                case TransactionType.Success:
                    model.Type = "success";
                    break;

                case TransactionType.Error:
                    model.Type = "error";
                    break;
            }

            return View(model);
        }
    }
}