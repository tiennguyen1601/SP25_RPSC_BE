﻿﻿-- Tạo database RPSC và chuyển sang database đó
CREATE DATABASE RPSC;
GO

USE RPSC;
GO

--Bảng Role
CREATE TABLE Role (
    RoleId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    RoleName NVARCHAR(255) NOT NULL
);
GO

--Bảng ServicePackage
CREATE TABLE ServicePackage (
    PackageId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    Duration INT,
    Description NVARCHAR(MAX),
    Status NVARCHAR(50)
);
GO

--Bảng User
CREATE TABLE [User] (
    UserId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    FullName NVARCHAR(255),
    Dob DATE,
    Address NVARCHAR(255),
    PhoneNumber NVARCHAR(50),
    Gender NVARCHAR(50),
    Avatar NVARCHAR(255),
    Password NVARCHAR(255),
    Status NVARCHAR(50),
    CreateAt DATETIME DEFAULT GETDATE(),
    UpdateAt DATETIME,
    RoleId NVARCHAR(36),
    CONSTRAINT FK_User_Role FOREIGN KEY (RoleId) REFERENCES Role(RoleId)
);
GO

--Bảng Landlord
CREATE TABLE Landlord (
    LandlordId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    CompanyName NVARCHAR(255),
    NumberRoom INT,
    LiscenseNumber NVARCHAR(50),
    BusinessLiscense NVARCHAR(50),
    Status NVARCHAR(50),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    UserId NVARCHAR(36),
    CONSTRAINT FK_Landlord_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);
GO

--Bảng Customer
CREATE TABLE Customer (
    CustomerId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Preferences NVARCHAR(MAX),
    LifeStyle NVARCHAR(255),
    BudgetRange NVARCHAR(50),
    PreferredLocation NVARCHAR(255),
    Requirement NVARCHAR(MAX),
    Status NVARCHAR(50),
    UserId NVARCHAR(36),
    CONSTRAINT FK_Customer_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);
GO
CREATE TABLE RoomType (
    RoomTypeId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    RoomTypeName NVARCHAR(255),
	Deposite DECIMAL(18,2),
    Square DECIMAL(10,2),
    Description NVARCHAR(MAX),
    MaxOccupancy INT,
	Status NVARCHAR(36),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
);
GO
--Bảng Rooms
CREATE TABLE Rooms (
    RoomId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
	RoomNumber NVARCHAR(50),
    Title NVARCHAR(255),
    Description NVARCHAR(MAX),
    Status NVARCHAR(50),
    Location NVARCHAR(255),
    UpdatedAt DATETIME,
    LandlordId NVARCHAR(36),
	RoomTypeId NVARCHAR(36),
    CONSTRAINT FK_Rooms_RoomType FOREIGN KEY (RoomTypeId) REFERENCES RoomType(RoomTypeId),
	CONSTRAINT FK_Rooms_Landlord FOREIGN KEY (LandlordId) REFERENCES Landlord(LandlordId)
);
GO

--Bảng Post
CREATE TABLE Post (
    PostId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(255),
    Descripton NVARCHAR(MAX),
    Price DECIMAL(18,2),
    Status NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    RentalRoomId NVARCHAR(36),
    CONSTRAINT FK_Post_RentalRoom FOREIGN KEY (RentalRoomId) REFERENCES Rooms(RoomId)
);
GO

--Bảng RoommateRequests
CREATE TABLE RoommateRequests (
    RequestId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Message NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50), -- Ví dụ: Pending, Accepted, Rejected
    PostId NVARCHAR(36),
    CONSTRAINT FK_RoommateRequests_Post FOREIGN KEY (PostId) REFERENCES Post(PostId)
);
GO

--Bảng CustomerRequest
CREATE TABLE CustomerRequest (
    CustomerRequestId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Status NVARCHAR(50),
    RequestId NVARCHAR(36),
    CustomerId NVARCHAR(36),
    CONSTRAINT FK_CustomerRequest_RoommateRequests FOREIGN KEY (RequestId) REFERENCES RoommateRequests(RequestId),
    CONSTRAINT FK_CustomerRequest_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId)
);
GO

--Bảng CustomerContracts
CREATE TABLE CustomerContracts (
    ContractId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    StartDate DATETIME,
    EndDate DATETIME,
    Status NVARCHAR(50),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    Term NVARCHAR(50),
    TenantId NVARCHAR(36),
    RentalRoomId NVARCHAR(36),
    CONSTRAINT FK_CustomerContracts_Tenant FOREIGN KEY (TenantId) REFERENCES Customer(CustomerId),
    CONSTRAINT FK_CustomerContracts_RentalRoom FOREIGN KEY (RentalRoomId) REFERENCES Rooms(RoomId)
);
GO



