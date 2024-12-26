using NorthwindDAL.Views.StoredProceduresRes;

namespace NorthwindDAL.Views
{
    public class OrderStatisticsView
    {
        public List<CustOrderHist> CustOrderHistRes { get; set; } = new();
        public List<CustOrdersDetail> CustOrdersDetailRes { get; set; } = new();
    }
}
