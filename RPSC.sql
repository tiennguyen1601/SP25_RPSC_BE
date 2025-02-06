-- Tạo database RPSC và chuyển sang database đó
CREATE DATABASE RPSC;
GO

USE RPSC;
GO

-- 1. Bảng Role (vai trò)
CREATE TABLE Role (
    RoleId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    RoleName NVARCHAR(255) NOT NULL
);
GO

-- 2. Bảng ServicePackage (gói dịch vụ)
CREATE TABLE ServicePackage (
    PackageId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    Duration INT,  -- ví dụ: số ngày hoặc số tháng
    Description NVARCHAR(MAX),
    Status NVARCHAR(50)
);
GO

-- 3. Bảng User
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

-- 4. Bảng Landlord (chủ nhà)
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

-- 5. Bảng Customer (khách hàng)
CREATE TABLE Customer (
    CustomerId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Preferences NVARCHAR(MAX),
    LifeStyle NVARCHAR(255),
    BudgetRange NVARCHAR(50),
    PreferredLocation NVARCHAR(255),
    Gender NVARCHAR(50),
    Requirement NVARCHAR(MAX),
    Status NVARCHAR(50),
    UserId NVARCHAR(36),
    CONSTRAINT FK_Customer_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);
GO

-- 6. Bảng Rooms (phòng cho thuê)
CREATE TABLE Rooms (
    RoomId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(255),
    Description NVARCHAR(MAX),
    Deposite DECIMAL(18,2),
    Status NVARCHAR(50),
    Location NVARCHAR(255),
    UpdatedAt DATETIME,
    LandlordId NVARCHAR(36),
    CONSTRAINT FK_Rooms_Landlord FOREIGN KEY (LandlordId) REFERENCES Landlord(LandlordId)
);
GO

-- 7. Bảng Post (tin đăng)
CREATE TABLE Post (
    PostId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(255),
    Descripton NVARCHAR(MAX),
    Price DECIMAL(18,2),
    Status NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    RentalRoomId NVARCHAR(36),  -- FK tham chiếu đến Rooms (phòng cho thuê)
    CONSTRAINT FK_Post_RentalRoom FOREIGN KEY (RentalRoomId) REFERENCES Rooms(RoomId)
);
GO

-- 8. Bảng RoommateRequests (yêu cầu tìm bạn ở ghép)
CREATE TABLE RoommateRequests (
    RequestId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Message NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50), -- Ví dụ: Pending, Accepted, Rejected
    PostId NVARCHAR(36),
    CONSTRAINT FK_RoommateRequests_Post FOREIGN KEY (PostId) REFERENCES Post(PostId)
);
GO

-- 9. Bảng CustomerRequest (yêu cầu của khách hàng)
CREATE TABLE CustomerRequest (
    CustomerRequestId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Status NVARCHAR(50),
    RequestId NVARCHAR(36),
    CustomerId NVARCHAR(36),
    CONSTRAINT FK_CustomerRequest_RoommateRequests FOREIGN KEY (RequestId) REFERENCES RoommateRequests(RequestId),
    CONSTRAINT FK_CustomerRequest_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId)
);
GO

-- 10. Bảng CustomerContracts (hợp đồng khách hàng)
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

-- 11. Bảng ExtendContracts (hợp đồng gia hạn)
CREATE TABLE ExtendContracts (
    ExtendContractId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    StartDate DATETIME,
    EndDate DATETIME,
    ContractId NVARCHAR(36),
    CONSTRAINT FK_ExtendContracts_Contract FOREIGN KEY (ContractId) REFERENCES CustomerContracts(ContractId)
);
GO

-- 12. Bảng Payment (thanh toán)
CREATE TABLE Payment (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    Amount DECIMAL(18,2),
    PaymentDate DATETIME DEFAULT GETDATE(),
    TransactionID NVARCHAR(255),
    PaymentMethod NVARCHAR(50), -- Ví dụ: Online, Cash
    ContractId NVARCHAR(36),
    ExtendContractId NVARCHAR(36),
    CONSTRAINT FK_Payment_Contract FOREIGN KEY (ContractId) REFERENCES CustomerContracts(ContractId),
    CONSTRAINT FK_Payment_ExtendContract FOREIGN KEY (ExtendContractId) REFERENCES ExtendContracts(ExtendContractId)
);
GO

-- 13. Bảng Report (báo cáo)
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

-- 14. Bảng Notification (thông báo)
CREATE TABLE Notification (
    NotificationId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    Message NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UserId NVARCHAR(36),
    CONSTRAINT FK_Notification_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
);
GO

-- 15. Bảng LandlordContracts (hợp đồng của chủ nhà)
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

-- 16. Bảng Feedback (phản hồi)
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

-- 17. Bảng Favorite (yêu thích)
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

-- 18. Bảng PricePackage (bảng giá)
CREATE TABLE PricePackage (
    PriceId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    ApplicableDate DATETIME,
    Price DECIMAL(18,2),
    PackageId NVARCHAR(36),
    CONSTRAINT FK_PricePackage_Package FOREIGN KEY (PackageId) REFERENCES ServicePackage(PackageId)
);
GO

-- 19. Bảng Address (địa chỉ)
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

-- 20. Bảng RoomType (loại phòng)
CREATE TABLE RoomType (
    RoomTypeId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    RoomTypeName NVARCHAR(255),
    Square DECIMAL(10,2),
    Description NVARCHAR(MAX),
    Amenities NVARCHAR(MAX),
    MaxOccupancy INT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    RoomId NVARCHAR(36),
    CONSTRAINT FK_RoomType_Room FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId)
);
GO

-- 21. Bảng RoomImage (hình ảnh phòng)
CREATE TABLE RoomImage (
    ImageId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    ImageUrl NVARCHAR(255),
    RoomTypeId NVARCHAR(36),
    CONSTRAINT FK_RoomImage_RoomType FOREIGN KEY (RoomTypeId) REFERENCES RoomType(RoomTypeId)
);
GO

-- 22. Bảng RoomPrice (giá phòng theo loại)
CREATE TABLE RoomPrice (
    RoomPriceId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    ApplicableDate DATETIME,
    Price DECIMAL(18,2),
    RoomTypeId NVARCHAR(36),
    CONSTRAINT FK_RoomPrice_RoomType FOREIGN KEY (RoomTypeId) REFERENCES RoomType(RoomTypeId)
);
GO

-- 23. Bảng RoomService (dịch vụ của phòng)
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

-- 24. Bảng RoomServicePrice (giá dịch vụ phòng)
CREATE TABLE RoomServicePrice (
    RoomServicePriceId  NVARCHAR(36) PRIMARY KEY DEFAULT NEWID(),
    ApplicableDate DATETIME,
    Price DECIMAL(18,2),
    RoomServiceId NVARCHAR(36),
    CONSTRAINT FK_RoomServicePrice_RoomService FOREIGN KEY (RoomServiceId) REFERENCES RoomService(RoomServiceId)
);
GO

-- 25. Bảng RoomStay (lưu trú)
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

-- 26. Bảng RoomStayCustomer (khách lưu trú)
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