--Bảng Report
CREATE TABLE Report (
    ReportId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Description NVARCHAR(MAX),
    Type NVARCHAR(50), -- Ví dụ: Room, ReportedUser
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    Status NVARCHAR(50),
    ReporterId NVARCHAR(36),
    ReportedUserId NVARCHAR(36),
    RentalRoomId NVARCHAR(36),
    CONSTRAINT FK_Report_Reporter FOREIGN KEY (ReporterId) REFERENCES [User](UserId),
    CONSTRAINT FK_Report_ReportedUser FOREIGN KEY (ReportedUserId) REFERENCES [User](UserId),
    CONSTRAINT FK_Report_RentalRoom FOREIGN KEY (RentalRoomId) REFERENCES Rooms(RoomId)
);
GO

--Bảng Notification
CREATE TABLE Notification (
    NotificationId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Message NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UserId NVARCHAR(36),
    CONSTRAINT FK_Notification_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);
GO

--Bảng LandlordContracts
CREATE TABLE LandlordContracts (
    LContractId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    StartDate DATETIME,
    EndDate DATETIME,
    Status NVARCHAR(50),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    Term NVARCHAR(50),
    PackageId NVARCHAR(36),
    LandlordId NVARCHAR(36),
    CONSTRAINT FK_LandlordContracts_Package FOREIGN KEY (PackageId) REFERENCES ServicePackage(PackageId),
    CONSTRAINT FK_LandlordContracts_Landlord FOREIGN KEY (LandlordId) REFERENCES Landlord(LandlordId)
);
GO

-- Bảng Transaction
CREATE TABLE [Transaction] (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    Amount DECIMAL(18,2),
    StartDateContract DATETIME,
    EndDateContract DATETIME,
    PaymentDate DATETIME,
    PaymentId NVARCHAR(255),
    PaymentMethod NVARCHAR(50) CHECK (PaymentMethod IN ('Online', 'Cash')),
    LContractId NVARCHAR(36),
    CONSTRAINT FK_Transaction_LandlordContracts FOREIGN KEY (LContractId) REFERENCES LandlordContracts(LContractId)
);

GO
--Bảng Feedback
CREATE TABLE Feedback (
    FeedbackId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Description NVARCHAR(MAX),
    Type NVARCHAR(50), -- Ví dụ: Suggestion, Complaint, Compliment
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    Status NVARCHAR(50),
    Rating INT,
    ReviewerId NVARCHAR(36),
    RevieweeId NVARCHAR(36),
    RentalRoomId NVARCHAR(36),
    CONSTRAINT FK_Feedback_Reviewer FOREIGN KEY (ReviewerId) REFERENCES [User](UserId),
    CONSTRAINT FK_Feedback_Reviewee FOREIGN KEY (RevieweeId) REFERENCES [User](UserId),
    CONSTRAINT FK_Feedback_RentalRoom FOREIGN KEY (RentalRoomId) REFERENCES Rooms(RoomId)
);
GO

--Bảng Favorite
CREATE TABLE Favorite (
    FavoriteId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    CreateDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50),
    RoomId NVARCHAR(36),
    CustomerId NVARCHAR(36),
    CONSTRAINT FK_Favorite_Room FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId),
    CONSTRAINT FK_Favorite_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId)
);
GO

--Bảng ServiceDetail
CREATE TABLE ServiceDetail (
    ServiceDetailId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    [Type] NVARCHAR(50),
    LimitPost INT,
	Status NVARCHAR(36),
	PackageId  NVARCHAR(36),
    CONSTRAINT FK_ServicePackage FOREIGN KEY (PackageId) REFERENCES ServicePackage(PackageId)
);
GO

--Bảng PricePackage
CREATE TABLE PricePackage (
    PriceId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
	Price DECIMAL(18,2),
    ApplicableDate DATETIME,
    ServiceDetailId NVARCHAR(36),
    CONSTRAINT FK_ServiceDetail FOREIGN KEY (ServiceDetailId) REFERENCES ServiceDetail(ServiceDetailId)
);
GO

--Bảng Address
CREATE TABLE Address (
    AddressId NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    City NVARCHAR(255),
    District NVARCHAR(255),
    Street NVARCHAR(255),
    HouseNumber NVARCHAR(50),
    [Long] FLOAT,  -- Lưu ý: "Long" có thể là từ khóa nên được đặt trong dấu []
    Lat FLOAT,
    RoomId NVARCHAR(36),
    CONSTRAINT FK_Address_Room FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId)
);
GO


