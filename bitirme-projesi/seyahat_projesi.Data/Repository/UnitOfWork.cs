using seyahat_projesi.Data.Repository.IRepository;
using System;

namespace seyahat_projesi.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Guide = new GuideRepository(_context);
            Tour = new TourRepository(_context);
            Booking = new BookingRepository(_context);
            Payment = new PaymentRepository(_context);
            Coupon = new CouponRepository(_context);
            Review = new ReviewRepository(_context);
            Blog = new BlogRepository(_context);
            ContactMessage = new ContactMessageRepository(_context);
            Faq = new FaqRepository(_context);
            ChatMessage = new ChatMessageRepository(_context);
            SystemLog = new SystemLogRepository(_context);
        }

        public ICategoryRepository Category { get; private set; }
        public IGuideRepository Guide { get; private set; }
        public ITourRepository Tour { get; private set; }
        public IBookingRepository Booking { get; private set; }
        public IPaymentRepository Payment { get; private set; }
        public ICouponRepository Coupon { get; private set; }
        public IReviewRepository Review { get; private set; }
        public IBlogRepository Blog { get; private set; }
        public IContactMessageRepository ContactMessage { get; private set; }
        public IFaqRepository Faq { get; private set; }
        public IChatMessageRepository ChatMessage { get; private set; }
        public ISystemLogRepository SystemLog { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
