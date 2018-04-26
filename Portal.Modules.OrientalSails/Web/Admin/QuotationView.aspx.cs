using Portal.Modules.OrientalSails.BusinessLogic;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Enums.Quotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class QuotationView : System.Web.UI.Page
    {
        public QuotationViewBLL quotationViewBLL;
        public QuotationViewBLL QuotationViewBLL
        {
            get
            {
                if (quotationViewBLL == null)
                    quotationViewBLL = new QuotationViewBLL();
                return quotationViewBLL;
            }
        }
        public Quotation Quotation
        {
            get
            {
                Quotation quotation = null;
                try
                {
                    if (Request.QueryString["qi"] != null)
                        quotation = QuotationViewBLL.QuotationGetById(Convert.ToInt32(Request.QueryString["qi"]));
                }
                catch (Exception) { }
                return quotation;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var listQuotationPrice = Quotation.ListQuotationPrice;
            txtOs2d1nDouble.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails1, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtOs2d1nSingle.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails1, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtOs2d1nChildren6to11.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails1, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Children6to11, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtOs12d1nCharter.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails1, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtOs22d1nCharter1to4passenger.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails2, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._1to4passenger).ToString();
            txtOs22d1nCharter5to8passenger.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails2, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._5to8passenger).ToString();
            txtOs22d1nCharter9to12passenger.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails2, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._9to12passenger).ToString();
            txtOs22d1nCharter13to17passenger.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails2, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._13to17passenger).ToString();
            txtOs3d2nDouble.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails1, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtOs3d2nSingle.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails1, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtOs3d2nChildren6to11.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails1, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Children6to11, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtOs13d2nCharter.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails1, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtOs23d2nCharter1to4passenger.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails2, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._1to4passenger).ToString();
            txtOs23d2nCharter5to8passenger.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails2, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._5to8passenger).ToString();
            txtOs23d2nCharter9to12passenger.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails2, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._9to12passenger).ToString();
            txtOs23d2nCharter13to17passenger.Text = "$" + GetPrice((int)CruiseEnum.OrientalSails2, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._13to17passenger).ToString();
            txtCls2d1nDouble.Text = "$" + GetPrice((int)CruiseEnum.Calypso, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtCls2d1nSingle.Text = "$" + GetPrice((int)CruiseEnum.Calypso, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtCls2d1nChildren6to11.Text = "$" + GetPrice((int)CruiseEnum.Calypso, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Children6to11, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtCls2d1nCharter.Text = "$" + GetPrice((int)CruiseEnum.Calypso, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtCls3d2nDouble.Text = "$" + GetPrice((int)CruiseEnum.Calypso, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtCls3d2nSingle.Text = "$" + GetPrice((int)CruiseEnum.Calypso, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtCls3d2nChildren6to11.Text = "$" + GetPrice((int)CruiseEnum.Calypso, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Children6to11, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtCls3d2nCharter.Text = "$" + GetPrice((int)CruiseEnum.Calypso, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nDeluxeDouble.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nDeluxeSingle.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nDeluxeExtrabed.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Extrabed, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nExecutiveDouble.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Executive, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nExecutiveSingle.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Executive, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nExecutiveExtrabed.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Executive, (int)RoomTypeEnum.Extrabed, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nSuiteDouble.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Suite, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nSuiteSingle.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Suite, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nSuiteExtrabed.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Suite, (int)RoomTypeEnum.Extrabed, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl2d1nCharter1to40passenger.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._1to40passenger).ToString();
            txtStl2d1nCharter41to50passenger.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._41to50passenger).ToString();
            txtStl2d1nCharter51to63passenger.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._2Day1Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._51to63passenger).ToString();
            txtStl3d2nDeluxeDouble.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nDeluxeSingle.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nDeluxeExtrabed.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Deluxe, (int)RoomTypeEnum.Extrabed, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nExecutiveDouble.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Executive, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nExecutiveSingle.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Executive, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nExecutiveExtrabed.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Executive, (int)RoomTypeEnum.Extrabed, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nSuiteDouble.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Suite, (int)RoomTypeEnum.Double, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nSuiteSingle.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Suite, (int)RoomTypeEnum.Single, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nSuiteExtrabed.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Suite, (int)RoomTypeEnum.Extrabed, false, (int)NumberOfPassengerEnum.Unknow).ToString();
            txtStl3d2nCharter1to40passenger.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._1to40passenger).ToString();
            txtStl3d2nCharter41to50passenger.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._41to50passenger).ToString();
            txtStl3d2nCharter51to63passenger.Text = "$" + GetPrice((int)CruiseEnum.Starlight, (int)TripEnum._3Day2Night, (int)RoomClassEnum.Unknow, (int)RoomTypeEnum.Unknow, true, (int)NumberOfPassengerEnum._51to63passenger).ToString();
        }
        public string GetValidFrom()
        {
            var validFrom = "";
            if (Quotation.ValidFrom != null)
                validFrom = Quotation.ValidFrom.Value.ToString("dd/MM/yyyy");
            return validFrom;
        }
        public string GetValidTo()
        {
            var validTo = "";
            if (Quotation.ValidTo != null)
                validTo = Quotation.ValidTo.Value.ToString("dd/MM/yyyy");
            return validTo;
        }
        public double GetPrice(int cruiseId, int tripId, int roomClassId, int roomTypeId, bool isCharter, int numberOfPassenger)
        {
            var quotation = Quotation.ListQuotationPrice.Where(x => x.CruiseId == cruiseId
            && x.TripId == tripId
            && x.RoomClassId == roomClassId
            && x.RoomTypeId == roomTypeId
            && x.IsCharter == isCharter
            && x.NumberOfPassenger == numberOfPassenger).FirstOrDefault();

            if (quotation != null)
                return quotation.Price;
            else
                return 0.0;
        }
    }
}