using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PayTest.Models;

namespace PayTest.Controllers
{
    public class OrderController : ApiController
    {
        private const string PAY_COMMAND = "pay";
        private const string STATUS_COMMAND = "status";
        private const string REFUND_COMMAND = "refund";

        /// <summary>
        /// Осуществление платежа.
        /// </summary>
        [HttpPost]
        [Route(PAY_COMMAND)]
        public IHttpActionResult PayPost(Payment payment)
        {
            if (payment == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                //Можно сделать необходимые преобразования данных (например, типов)
                OrderContext context = new OrderContext();
                context.Payments.Add(payment);
                context.SaveChanges();

                Card card = context.Cards.Where(c => c.CardNumber == payment.CardNumber &&
                    c.ExpiryMonth == payment.ExpiryMonth && c.ExpiryYear == payment.ExpiryYear &&
                    c.CVV == payment.CVV && ((c.IsLimited == false) ? true : (c.Balance - payment.AmountKop >= c.LimitSum))).FirstOrDefault();

                if (card ==null)
                    return BadRequest("Операции по карте недоступны");

                Order order = context.Orders.Where(o => o.OrderId == payment.OrderId && o.OrderSum == payment.AmountKop && o.StatusId == 1).FirstOrDefault();

                if (order == null)
                    return BadRequest("Оплата заказа невозможна");

                card.Balance -= order.OrderSum;
                order.StatusId = 2;
                order.CardNumber = payment.CardNumber;
                context.SaveChanges();
                return Ok();

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Статус.
        /// </summary>
        [HttpGet]
        //[Route(STATUS_COMMAND)]
        public IHttpActionResult Get(int id)
        {
            OrderContext context = new OrderContext();
            int? status = context.Orders.Where(o => o.OrderId == id).FirstOrDefault().StatusId;

            if (status == null)
                return BadRequest("Заказ не найден");

            return Ok(context.Statuses.Where(s=>s.StatusId==status).FirstOrDefault().StatusName);
        }

        /// <summary>
        /// Возврат.
        /// </summary>
        [HttpPut]
        [Route(REFUND_COMMAND)]
        public IHttpActionResult Refund(int id)
        {
            try
            {
                OrderContext context = new OrderContext();

                Order order = context.Orders.Where(o => o.OrderId == id && o.StatusId == 2).FirstOrDefault();

                if (order == null)
                    return BadRequest("Оплаченного заказа не найдено");

                Card card = context.Cards.Where(c => c.CardNumber == order.CardNumber).FirstOrDefault();

                if (card == null)
                    return BadRequest("Карта не найдена");

                card.Balance += order.OrderSum;
                order.StatusId = 3;
                order.CardNumber = null;
                context.SaveChanges();
                return Ok();

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
