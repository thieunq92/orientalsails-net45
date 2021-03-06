namespace Portal.Modules.OrientalSails.Domain
{
    public class SailExpensePayment
    {
        public virtual int Id { get; set; }
        public virtual double Transfer { get; set; }
        public virtual SailExpense Expense { get; set; }
        public virtual double Ticket { get; set; }
        public virtual double Guide { get; set; }
        public virtual double Meal { get; set; }
        public virtual double Kayaing { get; set; }
        public virtual double Service { get; set; }
        public virtual double Cruise { get; set; }
        public virtual double Others { get; set; }

        public virtual double TotalPayment
        {
            get { return Transfer + Ticket + Guide + Meal + Kayaing + Service + Cruise + Others; }
        }
    }
}