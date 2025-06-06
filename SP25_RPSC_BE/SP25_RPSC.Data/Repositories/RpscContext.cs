﻿using System;
using System.Collections.Generic;
using SP25_RPSC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

    public virtual DbSet<BusinessImage> BusinessImages { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerContract> CustomerContracts { get; set; }

    public virtual DbSet<CustomerMoveOut> CustomerMoveOuts { get; set; }

    public virtual DbSet<CustomerRentRoomDetailRequest> CustomerRentRoomDetailRequests { get; set; }

    public virtual DbSet<CustomerRequest> CustomerRequests { get; set; }

    public virtual DbSet<ExtendCcontract> ExtendCcontracts { get; set; }

    public virtual DbSet<ExtendContractRequest> ExtendContractRequests { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<ImageRf> ImageRves { get; set; }

    public virtual DbSet<Landlord> Landlords { get; set; }

    public virtual DbSet<LandlordContract> LandlordContracts { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostRoom> PostRooms { get; set; }

    public virtual DbSet<PricePackage> PricePackages { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomAmentiesList> RoomAmentiesLists { get; set; }

    public virtual DbSet<RoomAmenty> RoomAmenties { get; set; }

    public virtual DbSet<RoomImage> RoomImages { get; set; }

    public virtual DbSet<RoomPrice> RoomPrices { get; set; }

    public virtual DbSet<RoomRentRequest> RoomRentRequests { get; set; }

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
            entity.HasKey(e => e.AddressId).HasName("PK__Address__091C2AFB8591DBAE");

            entity.ToTable("Address");

            entity.Property(e => e.AddressId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.District).HasMaxLength(255);
            entity.Property(e => e.HouseNumber).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(255);
        });

        modelBuilder.Entity<BusinessImage>(entity =>
        {
            entity.HasKey(e => e.BusinessImageId).HasName("PK__Business__C9D1A714F69B4966");

            entity.ToTable("BusinessImage");

            entity.Property(e => e.BusinessImageId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Landlord).WithMany(p => p.BusinessImages)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_BusinessImage_Landlord");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__Chat__A9FBE7C673655D85");

            entity.ToTable("Chat");

            entity.Property(e => e.ChatId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReceiverId).HasMaxLength(36);
            entity.Property(e => e.SenderId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Receiver).WithMany(p => p.ChatReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK_Receiver_User");

            entity.HasOne(d => d.Sender).WithMany(p => p.ChatSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_Sender_User");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D880ED711A");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.BudgetRange).HasMaxLength(50);
            entity.Property(e => e.CustomerType).HasMaxLength(50);
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
            entity.HasKey(e => e.ContractId).HasName("PK__Customer__C90D346995B60A41");

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
            entity.HasKey(e => e.Cmoid).HasName("PK__Customer__041DE2C315A6841F");

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
                .HasConstraintName("FK__CustomerM__RoomS__45BE5BA9");

            entity.HasOne(d => d.UserDeposite).WithMany(p => p.CustomerMoveOutUserDeposites)
                .HasForeignKey(d => d.UserDepositeId)
                .HasConstraintName("FK__CustomerM__UserD__44CA3770");

            entity.HasOne(d => d.UserMove).WithMany(p => p.CustomerMoveOutUserMoves)
                .HasForeignKey(d => d.UserMoveId)
                .HasConstraintName("FK__CustomerM__UserM__43D61337");
        });

        modelBuilder.Entity<CustomerRentRoomDetailRequest>(entity =>
        {
            entity.HasKey(e => e.CustomerRentRoomDetailRequestId).HasName("PK__Customer__F36BB7803DAD6EF9");

            entity.ToTable("CustomerRentRoomDetailRequest");

            entity.Property(e => e.CustomerRentRoomDetailRequestId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CustomerId).HasMaxLength(36);
            entity.Property(e => e.DateWantToRent).HasColumnType("datetime");
            entity.Property(e => e.RoomRentRequestsId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerRentRoomDetailRequests)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_CustomerRentRoomDetailRequest_Customer");

            entity.HasOne(d => d.RoomRentRequests).WithMany(p => p.CustomerRentRoomDetailRequests)
                .HasForeignKey(d => d.RoomRentRequestsId)
                .HasConstraintName("FK_CustomerRequest_RoomRentRequests");
        });

        modelBuilder.Entity<CustomerRequest>(entity =>
        {
            entity.HasKey(e => e.CustomerRequestId).HasName("PK__Customer__C3E40740ECF612C6");

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
            entity.HasKey(e => e.ExtendCcontractId).HasName("PK__ExtendCC__12D144FEDB4CC72E");

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

        modelBuilder.Entity<ExtendContractRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__ExtendCo__33A8517A1D2236B4");

            entity.ToTable("ExtendContractRequest");

            entity.Property(e => e.RequestId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.ContractId).HasMaxLength(36);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Contract).WithMany(p => p.ExtendContractRequests)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK_ECR_Contract");

            entity.HasOne(d => d.Landlord).WithMany(p => p.ExtendContractRequests)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_ECR_Landlord");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__CE74FAD5231F92F2");

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
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD69AC0CA4D");

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

        modelBuilder.Entity<ImageRf>(entity =>
        {
            entity.HasKey(e => e.ImageRfid).HasName("PK__ImageRF__AF73718C4B8208CD");

            entity.ToTable("ImageRF");

            entity.Property(e => e.ImageRfid)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ImageRFId");
            entity.Property(e => e.FeedbackId).HasMaxLength(36);
            entity.Property(e => e.ImageRfurl)
                .HasMaxLength(255)
                .HasColumnName("ImageRFUrl");
            entity.Property(e => e.ReportId).HasMaxLength(36);

            entity.HasOne(d => d.Feedback).WithMany(p => p.ImageRves)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK_ImageRF_Feedback");

            entity.HasOne(d => d.Report).WithMany(p => p.ImageRves)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK_ImageRF_Report");
        });

        modelBuilder.Entity<Landlord>(entity =>
        {
            entity.HasKey(e => e.LandlordId).HasName("PK__Landlord__8EC79DA33C636E1D");

            entity.ToTable("Landlord");

            entity.Property(e => e.LandlordId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.BankName).HasMaxLength(255);
            entity.Property(e => e.BankNumber).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Template).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(36);

            entity.HasOne(d => d.User).WithMany(p => p.Landlords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Landlord_User");
        });

        modelBuilder.Entity<LandlordContract>(entity =>
        {
            entity.HasKey(e => e.LcontractId).HasName("PK__Landlord__F0935AFA43796E5A");

            entity.Property(e => e.LcontractId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("LContractId");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.LcontractUrl).HasColumnName("LContractUrl");
            entity.Property(e => e.PackageId).HasMaxLength(36);
            entity.Property(e => e.ServiceDetailId).HasMaxLength(36);
            entity.Property(e => e.SignedDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Landlord).WithMany(p => p.LandlordContracts)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_LandlordContracts_Landlord");

            entity.HasOne(d => d.Package).WithMany(p => p.LandlordContracts)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_LandlordContracts_Package");

            entity.HasOne(d => d.ServiceDetail).WithMany(p => p.LandlordContracts)
                .HasForeignKey(d => d.ServiceDetailId)
                .HasConstraintName("FK_LandlordContracts_ServiceDetail");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12644D8397");

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
            entity.HasKey(e => e.Id).HasName("PK__Otp__3214EC07A984FB3B");

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
            entity.HasKey(e => e.PostId).HasName("PK__Post__AA126018ACAAD1EE");

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
            entity.Property(e => e.UserId).HasMaxLength(36);

            entity.HasOne(d => d.RentalRoom).WithMany(p => p.Posts)
                .HasForeignKey(d => d.RentalRoomId)
                .HasConstraintName("FK_Post_RentalRoom");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Post_User");
        });

        modelBuilder.Entity<PostRoom>(entity =>
        {
            entity.HasKey(e => e.PostRoomId).HasName("PK__PostRoom__EBF0C142779A5801");

            entity.ToTable("PostRoom");

            entity.Property(e => e.PostRoomId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpPost).HasColumnType("datetime");
            entity.Property(e => e.RoomId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Room).WithMany(p => p.PostRooms)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Post_Room");
        });

        modelBuilder.Entity<PricePackage>(entity =>
        {
            entity.HasKey(e => e.PriceId).HasName("PK__PricePac__49575BAFBD8029F4");

            entity.ToTable("PricePackage");

            entity.Property(e => e.PriceId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApplicableDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceDetailId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(36);

            entity.HasOne(d => d.ServiceDetail).WithMany(p => p.PricePackages)
                .HasForeignKey(d => d.ServiceDetailId)
                .HasConstraintName("FK_ServiceDetail");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E591ECD9BA6");

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
            entity.HasKey(e => e.ReportId).HasName("PK__Report__D5BD48057DEBEE63");

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
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A49B47C87");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName).HasMaxLength(255);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__32863939C5BB4435");

            entity.Property(e => e.RoomId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.AvailableDateToRent).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.RoomNumber).HasMaxLength(50);
            entity.Property(e => e.RoomTypeId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_Rooms_RoomType");
        });

        modelBuilder.Entity<RoomAmentiesList>(entity =>
        {
            entity.HasKey(e => e.RoomAmenitiesListId).HasName("PK__RoomAmen__CC945124FB91FD74");

            entity.ToTable("RoomAmentiesList");

            entity.Property(e => e.RoomAmenitiesListId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoomAmentyId).HasMaxLength(36);
            entity.Property(e => e.RoomId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.RoomAmenty).WithMany(p => p.RoomAmentiesLists)
                .HasForeignKey(d => d.RoomAmentyId)
                .HasConstraintName("FK_RoomAmentiesList_RoomAmenties");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomAmentiesLists)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RoomAmentiesList_Room");
        });

        modelBuilder.Entity<RoomAmenty>(entity =>
        {
            entity.HasKey(e => e.RoomAmentyId).HasName("PK__RoomAmen__9611FEA3C5C9CE2E");

            entity.Property(e => e.RoomAmentyId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.Compensation).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Landlord).WithMany(p => p.RoomAmenties)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_RoomAmenities_Landlord");
        });

        modelBuilder.Entity<RoomImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__RoomImag__7516F70CD4F2190C");

            entity.ToTable("RoomImage");

            entity.Property(e => e.ImageId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.RoomId).HasMaxLength(36);

            entity.HasOne(d => d.Room).WithMany(p => p.RoomImages)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RoomImage_Rooms");
        });

        modelBuilder.Entity<RoomPrice>(entity =>
        {
            entity.HasKey(e => e.RoomPriceId).HasName("PK__RoomPric__CA19798466F8142E");

            entity.ToTable("RoomPrice");

            entity.Property(e => e.RoomPriceId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApplicableDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RoomId).HasMaxLength(36);

            entity.HasOne(d => d.Room).WithMany(p => p.RoomPrices)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RoomPrice_Rooms");
        });

        modelBuilder.Entity<RoomRentRequest>(entity =>
        {
            entity.HasKey(e => e.RoomRentRequestsId).HasName("PK__RoomRent__EE7DFAC0E5839039");

            entity.Property(e => e.RoomRentRequestsId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoomId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Room).WithMany(p => p.RoomRentRequests)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RoomRentRequests_Rooms");
        });

        modelBuilder.Entity<RoomService>(entity =>
        {
            entity.HasKey(e => e.RoomServiceId).HasName("PK__RoomServ__A11E84C15CAE7420");

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
            entity.HasKey(e => e.RoomServicePriceId).HasName("PK__RoomServ__81684856A0E26EFE");

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
            entity.HasKey(e => e.RoomStayId).HasName("PK__RoomStay__B2D2F2DB57DF7412");

            entity.ToTable("RoomStay");

            entity.Property(e => e.RoomStayId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.RoomId).HasMaxLength(36);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
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
            entity.HasKey(e => e.RoomStayCustomerId).HasName("PK__RoomStay__C4276ED7EA7FBF2E");

            entity.ToTable("RoomStayCustomer");

            entity.Property(e => e.RoomStayCustomerId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.CustomerId).HasMaxLength(36);
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.RoomStayId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
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
            entity.HasKey(e => e.RoomTypeId).HasName("PK__RoomType__BCC89631159E0F54");

            entity.ToTable("RoomType");

            entity.Property(e => e.RoomTypeId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.AddressId).HasMaxLength(36);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Deposite).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LandlordId).HasMaxLength(36);
            entity.Property(e => e.RoomTypeName).HasMaxLength(255);
            entity.Property(e => e.Square).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasMaxLength(36);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Address).WithMany(p => p.RoomTypes)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_RoomType_Address");

            entity.HasOne(d => d.Landlord).WithMany(p => p.RoomTypes)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK_Rooms_Landlord");
        });

        modelBuilder.Entity<RoommateRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Roommate__33A8517A80740D8A");

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
            entity.HasKey(e => e.ServiceDetailId).HasName("PK__ServiceD__6F80950C9CCBEEE6");

            entity.ToTable("ServiceDetail");

            entity.Property(e => e.ServiceDetailId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.Duration).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PackageId).HasMaxLength(36);
            entity.Property(e => e.Status).HasMaxLength(36);

            entity.HasOne(d => d.Package).WithMany(p => p.ServiceDetails)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_ServicePackage");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__ServiceP__322035CC06BD0A02");

            entity.ToTable("ServicePackage");

            entity.Property(e => e.PackageId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.HighLightTime).HasMaxLength(255);
            entity.Property(e => e.Label).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(255);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6B88BF32B9");

            entity.ToTable("Transaction");

            entity.Property(e => e.TransactionId).HasMaxLength(36);
            entity.Property(e => e.LcontractId)
                .HasMaxLength(36)
                .HasColumnName("LContractId");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Type).HasMaxLength(36);

            entity.HasOne(d => d.Lcontract).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.LcontractId)
                .HasConstraintName("FK_Transaction_LandlordContracts");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C168D36EF");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105346112EDD6").IsUnique();

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
