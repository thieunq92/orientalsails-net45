using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using CMS.Core.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Util;

namespace Portal.Modules.OrientalSails.DataAccess
{
    [Transactional]
    public class SailsDao : ISailsDao
    {
        private readonly ISessionManager _session;

        public SailsDao(ISessionManager sessionManager)
        {
            _session = sessionManager;
        }

        #region -- Room --

        /// <summary>
        /// Lấy về danh sách các room
        /// </summary>
        /// <param name="exceptDeleted">Có loại trừ deleted không</param>
        /// <returns></returns>
        public IList RoomGetAll(bool exceptDeleted)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Room));
            if (exceptDeleted)
            {
                criteria.Add(Expression.Eq("Deleted", false));
            }
            criteria.AddOrder(new Order("Order", true));
            return criteria.List();
        }

        /// <summary>
        /// Lấy về danh Room theo class và type
        /// </summary>
        /// <param name="cruise"></param>
        /// <param name="roomClass"></param>
        /// <param name="roomType"></param>
        /// <returns></returns>
        public IList RoomGetBy_ClassType(Cruise cruise, RoomClass roomClass, RoomTypex roomType)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Room));
            criteria.Add(Expression.Eq("RoomType", roomType))
                .Add(Expression.Eq("RoomClass", roomClass));
            if (cruise != null)
            {
                criteria.Add(Expression.Eq("Cruise", cruise));
            }
            return criteria.List();

        }

        /// <summary>
        /// Kiểm tra xem có tồn tại phòng tương ứng với type và class hay ko?
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="roomClass"></param>
        /// <returns></returns>
        public bool RoomCheckExist(RoomTypex roomType, RoomClass roomClass)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Room));
            criteria.Add(Expression.Eq("RoomClass", roomClass))
                .Add(Expression.Eq("RoomType", roomType))
                .Add(Expression.Eq("Deleted", false));
            return criteria.List().Count > 0;
        }

        /// <summary>
        /// Lấy danh sách các phòng đã book tại một thời điểm
        /// </summary>
        /// <param name="cruise"></param>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public IList RoomGetNotAvailable(Cruise cruise, DateTime date, int duration)
        {
            IList bookingList = BookingRoomNotAvaiable(cruise, date, duration);

            IList lockedRoom = new ArrayList();
            //foreach (Booking booking in bookingList)
            //{
            // Đối với mỗi booking room
            foreach (BookingRoom bookingRoom in bookingList)
            {
                // Nếu booking đã chọn phòng
                if (bookingRoom.Room != null)
                {
                    if (bookingRoom.VirtualAdult == 1 && bookingRoom.RoomType.IsShared)
                    {
                        bookingRoom.Room.IsAvailable = true;
                    }
                    else
                    {
                        bookingRoom.Room.IsAvailable = false;
                    }
                    bookingRoom.Room.Adult = bookingRoom.Adult;
                    bookingRoom.Room.Child = bookingRoom.Child;
                    bookingRoom.Room.Baby = bookingRoom.Baby;
                    lockedRoom.Add(bookingRoom.Room);
                }
                // Nếu chưa chọn phòng thì không cần add
            }
            //}
            return lockedRoom;
        }

        public IList RoomGetNotAvailable(Cruise cruise, DateTime date, int duration, Booking exception)
        {
            IList bookingList = BookingRoomNotAvaiable(cruise, date, duration, exception);

            IList lockedRoom = new ArrayList();
            // Đối với mỗi booking room
            foreach (BookingRoom bookingRoom in bookingList)
            {
                // Nếu booking đã chọn phòng
                if (bookingRoom.Room != null)
                {
                    if (bookingRoom.VirtualAdult == 1 && bookingRoom.RoomType.IsShared)
                    {
                        bookingRoom.Room.IsAvailable = true;
                    }
                    else
                    {
                        bookingRoom.Room.IsAvailable = false;
                    }
                    lockedRoom.Add(bookingRoom.Room);
                }
                // Nếu chưa chọn phòng thì không cần add
            }
            return lockedRoom;
        }

        /// <summary>
        /// Duration là số ngày
        /// </summary>
        /// <param name="cruise"></param>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public IList BookingRoomNotAvaiable(Cruise cruise, DateTime date, int duration)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking));
            criteria.Add(Expression.Eq("Deleted", false));
            criteria.CreateAlias("Trip", "trip");
            criteria.Add(Expression.Eq("Cruise", cruise));

            DateTime end;
            if (duration < 2)
            {
                date = date.AddDays(-1);
                end = date.AddDays(2);
            }
            else
            {
                end = date.AddDays(duration);
            }

            #region -- Thuật toán chỉnh sửa: chỉ xét 1 trường hợp duy nhất chỉ cần ngày khởi hành < check out và ngày kết thúc > checkin --
            // S < CO & E > CI tương đương với CO > S & CI < E
            ICriterion dateCrit = Expression.And(Expression.Lt("StartDate", end), Expression.Gt("EndDate", date));

            #endregion

            criteria.Add(LockCondition());
            criteria.Add(dateCrit);

            IList bookingList = criteria.List();

            IList lockedRoom = new ArrayList();
            foreach (Booking booking in bookingList)
            {
                foreach (BookingRoom bookingRoom in booking.BookingRooms)
                {
                    lockedRoom.Add(bookingRoom);
                }
            }

            // Trả về danh sách toàn bộ các phòng đã được book trong khoảng thời gian diễn ra hành trình
            return lockedRoom;
        }

        public IList BookingRoomNotAvaiable(Cruise cruise, DateTime date, int duration, Booking exception)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking));
            criteria.Add(Expression.Eq("Deleted", false));
            criteria.CreateAlias("Trip", "trip");
            criteria.Add(Expression.Eq("Cruise", cruise));

            // date <= start < date + duration - 1 (vì ngày available tính theo số đêm chứ không phải số ngày
            //ICriterion startCrit = Expression.And(Expression.Lt("StartDate", date.AddDays(duration - 1)),
            //                                      Expression.Ge("StartDate", date));
            //// date < end <= date + duration - 1 (vì ngày available tính theo số đêm chứ không phải số ngày
            //ICriterion endCrit = Expression.And(Expression.Le("EndDate", date.AddDays(duration - 1)),
            //                                      Expression.Gt("EndDate", date));            

            // Có ngày khởi hành hoặc ngày xuất phát rơi vào thời điểm diễn ra hành trình cần check thì tức là đã chiếm phòng
            //ICriterion dateCrit = Expression.Or(startCrit, endCrit);

            var dateCrit = Expression.And(Expression.Ge("StartDate", date), Expression.Lt("StartDate", date.AddDays(duration -1)));

            criteria.Add(LockCondition());
            criteria.Add(dateCrit);

            // Loại trừ một booking
            criteria.Add(Expression.Not(Expression.Eq("Id", exception.Id)));

            IList bookingList = criteria.List();

            IList lockedRoom = new ArrayList();
            foreach (Booking booking in bookingList)
            {
                foreach (BookingRoom bookingRoom in booking.BookingRooms)
                {
                    lockedRoom.Add(bookingRoom);
                }
            }

            // Trả về danh sách toàn bộ các phòng đã được book trong khoảng thời gian diễn ra hành trình
            return lockedRoom;
        }

        public static ICriterion LockCondition()
        {
            ICriterion crit = Expression.Or(Expression.Eq("Status", StatusType.Approved),
                                            Expression.And(Expression.Ge("Deadline", DateTime.Now),
                                                           Expression.Eq("Status", StatusType.Pending)));
            // Đánh dấu là approved, trạng thái là approved và không được transfer sang tàu khác
            return
                Expression.And(crit, Expression.Eq("IsTransferred", false));
        }

        #region -- Xử lý cũ --
        /// <summary>
        /// Lấy các phòng đã bị book tại 1 thời điểm
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        //public IList BookingRoomNotAvaiable(DateTime date)
        //{
        //    ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking));
        //    criteria.Add(Expression.Eq("Deleted", false));
        //    criteria.CreateAlias(Booking.TRIP, "trip");


        //    // Tour hai ngày (=1 đêm) chỉ mất một ngày đã book
        //    // Khi càng cần nhiều ngày thì các ngày mất càng lùi về phía sau
        //    ICriterion criterion2Day = Expression.And(LockCondition(), Expression.Eq(Booking.STARTDATE, date));

        //    // Tour ba ngày (=2 đêm) mất ngày đã book thêm một ngày nữa
        //    // Đoàn này có thể cải thiện thêm để động hơn
        //    ICriterion criterion3Day = Expression.And(Expression.Eq(Booking.STARTDATE, date.AddDays(-1)),
        //                                              Expression.Eq("trip.NumberOfDay", 3));
        //    criterion3Day = Expression.And(criterion3Day, LockCondition());
        //    criteria.Add(Expression.Or(criterion2Day, criterion3Day));
        //    IList bookingList = criteria.List();
        //    return bookingList;
        //}
        #endregion

        #endregion

        #region -- Rooom Class --
        public IList RoomClassGetAll()
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(RoomClass));
            criteria.AddOrder(new Order("Order", true));
            return criteria.List();
        }
        #endregion

        #region -- Rooom Type --
        public IList RoomTypexGetAll()
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(RoomTypex));
            criteria.AddOrder(new Order("Order", true));
            return criteria.List();
        }

        public IList RoomTypeGetByCruise(Cruise cruise)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Room));
            criteria.SetProjection(Projections.Distinct(Projections.Property("RoomType")));
            criteria.Add(Expression.Eq("Cruise", cruise));
            return criteria.List();
        }
        #endregion

        #region -- Sails Trip

        /// <summary>
        /// Lấy về tất cả các SailsTrip
        /// </summary>
        /// <param name="exceptDeleted">Có loại trừ deleted ko?</param>
        /// <returns></returns>
        public IList TripGetAll(bool exceptDeleted)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(SailsTrip));
            if (exceptDeleted)
            {
                criteria.Add(Expression.Eq("Deleted", false));
            }
            return criteria.List();
        }

        #endregion

        #region -- Booking --

        /// <summary>
        /// Lấy về danh sách các booking 
        /// </summary>
        /// <param name="exceptDeleted">Có lọai trừ deleted ko?</param>
        /// <returns></returns>
        public IList BookingGetAll(bool exceptDeleted)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking));
            if (exceptDeleted)
            {
                criteria.Add(Expression.Eq("Deleted", false));
            }
            return criteria.List();
        }

        /// <summary>
        /// Tìm kiếm Booking
        /// </summary>
        /// <param name="criterion"></param>
        /// <param name="customer"></param>
        /// <param name="blocked"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public IList BookingSearch(ICriterion criterion, string customer, int blocked, Order order)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking), "booking");
            if (criterion != null)
            {
                criteria.Add(criterion);
            }

            criteria.CreateAlias("Agency", "agency", JoinType.LeftOuterJoin);
            if (!string.IsNullOrEmpty(customer))
            {
                DetachedCriteria detached = DetachedCriteria.For(typeof(Booking))
                    .CreateAlias("Customers", "customer")
                    .SetProjection(Projections.ProjectionList()
                                       .Add(Projections.RowCount()))
                        .Add(Property.ForName("Id").EqProperty("booking.Id"))
                        .Add(Expression.Like("customer.Fullname", customer, MatchMode.Anywhere));
                ICriterion crit2 = Expression.Like("agency.Name", customer, MatchMode.Anywhere);
                criteria.Add(Expression.Or(Subqueries.Le(1, detached), crit2));
            }
            criteria.SetFetchMode("booking.Cruise", FetchMode.Eager);
            criteria.CreateAlias("BookingSale", "bookingsale");



            if (blocked > 0)
            {
                // Như vậy, bản thân booking đó không được là charter
                criteria.Add(Expression.Eq("IsCharter", false));

                DetachedCriteria detached = DetachedCriteria.For(typeof(Booking))
                    .SetProjection(Projections.ProjectionList()
                                       .Add(Projections.RowCount()))
                    .Add(Property.ForName("StartDate").GeProperty("booking.StartDate"))
                    .Add(Property.ForName("StartDate").LtProperty("booking.EndDate"))
                    .Add(Property.ForName("EndDate").GtProperty("booking.StartDate"))
                    .Add(Property.ForName("EndDate").LeProperty("booking.EndDate"))
                    .Add(Property.ForName("Cruise").EqProperty("booking.Cruise"))
                    .Add(Expression.Eq("IsCharter", true))
                    .Add(Expression.Eq("Status", StatusType.Approved));
                // Nếu trong khoảng trùng ngày có một booking charter đã approved (cùng tàu)
                criteria.Add(Subqueries.Le(1, detached));
            }

            if (order != null)
            {
                criteria.AddOrder(order);
            }
            return criteria.List();
        }

        public IList BookingSearch(ICriterion criterion, string customer, int blocked, int pageSize, int pageIndex, Order order)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking), "booking");
            if (criterion != null)
            {
                criteria.Add(criterion);
            }

            criteria.CreateAlias("Agency", "agency", JoinType.LeftOuterJoin);
            if (!string.IsNullOrEmpty(customer))
            {
                DetachedCriteria detached = DetachedCriteria.For(typeof(Booking))
                    .CreateAlias("Customers", "customer")
                    .SetProjection(Projections.ProjectionList()
                                       .Add(Projections.RowCount()))
                        .Add(Property.ForName("Id").EqProperty("booking.Id"))
                        .Add(Expression.Like("customer.Fullname", customer, MatchMode.Anywhere));
                ICriterion crit2 = Expression.Like("agency.Name", customer, MatchMode.Anywhere);
                criteria.Add(Expression.Or(Subqueries.Le(1, detached), crit2));
            }

            criteria.CreateAlias("BookingSale", "bookingsale");

            if (blocked > 0)
            {
                // Như vậy, bản thân booking đó không được là charter
                criteria.Add(Expression.Eq("IsCharter", false));

                DetachedCriteria detached = DetachedCriteria.For(typeof(Booking))
                    .SetProjection(Projections.ProjectionList()
                                       .Add(Projections.RowCount()))
                    .Add(Property.ForName("StartDate").GeProperty("booking.StartDate"))
                    .Add(Property.ForName("StartDate").LtProperty("booking.EndDate"))
                    .Add(Property.ForName("EndDate").GtProperty("booking.StartDate"))
                    .Add(Property.ForName("EndDate").LeProperty("booking.EndDate"))
                    .Add(Property.ForName("Cruise").EqProperty("booking.Cruise"))
                    .Add(Expression.Eq("IsCharter", true))
                    .Add(Expression.Eq("Status", StatusType.Approved));
                // Nếu trong khoảng trùng ngày có một booking charter đã approved
                criteria.Add(Subqueries.Le(1, detached));
            }

            if (order != null)
            {
                criteria.AddOrder(order);
            }

            if (pageSize > 0)
            {
                if (pageIndex < 0)
                {
                    pageIndex = 0;
                }

                criteria.SetMaxResults(pageSize);
                criteria.SetFirstResult(pageIndex * pageSize);
            }
            return criteria.List();
        }

        public int BookingCount(ICriterion criterion, string customer, int blocked)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking), "booking");
            if (criterion != null)
            {
                criteria.Add(criterion);
            }

            if (!string.IsNullOrEmpty(customer))
            {
                DetachedCriteria detached = DetachedCriteria.For(typeof(Booking))
                    .CreateAlias("Customers", "customer")
                    .SetProjection(Projections.ProjectionList()
                                       .Add(Projections.RowCount()))
                        .Add(Property.ForName("Id").EqProperty("booking.Id"))
                        .Add(Expression.Like("customer.Fullname", customer, MatchMode.Anywhere));
                criteria.CreateAlias("Agency", "agency", JoinType.LeftOuterJoin);
                ICriterion crit2 = Expression.Like("agency.Name", customer, MatchMode.Anywhere);
                criteria.Add(Expression.Or(Subqueries.Le(1, detached), crit2));
            }

            if (blocked > 0)
            {
                // Như vậy, bản thân booking đó không được là charter
                criteria.Add(Expression.Eq("IsCharter", false));

                DetachedCriteria detached = DetachedCriteria.For(typeof(Booking))
                    .SetProjection(Projections.ProjectionList()
                                       .Add(Projections.RowCount()))
                    .Add(Property.ForName("StartDate").GeProperty("booking.StartDate"))
                    .Add(Property.ForName("StartDate").LtProperty("booking.EndDate"))
                    .Add(Property.ForName("EndDate").GtProperty("booking.StartDate"))
                    .Add(Property.ForName("EndDate").LeProperty("booking.EndDate"))
                    .Add(Property.ForName("Cruise").EqProperty("booking.Cruise"))
                    .Add(Expression.Eq("IsCharter", true))
                    .Add(Expression.Eq("Status", StatusType.Approved));
                // Nếu trong khoảng trùng ngày có một booking charter đã approved
                criteria.Add(Subqueries.Le(1, detached));
            }

            criteria.SetProjection(Projections.RowCount());
            return (int)(criteria.List()[0]);
        }

        public int BookingCount(ICriterion criterion)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking), "booking");
            criteria.CreateAlias("Agency", "agency");
            if (criterion != null)
            {
                criteria.Add(criterion);
            }

            criteria.SetProjection(Projections.RowCount());
            var list = criteria.List();
            if (list.Count > 0)
                return (int)(criteria.List()[0]);
            return 0;
        }

        public int BookingGetMaxCustomId()
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking));
            criteria.SetProjection(Projections.Max("CustomBookingId"));
            IList list = criteria.List();
            if (list != null)
            {
                return (int)list[0];
            }
            return -1;
        }
  
        #endregion

        #region -- Sail Price Config --
        /// <summary>
        /// Lấy về price config theo trip, room type và room class
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="type"></param>
        /// <param name="roomClass"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public SailsPriceConfig SailsPriceConfigGetBy_RoomType_RoomClass_Trip(SailsTrip trip, RoomTypex type, RoomClass roomClass, TripOption option)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(SailsPriceConfig));

            criteria.Add(Expression.Eq("Trip", trip))
                .Add(Expression.Eq("RoomType", type))
                .Add(Expression.Eq("RoomClass", roomClass))
                .Add(Expression.Eq("TripOption", option));
            criteria.AddOrder(new Order("Id", false));
            IList result = criteria.List();

            if (result.Count > 0)
            {
                return result[0] as SailsPriceConfig;
            }
            return null;
        }
        #endregion

        #region -- Agency --
        /// <summary>
        /// Lấy về đại lý theo role cho trước
        /// </summary>
        /// <param name="role">Role đại lý lấy về</param>
        /// <returns>AgencyPolicy</returns>
        public IList AgencyPolicyGetByRole(Role role)
        {
            ICriteria criteria =
                _session.OpenSession().CreateCriteria(typeof(AgencyPolicy))
                    .Add(Expression.Eq("Role", role))
                    .Add(Expression.Eq("ModuleType", "ORIENTALSAILS"))
                    .AddOrder(Order.Asc("CostFrom"));
            return criteria.List();
        }
        public int UserCount()
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(User))
                .SetProjection(Projections.RowCount());
            try
            {
                return Convert.ToInt32(criteria.List()[0]);
            }
            catch
            {
                return 0;
            }
        }

        public IList AgencyGetWithLastBooking(ICriterion criterion, params Order[] orders)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(vAgency));
            if (criterion != null)
            {
                criteria.Add(criterion);
            }

            foreach (Order order in orders)
            {
                if (order != null)
                {
                    criteria.AddOrder(order);
                }
            }

            return criteria.List();
        }
        #endregion

        #region -- Customer --
        /// <summary>
        /// Lấy về các Customer của 1 booking
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        public IList<Customer> CustomerGetByBooking(Booking booking)
        {
            return booking.Customers;
        }

        public IList CustomerGetAllDistinct()
        {
            ISession session = _session.OpenSession();
            ICriteria criteria = session.CreateCriteria(typeof(Customer));
            criteria.SetProjection(Projections.Distinct(Projections.Property("Passport")));
            criteria.Add(Expression.Not(Expression.Eq("Passport", "")));
            // Lấy về danh sách passport
            IList passports = criteria.List();
            IList result = new ArrayList();
            foreach (string passport in passports)
            {
                ICriteria critPass = session.CreateCriteria(typeof(Customer));
                critPass.Add(Expression.Like("Passport", passport));
                IList list = critPass.List();
                if (list.Count > 0)
                {
                    result.Add(list[list.Count - 1]);
                }
            }
            return result;
        }

        public IList CustomerGetByCriterionPaged(ICriterion criterion, int pageSize, int pageIndex, out int count, params Order[] orders)
        {
            ISession session = _session.OpenSession();
            ICriteria criteria1 = session.CreateCriteria(typeof(Customer));
            criteria1.SetProjection(Projections.Distinct(Projections.Property("Code")));
            if (criterion != null)
            {
                criteria1.Add(criterion);
            }
            criteria1.SetProjection(Projections.RowCount());
            count = (int)criteria1.List()[0];

            ICriteria criteria = session.CreateCriteria(typeof(Customer));

            criteria.SetProjection(Projections.Distinct(Projections.Property("Code")));
            if (criterion != null)
            {
                criteria.Add(criterion);
            }

            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            if (pageSize > 0)
            {
                criteria.SetFirstResult(pageSize * pageIndex);
                criteria.SetMaxResults(pageSize);
            }

            foreach (Order order in orders)
            {
                if (order != null)
                {
                    criteria.AddOrder(order);
                }
            }

            IList passports = criteria.List();
            IList result = new ArrayList();
            foreach (string passport in passports)
            {
                ICriteria critPass = session.CreateCriteria(typeof(Customer));
                critPass.Add(Expression.Like("Code", passport));
                IList list = critPass.List();
                if (list.Count > 0)
                {
                    result.Add(list[list.Count - 1]);
                }
            }
            return result;
        }

        #endregion

        #region -- Extra Option --
        #endregion

        public void IncomeSum(DateTime from, DateTime to, out double income, out double receivable)
        {
            income = 0;
            receivable = 0;
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking));
            criteria.Add(LockCondition());
            criteria.Add(Expression.Ge("StartDate", from.Date));
            criteria.Add(Expression.Le("StartDate", to.Date));
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Sum("Total")).Add(Projections.Sum("Paid")));
            //criteria.SetProjection(Projections.Sum(Booking.TOTAL));
            //criteria.SetProjection(Projections.Sum(Booking.PAID));
            IList list = criteria.List();
            IList resultList = (IList)list[0];
            if (resultList[0] != null)
            {
                income = Convert.ToDouble(resultList[0]);
            }
            if (resultList[1] != null)
            {
                receivable = income - Convert.ToDouble(resultList[1]);
            }
            return;
        }

        public void OutcomeSum(DateTime from, DateTime to, out double outcome, out double payable)
        {
            outcome = 0;
            payable = 0;
            //ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(SailExpense));
            //criteria.CreateAlias("Payment", "payment");
            //criteria.Add(Expression.Ge(SailExpense.DATE, from.Date));
            //criteria.Add(Expression.Le(SailExpense.DATE, to.Date));
            ////criteria.SetProjection(Projections.ProjectionList().Add(Projections.Sum(SailExpense.TOTAL)).Add(Projections.Sum(Booking.PAID)));
            //IList expenses = criteria.List();
            //foreach (SailExpense expense in expenses)
            //{
            //    outcome += expense.TotalCost;
            //    if (expense.Payment != null)
            //    {
            //        payable += expense.Payment.TotalDebt;
            //    }
            //    else
            //    {
            //        payable += expense.TotalCost;
            //    }
            //}
        }

        public IList ExpenseServiceGet(Cruise cruise, DateTime? from, DateTime? to, CostType type, Agency agency)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(ExpenseService));

            criteria.CreateAlias("Expense", "expense");

            if (from.HasValue && to.HasValue)
            {
                if (to > DateTime.Today)
                {
                    to = DateTime.Today;
                }
                criteria.Add(Expression.And(Expression.Ge("expense.Date", from), Expression.Le("expense.Date", to)));
            }
            else
            {
                to = DateTime.Today;
                criteria.Add(Expression.Le("expense.Date", to));
            }
            if (cruise != null)
            {
                criteria.Add(Expression.Eq("expense.Cruise", cruise));
            }

            if (agency != null)
            {
                criteria.Add(Expression.Eq("Supplier", agency));
            }

            if (type != null)
            {
                criteria.Add(Expression.Eq("Type", type));
            }
            criteria.AddOrder(Order.Asc("expense.Date"));

            return criteria.List();
        }

        public int CustomerServiceCountByBooking(ExtraOption service, Booking booking)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(CustomerService));
            criteria.CreateAlias("Customer", "customer");
            criteria.Add(Expression.Eq("customer.Booking", booking));
            criteria.Add(Expression.Eq("IsExcluded", false));
            criteria.Add(Expression.Eq("Service", service));
            criteria.SetProjection(Projections.RowCount());
            IList result = criteria.List();
            return (int)result[0];
        }

        public int RunningDayCount(Cruise cruise, int year, int month)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(Booking));
            criteria.Add(Expression.Eq("Deleted", false));
            criteria.Add(Expression.Eq("IsTransferred", false));
            criteria.Add(Expression.Eq("Status", StatusType.Approved));

            if (cruise != null)
            {
                criteria.Add(Expression.Eq("Cruise", cruise));
            }
            DateTime from;
            DateTime to;
            if (month <= 0)
            {
                from = new DateTime(year, 1, 1);
                to = from.AddYears(1).AddDays(-1);
            }
            else
            {
                from = new DateTime(year, month, 1);
                to = from.AddMonths(1).AddDays(-1);
            }
            criteria.Add(Expression.Ge("StartDate", from));
            criteria.Add(Expression.Le("StartDate", to));
            criteria.SetProjection(Projections.CountDistinct("StartDate"));
            IList list = criteria.List();
            return (int)list[0];
        }

        public double SumBarByDate(DateTime date)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(BarRevenue));
            criteria.Add(Expression.Eq("Date", date));
            criteria.SetProjection(Projections.Sum("Revenue"));
            IList list = criteria.List();
            if (list[0] != null)
            {
                return (double)list[0];
            }
            return 0;
        }

        public IList FeedbackReport(ICriterion criterion)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(AnswerGroup));
            criteria.CreateAlias("AnswerSheet", "sheet");
            if (criterion != null)
            {
                criteria.Add(criterion);
            }
            criteria.AddOrder(Order.Desc("sheet.Date"));
            return criteria.List();
        }

        public int AnswerReport(ICriterion criterion)
        {
            ICriteria criteria = _session.OpenSession().CreateCriteria(typeof(AnswerOption));
            criteria.CreateAlias("AnswerSheet", "sheet");
            if (criterion != null)
            {
                criteria.Add(criterion);
            }
            criteria.SetProjection(Projections.RowCount());
            IList list = criteria.List();
            if (list.Count > 0 && list[0] != null)
            {
                return (int)list[0];
            }
            return 0;
            //criteria.AddOrder(Order.Asc("sheet.Date"));
            //return criteria.List();
        }

        #region -- NEW DAO --
        public IList<Booking> GetBookingShadow(DateTime date)
        {
            using (var session = _session.OpenSession())
            {
                var criteria = session.CreateCriteria(typeof(BookingHistory));
                criteria.SetProjection(Projections.Distinct(Projections.Property("Booking")));
                criteria.CreateAlias("Booking", "booking");

                var crit = Expression.Not(Expression.EqProperty("booking.StartDate", "StartDate"));// đã có sự thay đổi về startdate

                crit = Expression.Or(crit, Expression.And(Expression.Eq("Status", StatusType.Cancelled),
                                                           Expression.Eq("booking.Status", StatusType.Cancelled))); // hoặc đã bị hủy
                crit = Expression.And(crit, Expression.Not(Expression.Eq("booking.Deleted",true)));
                criteria.Add(crit);
                criteria.Add(Expression.Eq("StartDate", date)); // và ngày thay đổi đó nằm ở vị trí này trong lịch sử

                return criteria.List<Booking>();
            }
        }
        #endregion
    }
}