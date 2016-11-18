using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayTest.Models
{
    public class Status
    {
        [Required]
        public int StatusId { get; set; }
        [Required]
        public string StatusName { get; set; }
    }

    //В модели можно было бы добавить поля дат
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        public int OrderSum { get; set; }

        [Required]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        public string CardNumber { get; set; }
    }

    public class Payment
    {
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Не получен идентификатор заказа")]
        public int OrderId { get; set; }

        //Предполагаю, что номер должен состоять их 16 любых цифр
        [Required(ErrorMessage = "Не указан номер карты")]
        [RegularExpression(@"\d{16}", ErrorMessage = "Некорректный номер карты")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Не указан месяц истечения срока действия карты")]
        [RegularExpression(@"[0-2][0-9]", ErrorMessage = "Некорректное значение месяца")]
        public string ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Не указан год истечения срока действия карты")]
        [Range(2016, int.MaxValue, ErrorMessage = "Некорректное значение года")]
        //Можно было бы строкой сделать или месяц в базе хранить как int (для единообразия частей даты).
        public int ExpiryYear { get; set; }

        [Required(ErrorMessage = "Не указан верификационный код")]
        [RegularExpression(@"\d{3}", ErrorMessage = "Неверный формат верификационного кода")]
        public string CVV { get; set; }

        public string CardholderName { get; set; }

        [Required(ErrorMessage = "Не указана сумма")]
        [Range(0, uint.MaxValue, ErrorMessage = "Сумма дожна быть больше нуля")]// Атрибут зависит от бизнес логики
        //тип данных может быть другим в зависимости от теоритически возможного диапазона значений сумм
        public uint AmountKop { get; set; }
    }

    public class Card
    {
        public int CardId { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiryMonth { get; set; }
        [Required]
        public int ExpiryYear { get; set; }
        [Required]
        public string CVV { get; set; }

        public string CardholderName { get; set; }
        [Required]
        public bool IsLimited { get; set; }
        [Required]
        public int Balance { get; set; }

        public int LimitSum { get; set; }
    }

}