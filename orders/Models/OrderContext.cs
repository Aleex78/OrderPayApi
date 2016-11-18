using System;
using System.Data.Entity;

namespace PayTest.Models
{
    public class OrderContext : DbContext
    {
        class MyContextInitializer : DropCreateDatabaseAlways<OrderContext>
        {
            protected override void Seed(OrderContext db)
            {
                Card unlimited = new Card { CardNumber="2323232323232323", CardholderName="First User", CVV="130", ExpiryMonth="11", ExpiryYear=2017, IsLimited=false, Balance=1000};
                Card limited = new Card { CardNumber = "1234567890123456", CardholderName = "Second User", CVV = "181", ExpiryMonth = "03", ExpiryYear = 2019, IsLimited = true, Balance = 5000, LimitSum =-2000 };

                Status status1 = new Status { StatusId = 1, StatusName = "Оформлен" };
                Status status2 = new Status { StatusId = 2, StatusName = "Оплачен" };
                Status status3 = new Status { StatusId = 3, StatusName = "Возвращён" };

                db.Cards.Add(unlimited);
                db.Cards.Add(limited);
                db.Statuses.Add(status1);
                db.Statuses.Add(status2);
                db.Statuses.Add(status3);           
                db.SaveChanges();

                Order order1= new Order{CardNumber= "2323232323232323", StatusId = 2, OrderSum=111};
                Order order2 = new Order {StatusId = 1, OrderSum = 5500 };
                Order order3 = new Order {StatusId = 1, OrderSum = 2000 };
                Order order4 = new Order {StatusId = 1, OrderSum = 111 };
                Order order5 = new Order { StatusId = 1, OrderSum = 10000000 };

                db.Orders.Add(order1);
                db.Orders.Add(order2);
                db.Orders.Add(order3);
                db.Orders.Add(order4);
                db.Orders.Add(order5);
                db.SaveChanges();
            }
        }
        static OrderContext()
        {
            Database.SetInitializer<OrderContext>(new MyContextInitializer());
        }
        public DbSet<Order> Orders { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Status> Statuses { get; set; }
    }
}