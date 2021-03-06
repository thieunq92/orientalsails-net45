using System;
using System.Collections;
using System.Collections.Generic;
using CMS.Core.Domain;
using Portal.Modules.OrientalSails.Web.Util;

namespace Portal.Modules.OrientalSails.Domain
{
    public class SailExpense
    {
        protected IList services;

        public SailExpense()
        {
            Id = -1;
        }

        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual Cruise Cruise { get; set; }
        public virtual SailExpensePayment Payment { get; set; }

        public virtual IList Services
        {
            get
            {
                if (services == null)
                {
                    services = new ArrayList();
                }
                return services;
            }
            set
            {
                services = value;
            }
        }

        public virtual bool LockIncome { get; set; }
        public virtual bool LockOutcome { get; set; }
        public virtual DateTime? LockDate { get; set; }
        public virtual User LockBy { get; set; }

        private int _numberOfGroup;
        public virtual int NumberOfGroup
        {
            get
            {
                if (_numberOfGroup <= 0)
                {
                    _numberOfGroup = 1;
                }
                return _numberOfGroup;
            }
            set { _numberOfGroup = value; }
        }

        public virtual Dictionary<CostType, double> Calculate(IList costTypes, GetCurrentCostTable costTable, GetCurrentDailyCostTable dailyTable, GetCurrentCruiseExpenseTable getCruiseTable, Cruise activecruise, IList bookings, SailsModule module, bool partnership)
        {
            Cruise = activecruise;
            // Nếu là chi phí từng tàu, tính chi phí cho tàu đó
            // Nếu là chi phí tổng, tính chi phí từng tàu rồi sau đó cộng lại

            #region -- Chi phí cho một tàu --
            if (Cruise != null)
            {
                // Dựng bảng dịch vụ trắng
                Dictionary<CostType, ExpenseService> serviceMap = new Dictionary<CostType, ExpenseService>();
                Dictionary<CostType, double> serviceTotal = new Dictionary<CostType, double>();
                foreach (CostType type in costTypes)
                {
                    serviceMap.Add(type, null);
                    serviceTotal.Add(type, 0);
                }

                #region -- Tạo bảng giá trắng và lấy giá nhập thủ công theo thuyến --
                // Kiểm tra xem đã có giá các dịch vụ nào
                foreach (ExpenseService service in Services)
                {
                    // Nếu không thuộc diện tính chi phí cho ngày (không nằm trong danh sách chi phí)
                    if (!serviceMap.ContainsKey(service.Type))
                    {
                        continue;
                    }

                    serviceMap[service.Type] = service;

                    // Nếu là giá nhập thủ công thì cộng luôn
                    if (service.Type.IsDailyInput)
                    {
                        serviceTotal[service.Type] += service.Cost;
                    }
                }
                #endregion

                int adultHaiPhong = 0;
                int childHaiPhong = 0;

                // Tính giá từng dịch vụ với từng booking (chi phí theo số khách )
                #region -- Dịch vụ theo booking (chi phí theo số khách) --
                foreach (Booking booking in bookings)
                {
                    Dictionary<CostType, double> bookingCost = booking.Cost(costTable(Date, booking.Trip, booking.TripOption), costTypes);

                    // Sau khi có bảng giá từng booking thì cộng vào tổng
                    foreach (CostType type in costTypes)
                    {
                        serviceTotal[type] += bookingCost[type];
                    }

                    // Đồng thời tính số người để tính giá thuê tàu Hải Phong luôn
                    adultHaiPhong += booking.Adult;
                    childHaiPhong += booking.Child;
                }
                #endregion

                #region -- Chi phí theo chuyến --
                bool _isRun = bookings.Count > 0; // Nếu có booking thì tính chi phí theo chuyến (tàu có chạy)

                if (_isRun)
                {
                    DailyCostTable table = dailyTable(Date);
                    if (table != null)
                    {
                        foreach (DailyCost cost in dailyTable(Date).Costs)
                        {
                            if (serviceTotal.ContainsKey(cost.Type))
                            {
                                serviceTotal[cost.Type] += cost.Cost; // Luôn cộng luôn chi phí vào tổng
                            }
                        }
                    }
                }

                #endregion

                #region -- Giá tàu Hải Phong --

                // Chỉ tính giá tàu Hải Phong nếu đây là bảng chi phí cho một tàu
                CruiseExpenseTable cruiseTable = getCruiseTable(Date, Cruise);
                CalculateCruiseExpense(costTypes, serviceTotal, adultHaiPhong, childHaiPhong, cruiseTable);

                #endregion

                #region -- Trước khi trả về kết quả, kiểm tra cơ sở dữ liệu --
                foreach (CostType type in costTypes)
                {
                    // Bỏ qua dịch vụ theo ngày vì đã lưu theo từng dịch vụ riêng rẽ
                    if (type.IsDailyInput)
                    {
                        continue;
                    }
                    if (serviceMap[type] != null)
                    {
                        // Nếu giá dịch vụ trong CSDL không bằng thực tính
                        if (serviceMap[type].Cost != serviceTotal[type])
                        {
                            serviceMap[type].Cost = serviceTotal[type];
                            module.SaveOrUpdate(serviceMap[type]);
                        }
                        // Ngược lại thì bỏ qua
                    }
                    else
                    {
                        // Nếu chưa có thì cập nhật mới
                        if (type.DefaultAgency == null && partnership)
                        {
                            throw new Exception("You must config default agency for " + type.Name);
                        }
                        ExpenseService service = new ExpenseService();
                        service.Expense = this;
                        service.Cost = serviceTotal[type];
                        service.Name = string.Format("{0:dd/MM/yyyy}- {1}", Date, type.Name);
                        service.Paid = 0;
                        service.Supplier = type.DefaultAgency;
                        if (service.Supplier != null)
                        {
                            service.Phone = type.DefaultAgency.Phone;
                        }
                        service.Type = type;
                        module.SaveOrUpdate(service);
                    }
                }
                #endregion
                return serviceTotal;
            }
            #endregion

            #region -- Chi phí cho tất cả các tàu --
            Dictionary<CostType, double> total = new Dictionary<CostType, double>();
            #region -- Lấy về chi phí cho từng tàu nếu là chi phí tổng --

            //Chi phí cho từng tàu
            //Dictionary<int, SailExpense> expenseCruise = new Dictionary<int, SailExpense>();
            IList cruises = module.CruiseGetAll();

            #region -- Tạo bảng giá trắng --
            foreach (CostType type in costTypes)
            {
                total.Add(type, 0);
            }
            #endregion

            foreach (Cruise cruise in cruises)
            {
                SailExpense expense = module.ExpenseGetByDate(cruise, Date);

                IList filtered = new ArrayList();
                foreach (Booking booking in bookings)
                {
                    if (booking.Cruise != null && booking.Cruise.Id == cruise.Id)
                    {
                        filtered.Add(booking);
                    }
                }

                Dictionary<CostType, double> expenses = expense.Calculate(costTypes, costTable, dailyTable,
                                                                          getCruiseTable, cruise, filtered, module,
                                                                          partnership);
                foreach (CostType type in costTypes)
                {
                    total[type] += expenses[type];
                }
            }

            #endregion

            return total;
            #endregion
        }

