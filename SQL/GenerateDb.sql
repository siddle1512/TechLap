CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    BirthYear NVARCHAR(4),
    Gender INT,
    Email NVARCHAR(100) NOT NULL,
    HashedPassword NVARCHAR(255) NOT NULL,
    AvatarPath NVARCHAR(255),
    AddressPath NVARCHAR(255),
    Status INT
);

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Slug NVARCHAR(100)
);

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Brand NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    CategoryId INT NOT NULL,
    Cpu NVARCHAR(100),
    Ram NVARCHAR(50),
    Vga NVARCHAR(100),
    ScreenSize NVARCHAR(50),
    HardDisk NVARCHAR(100),
    Os NVARCHAR(50),
    Price FLOAT NOT NULL,
    Amount NVARCHAR(50),
    Image NVARCHAR(255),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    TotalPrice FLOAT NOT NULL,
    Payment INT,
    Status INT,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE OrderDetails (
    OrderId INT,
    ProductId INT,
    Quantity INT NOT NULL,
    PRIMARY KEY (OrderId, ProductId),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE Admins (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    HashedPassword NVARCHAR(255) NOT NULL,
    Role INT
);