-- Seed Admins
INSERT INTO Admins (Username, HashedPassword, Role, CreatedDate, LastModifiedDate)
VALUES
('admin_vn1', 'hashed_password_1', 1, GETDATE(), GETDATE()),
('admin_vn2', 'hashed_password_2', 2, GETDATE(), GETDATE());

-- Seed Categories
INSERT INTO Categories (Name, Slug, CreatedDate, LastModifiedDate)
VALUES
('Laptop', 'laptop', GETDATE(), GETDATE()),
('Máy tính để bàn', 'desktop', GETDATE(), GETDATE()),
('Máy tính bảng', 'tablet', GETDATE(), GETDATE()),
('Phụ kiện', 'accessories', GETDATE(), GETDATE());

-- Seed Discounts
INSERT INTO Discount (DiscountCode, DiscountPercentage, StartDate, EndDate, UsageLimit, TimesUsed, Status, CreatedDate, LastModifiedDate)
VALUES
('KHACHHANG10', 10.00, '2024-06-01', '2024-06-30', 100, 0, 1, GETDATE(), GETDATE()),
('GIAMGIA20', 20.00, '2024-12-01', '2024-12-31', 50, 0, 1, GETDATE(), GETDATE());

-- Seed Users
INSERT INTO Users (FullName, BirthYear, Gender, Email, HashedPassword, AvatarPath, Address, Status, CreatedDate, LastModifiedDate)
VALUES
('Nguyễn Văn A', '1990-05-15', 1, 'nguyen.a@example.com', 'hashed_password_1', '/images/nguyen_a.png', '123 Đường 1, Quận 1, TP.HCM', 1, GETDATE(), GETDATE()),
('Trần Thị B', '1985-03-22', 2, 'tran.b@example.com', 'hashed_password_2', '/images/tran_b.png', '456 Đường 2, Quận 2, TP.HCM', 1, GETDATE(), GETDATE()),
('Lê Văn C', '1992-07-30', 1, 'le.c@example.com', 'hashed_password_3', '/images/le_c.png', '789 Đường 3, Quận 3, TP.HCM', 1, GETDATE(), GETDATE());

-- Seed Products
INSERT INTO Products (Brand, Model, CategoryId, Cpu, Ram, Vga, ScreenSize, HardDisk, Os, Price, Stock, Image, CreatedDate, LastModifiedDate)
VALUES
('Asus', 'ZenBook 14', 1, 'Intel i5', '8GB', 'NVIDIA GTX 1650', '14"', '512GB SSD', 'Windows 10', 18000.00, 20, '/images/zenbook14.png', GETDATE(), GETDATE()),
('Dell', 'Inspiron 15', 1, 'Intel i7', '16GB', 'NVIDIA MX250', '15.6"', '1TB HDD', 'Windows 10', 15000.00, 15, '/images/inspiron15.png', GETDATE(), GETDATE()),
('Samsung', 'Galaxy Tab S7', 3, 'Qualcomm Snapdragon 865+', '8GB', 'Adreno 650', '11"', '128GB', 'Android', 12000.00, 30, '/images/galaxy_tab_s7.png', GETDATE(), GETDATE());

-- Seed Orders
INSERT INTO Orders (UserId, OrderDate, TotalPrice, Payment, Status, DiscountId, CreatedDate, LastModifiedDate)
VALUES
(1, GETDATE(), 18000.00, 1, 1, NULL, GETDATE(), GETDATE()),
(2, GETDATE(), 15000.00, 2, 1, 1, GETDATE(), GETDATE()),
(3, GETDATE(), 12000.00, 1, 2, 2, GETDATE(), GETDATE());

-- Seed OrderDetails
INSERT INTO OrderDetails (OrderId, ProductId, Quantity, Price)
VALUES
(1, 1, 1, 18000.00),
(2, 2, 1, 15000.00),
(3, 3, 1, 12000.00);