        public virtual void CalculateCruiseExpense(IList costTypes, IDictionary<CostType, double> serviceTotal, int adultHaiPhong, int childHaiPhong, CruiseExpenseTable cruiseTable)
        {
            CostType cruise = null;
            foreach (CostType type in costTypes)
            {
                if (type.Id == SailsModule.HAIPHONG)
                {
                    cruise = type;
                    break;
                }
            }

            // Nếu tồn tại loại chi phí có ID giống như cấu hình cứng tại SailsModule.HAIPHONG
            if (cruise != null)
            {
                // Tính số khách dùng để tính giá
                double haiphong = adultHaiPhong + childHaiPhong / 2;

                if (haiphong > 0)
                {
                    #region Dò tìm dòng giá tàu Hải Phong phù hợp với số khách
                    int index = -1;
                    for (int ii = 0; ii < cruiseTable.Expenses.Count; ii++)
                    {
                        CruiseExpense cExpense = (CruiseExpense)cruiseTable.Expenses[ii];
                        if (cExpense.CustomerFrom <= haiphong && cExpense.CustomerTo >= haiphong)
                        {
                            index = ii;
                            break;
                        }
                    }

                    if (index < 0)
                    {
                        throw new Exception("Hai phong cruise price is not valid, can not find price for " +
                                            haiphong +
                                            " persons");
                    }
                    #endregion

                    #region Dựa vào dòng giá, tính chi phí
                    serviceTotal[cruise] += ((CruiseExpense)cruiseTable.Expenses[index]).Price;
                    // Nếu đúng bằng, lấy luôn giá này
                    //if (((CruiseExpense)cruiseTable.Expenses[index]).CustomerFrom == haiphong)
                    //{
                    //    serviceTotal[cruise] += ((CruiseExpense)cruiseTable.Expenses[index]).Price;
                    //}
                    //else
                    //{
                    //    if (index < 1)
                    //    {
                    //        throw new Exception("Hai phong cruise price is not valid, can not calculate for " +
                    //                            haiphong +
                    //                            " persons");
                    //    }
                    //    CruiseExpense upperExpense = (CruiseExpense)cruiseTable.Expenses[index];
                    //    CruiseExpense lowerExpense = (CruiseExpense)cruiseTable.Expenses[index - 1];

                    //    if (lowerExpense.CustomerTo != upperExpense.CustomerFrom - 1)
                    //    {
                    //        throw new Exception("Hai phong cruise price is not valid, price table must be continity");
                    //    }

                    //    // Nếu nhỏ hơn thì có 2 trường hợp: nhỏ hơn và nằm trong khoảng nhỏ
                    //    if (lowerExpense.CustomerTo > haiphong)
                    //    {
                    //        // Công thức sai toét, nhưng tạm thời bỏ qua
                    //        serviceTotal[cruise] += lowerExpense.Price * haiphong /
                    //                               (lowerExpense.CustomerTo - lowerExpense.CustomerFrom + 1);
                    //    }
                    //    else
                    //    {
                    //        serviceTotal[cruise] += lowerExpense.Price +
                    //                                (upperExpense.Price - lowerExpense.Price) *
                    //                                (haiphong - lowerExpense.CustomerTo);
                    //    }
                    //}
                    #endregion
                }
            }
        }

        public virtual Dictionary<CostType, double> GetPayable(IList costTypes)
        {
            Dictionary<CostType, double> serviceTotal = new Dictionary<CostType, double>();
            foreach (CostType type in costTypes)
            {
                serviceTotal.Add(type, 0);
            }

            foreach (ExpenseService service in Services)
            {
                serviceTotal[service.Type] += service.Cost - service.Paid;
            }

            return serviceTotal;
        }
    }

    public delegate CostingTable GetCurrentCostTable(DateTime date, SailsTrip trip, TripOption option);
    public delegate DailyCostTable GetCurrentDailyCostTable(DateTime date);
    public delegate CruiseExpenseTable GetCurrentCruiseExpenseTable(DateTime date, Cruise cruise);
}