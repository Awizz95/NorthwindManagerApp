using NorthwindDAL.Models;

namespace NorthwindDAL.Infrastructure
{
    public static class ExtensionMessage
    {
        public static string ORDER_NOT_FOUND = "Order was not found.";
        public static string CANNOT_UPDATE_INPROGRESS_OR_COMPLETED = $"Cannot update orders with status \"{OrderStatus.InProgress}\" or \"{OrderStatus.Completed}\".";
        public static string UNKNOWN_STATUS = "Unknown order status.";
        public static string CANNOT_DELETE_COMPLETED = $"Cannot delete order with status \"{OrderStatus.Completed}\".";
    }
}
