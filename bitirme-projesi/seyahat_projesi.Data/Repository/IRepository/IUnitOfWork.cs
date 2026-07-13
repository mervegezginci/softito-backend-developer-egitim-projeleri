using System;

namespace seyahat_projesi.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IGuideRepository Guide { get; }
        ITourRepository Tour { get; }
        IBookingRepository Booking { get; }
        IPaymentRepository Payment { get; }
        ICouponRepository Coupon { get; }
        IReviewRepository Review { get; }
        IBlogRepository Blog { get; }
        IContactMessageRepository ContactMessage { get; }
        IFaqRepository Faq { get; }
        IChatMessageRepository ChatMessage { get; }
        ISystemLogRepository SystemLog { get; }

        void Save();
    }
}