--Bảng RoomImage
CREATE TABLE RoomImage (
    ImageId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    ImageUrl NVARCHAR(255),
    RoomTypeId NVARCHAR(36),
    CONSTRAINT FK_RoomImage_RoomType FOREIGN KEY (RoomTypeId) REFERENCES RoomType(RoomTypeId)
);
GO

--Bảng RoomPrice
CREATE TABLE RoomPrice (
    RoomPriceId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    ApplicableDate DATETIME,
    Price DECIMAL(18,2),
    RoomTypeId NVARCHAR(36),
    CONSTRAINT FK_RoomPrice_RoomType FOREIGN KEY (RoomTypeId) REFERENCES RoomType(RoomTypeId)
);
GO

--Bảng RoomService
CREATE TABLE RoomService (
    RoomServiceId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    RoomServiceName NVARCHAR(255),
    Description NVARCHAR(MAX),
    Status NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    RoomTypeId NVARCHAR(36),
    CONSTRAINT FK_RoomService_RoomType FOREIGN KEY (RoomTypeId) REFERENCES RoomType(RoomTypeId)
);
GO

--Bảng RoomServicePrice
CREATE TABLE RoomServicePrice (
    RoomServicePriceId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    ApplicableDate DATETIME,
    Price DECIMAL(18,2),
    RoomServiceId NVARCHAR(36),
    CONSTRAINT FK_RoomServicePrice_RoomService FOREIGN KEY (RoomServiceId) REFERENCES RoomService(RoomServiceId)
);
GO

--Bảng RoomStay
CREATE TABLE RoomStay (
    RoomStayId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    RoomId NVARCHAR(36),
    LandlordId NVARCHAR(36),
    Status NVARCHAR(50),
    UpdatedAt DATETIME,
    CONSTRAINT FK_RoomStay_Room FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId),
    CONSTRAINT FK_RoomStay_Landlord FOREIGN KEY (LandlordId) REFERENCES Landlord(LandlordId)
);
GO

--Bảng RoomStayCustomer
CREATE TABLE RoomStayCustomer (
    RoomStayCustomerId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    RoomStayId NVARCHAR(36),
    CustomerId NVARCHAR(36),
    UpdatedAt DATETIME,
    LandlordId NVARCHAR(36),
    CONSTRAINT FK_RoomStayCustomer_RoomStay FOREIGN KEY (RoomStayId) REFERENCES RoomStay(RoomStayId),
    CONSTRAINT FK_RoomStayCustomer_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
    CONSTRAINT FK_RoomStayCustomer_Landlord FOREIGN KEY (LandlordId) REFERENCES Landlord(LandlordId)
);
GO

--Bảng RoomAmenties
CREATE TABLE RoomAmenties (
    RoomAmentyId NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255),
    Compensation DECIMAL(18,2),
    LandlordId NVARCHAR(36),
	CONSTRAINT FK_RoomAmenities_Landlord FOREIGN KEY (LandlordId) REFERENCES Landlord(LandlordId)
);
GO
CREATE TABLE RoomAmentiesList (
    RoomAmenitiesListId NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Description NVARCHAR(MAX),
    RoomId NVARCHAR(36),
	RoomAmentyId NVARCHAR(36),
	CONSTRAINT FK_RoomAmentiesList_Room FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId),
	CONSTRAINT FK_RoomAmentiesList_RoomAmenties FOREIGN KEY (RoomAmentyId) REFERENCES RoomAmenties(RoomAmentyId)
);
GO
--Bảng CustomerMoveOut
CREATE TABLE CustomerMoveOut (
    CMOId NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    UserMoveId NVARCHAR(36) FOREIGN KEY REFERENCES [User](UserId),
    UserDepositeId NVARCHAR(36) FOREIGN KEY REFERENCES [User](UserId),
    RoomStayId NVARCHAR(36) FOREIGN KEY REFERENCES RoomStay(RoomStayId),
    DateRequest DATETIME,
    Status INT
);
--Bảng OTP
CREATE TABLE Otp (
    Id NVARCHAR(36) PRIMARY KEY DEFAULT NEWID() NOT NULL , 
    Code NVARCHAR(MAX) NULL, 
    CreatedAt DATETIME NULL, 
    IsUsed BIT NULL,               
    CreatedBy NVARCHAR(36) NULL,
	CONSTRAINT FK_Otp_CreatedByNavigation FOREIGN KEY (CreatedBy) REFERENCES [User](UserId)
);
--Bảng RefreshToken
CREATE TABLE RefreshToken (
   RefreshTokenID NVARCHAR(36) PRIMARY KEY DEFAULT NEWID() NOT NULL ,
   ExpiredAt Datetime NOT NULL,
   Token VARCHAR (255) NOT NULL,
   UserId NVARCHAR(36),
   CONSTRAINT FK_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);
