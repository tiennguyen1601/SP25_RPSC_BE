using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SP25_RPSC.Data.Entities;

namespace SP25_RPSC.Data.Repositories;

public partial class RpscContext : DbContext
{
    public RpscContext()
    {
    }

    public RpscContext(DbContextOptions<RpscContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerContract> CustomerContracts { get; set; }

    public virtual DbSet<CustomerMoveOut> CustomerMoveOuts { get; set; }

    public virtual DbSet<CustomerRequest> CustomerRequests { get; set; }

    public virtual DbSet<ExtendCcontract> ExtendCcontracts { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Landlord> Landlords { get; set; }

    public virtual DbSet<LandlordContract> LandlordContracts { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PricePackage> PricePackages { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomAmenty> RoomAmenties { get; set; }

    public virtual DbSet<RoomImage> RoomImages { get; set; }

    public virtual DbSet<RoomPrice> RoomPrices { get; set; }

    public virtual DbSet<RoomService> RoomServices { get; set; }

    public virtual DbSet<RoomServicePrice> RoomServicePrices { get; set; }

    public virtual DbSet<RoomStay> RoomStays { get; set; }

    public virtual DbSet<RoomStayCustomer> RoomStayCustomers { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<RoommateRequest> RoommateRequests { get; set; }

    public virtual DbSet<ServiceDetail> ServiceDetails { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(GetConnectionString());

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnectionString"];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Address__091C2AFB6B644C48");

            entity.ToTable("Address");

            entity.Property(e => e.AddressId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.District).HasMaxLength(255);
            entity.Property(e => e.HouseNumber).HasMaxLength(50);
            entity.Property(e => e.RoomId).HasMaxLength(36);
            entity.Property(e => e.Street).HasMaxLength(255);

            entity.HasOne(d => d.Room).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Address_Room");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D877A3010C");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.BudgetRange).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.LifeStyle).HasMaxLength(255);
            entity.Property(e => e.PreferredLocation).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasMaxLength(36);

            entity.HasOne(d => d.User).WithMany(p => p.Customers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Customer_User");
        });

        modelBuilder.Entity<CustomerContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Customer__C90D3469F45906D1");

            entity.Property(e => e.ContractId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.RentalRoomId).HasMaxLength(36);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.Term).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.RentalRoom).WithMany(p => p.CustomerContracts)
                .HasForeignKey(d => d.RentalRoomId)
                .HasConstraintName("FK_CustomerContracts_RentalRoom");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CustomerContracts)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_CustomerContracts_Tenant");
        });

        modelBuilder.Entity<CustomerMoveOut>(entity =>
        {
            entity.HasKey(e => e.Cmoid).HasName("PK__Customer__041DE2C3F23A30D1");

            entity.ToTable("CustomerMoveOut");

            entity.Property(e => e.Cmoid)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("CMOId");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.RoomStayId).HasMaxLength(36);
            entity.Property(e => e.UserDepositeId).HasMaxLength(36);
            entity.Property(e => e.UserMoveId).HasMaxLength(36);

            entity.HasOne(d => d.RoomStay).WithMany(p => p.CustomerMoveOuts)
                .HasForeignKey(d => d.RoomStayId)
                .HasConstraintName("FK__CustomerM__RoomS__3587F3E0");

            entity.HasOne(d => d.UserDeposite).WithMany(p => p.CustomerMoveOutUserDeposites)
                .HasForeignKey(d => d.UserDepositeId)
                .HasConstraintName("FK__CustomerM__UserD__3493CFA7");

            entity.HasOne(d => d.UserMove).WithMany(p => p.CustomerMoveOutUserMoves)
                .HasForeignKey(d => d.UserMoveId)
                .HasConstraintName("FK__CustomerM__UserM__339FAB6E");
        });

        modelBuilder.Entity<CustomerRequest>(entity =>
        {
            entity.HasKey(e => e.CustomerRequestId).HasName("PK__Customer__C3E40740B7233985");

            entity.ToTable("CustomerRequest");

            entity.Property(e => e.CustomerRequestId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CustomerId).HasMaxLength(36);
            entity.Property(e => e.RequestId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerRequests)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_CustomerRequest_Customer");

            entity.HasOne(d => d.Request).WithMany(p => p.CustomerRequests)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_CustomerRequest_RoommateRequests");
        });

        modelBuilder.Entity<ExtendCcontract>(entity =>
        {
            entity.HasKey(e => e.ExtendCcontractId).HasName("PK__ExtendCC__12D144FE929385EB");

            entity.ToTable("ExtendCContracts");

            entity.Property(e => e.ExtendCcontractId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ExtendCContractID");
            entity.Property(e => e.ContractId)
                .HasMaxLength(36)
                .HasColumnName("ContractID");
            entity.Property(e => e.EndDateContract).HasColumnType("datetime");
            entity.Property(e => e.StartDateContract).HasColumnType("datetime");

            entity.HasOne(d => d.Contract).WithMany(p => p.ExtendCcontracts)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK_CustomerContracts");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__CE74FAD57D3EA23C");

            entity.ToTable("Favorite");

            entity.Property(e => e.FavoriteId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasMaxLength(36);
            entity.Property(e => e.RoomId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Favorite_Customer");

            entity.HasOne(d => d.Room).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Favorite_Room");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD6D754A768");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RentalRoomId).HasMaxLength(36);
            entity.Property(e => e.RevieweeId).HasMaxLength(36);
            entity.Property(e => e.ReviewerId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.RentalRoom).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.RentalRoomId)
                .HasConstraintName("FK_Feedback_RentalRoom");

            entity.HasOne(d => d.Reviewee).WithMany(p => p.FeedbackReviewees)
                .HasForeignKey(d => d.RevieweeId)
                .HasConstraintName("FK_Feedback_Reviewee");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.FeedbackReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .HasConstraintName("FK_Feedback_Reviewer");
        });

        modelBuilder.Entity<Landlord>(entity =>
        {
            entity.HasKey(e => e.LandlordId).HasName("PK__Landlord__8EC79DA3452D95F5");

            entity.ToTable("Landlord");

            entity.Property(e => e.LandlordId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.BusinessLiscense).HasMaxLength(50);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LiscenseNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(36);

            entity.HasOne(d => d.User).WithMany(p => p.Landlords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Landlord_User");
        });

        modelBuilder.Entity<LandlordContract>(entity =>
        {
            entity.HasKey(e => e.LcontractId).HasName("PK__Landlord__F0935AFA9C0A1792");

            entity.Property(e => e.LcontractId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("LContractId");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.PackageId).HasMaxLength(36);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Term).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Landlord).WithMany(p => p.LandlordContracts)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_LandlordContracts_Landlord");

            entity.HasOne(d => d.Package).WithMany(p => p.LandlordContracts)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_LandlordContracts_Package");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12E5B34670");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(36);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Otp__3214EC07D9610B45");

            entity.ToTable("Otp");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(36);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Otps)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Otp_CreatedByNavigation");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Post__AA12601856D9EDCC");

            entity.ToTable("Post");

            entity.Property(e => e.PostId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RentalRoomId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.RentalRoom).WithMany(p => p.Posts)
                .HasForeignKey(d => d.RentalRoomId)
                .HasConstraintName("FK_Post_RentalRoom");
        });

        modelBuilder.Entity<PricePackage>(entity =>
        {
            entity.HasKey(e => e.PriceId).HasName("PK__PricePac__49575BAF4D5238EF");

            entity.ToTable("PricePackage");

            entity.Property(e => e.PriceId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApplicableDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceDetailId).HasMaxLength(36);

            entity.HasOne(d => d.ServiceDetail).WithMany(p => p.PricePackages)
                .HasForeignKey(d => d.ServiceDetailId)
                .HasConstraintName("FK_ServiceDetail");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E599CB295A1");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.RefreshTokenId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("RefreshTokenID");
            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasMaxLength(36);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_User");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Report__D5BD4805138DEEC0");

            entity.ToTable("Report");

            entity.Property(e => e.ReportId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RentalRoomId).HasMaxLength(36);
            entity.Property(e => e.ReportedUserId).HasMaxLength(36);
            entity.Property(e => e.ReporterId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.RentalRoom).WithMany(p => p.Reports)
                .HasForeignKey(d => d.RentalRoomId)
                .HasConstraintName("FK_Report_RentalRoom");

            entity.HasOne(d => d.ReportedUser).WithMany(p => p.ReportReportedUsers)
                .HasForeignKey(d => d.ReportedUserId)
                .HasConstraintName("FK_Report_ReportedUser");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ReportReporters)
                .HasForeignKey(d => d.ReporterId)
                .HasConstraintName("FK_Report_Reporter");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A351372D9");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName).HasMaxLength(255);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__328639392EDE2925");

            entity.Property(e => e.RoomId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.Deposite).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Landlord).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_Rooms_Landlord");
        });

        modelBuilder.Entity<RoomAmenty>(entity =>
        {
            entity.HasKey(e => e.RoomAmentyId).HasName("PK__RoomAmen__9611FEA3F1D02B2E");

            entity.Property(e => e.RoomAmentyId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.Compensation).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.RoomTypeId).HasMaxLength(36);

            entity.HasOne(d => d.RoomType).WithMany(p => p.RoomAmenties)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_RoomAmenities_RoomType");
        });

        modelBuilder.Entity<RoomImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__RoomImag__7516F70C4C3C32CA");

            entity.ToTable("RoomImage");

            entity.Property(e => e.ImageId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.RoomTypeId).HasMaxLength(36);

            entity.HasOne(d => d.RoomType).WithMany(p => p.RoomImages)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_RoomImage_RoomType");
        });

        modelBuilder.Entity<RoomPrice>(entity =>
        {
            entity.HasKey(e => e.RoomPriceId).HasName("PK__RoomPric__CA19798437F4A021");

            entity.ToTable("RoomPrice");

            entity.Property(e => e.RoomPriceId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApplicableDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RoomTypeId).HasMaxLength(36);

            entity.HasOne(d => d.RoomType).WithMany(p => p.RoomPrices)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_RoomPrice_RoomType");
        });

        modelBuilder.Entity<RoomService>(entity =>
        {
            entity.HasKey(e => e.RoomServiceId).HasName("PK__RoomServ__A11E84C120956D0D");

            entity.ToTable("RoomService");

            entity.Property(e => e.RoomServiceId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoomServiceName).HasMaxLength(255);
            entity.Property(e => e.RoomTypeId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.RoomType).WithMany(p => p.RoomServices)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_RoomService_RoomType");
        });

        modelBuilder.Entity<RoomServicePrice>(entity =>
        {
            entity.HasKey(e => e.RoomServicePriceId).HasName("PK__RoomServ__81684856E3F992EC");

            entity.ToTable("RoomServicePrice");

            entity.Property(e => e.RoomServicePriceId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApplicableDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RoomServiceId).HasMaxLength(36);

            entity.HasOne(d => d.RoomService).WithMany(p => p.RoomServicePrices)
                .HasForeignKey(d => d.RoomServiceId)
                .HasConstraintName("FK_RoomServicePrice_RoomService");
        });

        modelBuilder.Entity<RoomStay>(entity =>
        {
            entity.HasKey(e => e.RoomStayId).HasName("PK__RoomStay__B2D2F2DB3C47F29C");

            entity.ToTable("RoomStay");

            entity.Property(e => e.RoomStayId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.RoomId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Landlord).WithMany(p => p.RoomStays)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_RoomStay_Landlord");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomStays)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RoomStay_Room");
        });

        modelBuilder.Entity<RoomStayCustomer>(entity =>
        {
            entity.HasKey(e => e.RoomStayCustomerId).HasName("PK__RoomStay__C4276ED7207E2A1D");

            entity.ToTable("RoomStayCustomer");

            entity.Property(e => e.RoomStayCustomerId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CustomerId).HasMaxLength(36);
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.RoomStayId).HasMaxLength(36);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.RoomStayCustomers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_RoomStayCustomer_Customer");

            entity.HasOne(d => d.Landlord).WithMany(p => p.RoomStayCustomers)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_RoomStayCustomer_Landlord");

            entity.HasOne(d => d.RoomStay).WithMany(p => p.RoomStayCustomers)
                .HasForeignKey(d => d.RoomStayId)
                .HasConstraintName("FK_RoomStayCustomer_RoomStay");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__RoomType__BCC896310AF18BFC");

            entity.ToTable("RoomType");

            entity.Property(e => e.RoomTypeId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoomId).HasMaxLength(36);
            entity.Property(e => e.RoomTypeName).HasMaxLength(255);
            entity.Property(e => e.Square).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomTypes)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RoomType_Room");
        });

        modelBuilder.Entity<RoommateRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Roommate__33A8517A8CB9B4AE");

            entity.Property(e => e.RequestId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Post).WithMany(p => p.RoommateRequests)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_RoommateRequests_Post");
        });

        modelBuilder.Entity<ServiceDetail>(entity =>
        {
            entity.HasKey(e => e.ServiceDetailId).HasName("PK__ServiceD__6F80950C02302E41");

            entity.ToTable("ServiceDetail");

            entity.Property(e => e.ServiceDetailId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.PackageId).HasMaxLength(36);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasMaxLength(36);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Package).WithMany(p => p.ServiceDetails)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_ServicePackage");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__ServiceP__322035CC690AD134");

            entity.ToTable("ServicePackage");

            entity.Property(e => e.PackageId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6B47C17E9B");

            entity.ToTable("Transaction");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EndDateContract).HasColumnType("datetime");
            entity.Property(e => e.LcontractId)
                .HasMaxLength(36)
                .HasColumnName("LContractId");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentId).HasMaxLength(255);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.StartDateContract).HasColumnType("datetime");

            entity.HasOne(d => d.Lcontract).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.LcontractId)
                .HasConstraintName("FK_Transaction_LandlordContracts");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CE0CF59EE");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105347AE4FBBB").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.RoleId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