--Bảng ExtendCContracts
CREATE TABLE ExtendCContracts (
   ExtendCContractID NVARCHAR(36) PRIMARY KEY DEFAULT NEWID() NOT NULL ,
   StartDateContract Datetime NOT NULL,
   EndDateContract Datetime  NOT NULL,
   ExtendCount INT,
   ContractID  NVARCHAR(36),
   CONSTRAINT FK_CustomerContracts FOREIGN KEY (ContractID) REFERENCES [CustomerContracts](ContractID)
);
INSERT INTO Role (RoleId, RoleName)
VALUES 
(NEWID(), 'Customer'),
(NEWID(), 'Landlord'),
(NEWID(), 'Admin');

INSERT INTO [RPSC].[dbo].[ServicePackage] ([Name], [Duration], [Description], [Status])
VALUES
    (N'1 tuần', 7, N'Gói dịch vụ 1 tuần', 1),
    (N'1 tháng', 30, N'Gói dịch vụ 1 tháng', 1),
    (N'3 tháng', 90, N'Gói dịch vụ 3 tháng', 1),
    (N'6 tháng', 180, N'Gói dịch vụ 6 tháng', 1),
    (N'1 năm', 365, N'Gói dịch vụ 1 năm', 1);


	INSERT INTO [RPSC].[dbo].[ServiceDetail] ([Type], [LimitPost], [Status], [PackageId])
VALUES
    (N'Loại 1', 15, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tuần')),
    (N'Loại 2', 20, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tuần')),
    (N'Loại 3', 30, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tuần')),
    (N'Không giới hạn', NULL, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tuần')),

    (N'Loại 1', 15, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tháng')),
    (N'Loại 2', 20, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tháng')),
    (N'Loại 3', 30, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tháng')),
    (N'Không giới hạn', NULL, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tháng')),

    (N'Loại 1', 15, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'3 tháng')),
    (N'Loại 2', 20, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'3 tháng')),
    (N'Loại 3', 30, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'3 tháng')),
    (N'Không giới hạn', NULL, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'3 tháng')),

    (N'Loại 1', 15, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'6 tháng')),
    (N'Loại 2', 20, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'6 tháng')),
    (N'Loại 3', 30, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'6 tháng')),
    (N'Không giới hạn', NULL, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'6 tháng')),

    (N'Loại 1', 15, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 năm')),
    (N'Loại 2', 20, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 năm')),
    (N'Loại 3', 30, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 năm')),
    (N'Không giới hạn', NULL, 1, (SELECT PackageId FROM ServicePackage WHERE Name = N'1 năm'));


	INSERT INTO [RPSC].[dbo].[PricePackage] ([Price], [ApplicableDate], [ServiceDetailId])
VALUES
    -- 1 tuần
    (15000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 1' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tuần'))),
    (19000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 2' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tuần'))),
    (29000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 3' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tuần'))),
    (40000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Không giới hạn' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tuần'))),

    -- 1 tháng
    (49000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 1' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tháng'))),
    (59000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 2' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tháng'))),
    (69000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 3' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tháng'))),
    (80000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Không giới hạn' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 tháng'))),

    -- 3 tháng
    (99000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 1' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'3 tháng'))),
    (109000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 2' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'3 tháng'))),
    (119000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 3' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'3 tháng'))),
    (150000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Không giới hạn' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'3 tháng'))),

    -- 6 tháng
    (169000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 1' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'6 tháng'))),
    (199000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 2' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'6 tháng'))),
    (219000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 3' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'6 tháng'))),
    (250000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Không giới hạn' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'6 tháng'))),

    -- 1 năm
    (249000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 1' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 năm'))),
    (269000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 2' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 năm'))),
    (299000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Loại 3' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 năm'))),
    (350000, GETDATE(), (SELECT ServiceDetailId FROM ServiceDetail WHERE Type = N'Không giới hạn' AND PackageId = (SELECT PackageId FROM ServicePackage WHERE Name = N'1 năm')));
