INSERT INTO Users (FullName, BirthYear, Gender, Email, HashedPassword, AvatarPath, Address, Status)
VALUES 
    ('Nguyễn Văn A', '1990', 1, 'nguyenvana@example.com', 'hashedpassword1', 'path/to/avatar1.jpg', '123 Đường ABC', 1),
    ('Trần Thị B', '1985', 0, 'tranthib@example.com', 'hashedpassword2', 'path/to/avatar2.jpg', '456 Đường DEF', 1),
    ('Lê Văn C', '1992', 1, 'levanc@example.com', 'hashedpassword3', 'path/to/avatar3.jpg', '789 Đường GHI', 1);

INSERT INTO Categories (Name, Slug)
VALUES 
    ('Laptop', 'laptop'),
    ('Điện thoại', 'dien-thoai'),
    ('Máy tính bảng', 'may-tinh-bang');

INSERT INTO Products (Brand, Model, CategoryId, Cpu, Ram, Vga, ScreenSize, HardDisk, Os, Price, Amount, Image)
VALUES 
    ('Dell', 'XPS 13', 1, 'Intel i7', '16GB', 'NVIDIA GTX 1650', '13.3 inch', '512GB SSD', 'Windows 10', 2000.00, '10', 'path/to/image1.jpg'),
    ('Apple', 'iPhone 13', 2, 'A15 Bionic', '4GB', 'N/A', '6.1 inch', '128GB', 'iOS', 999.99, '20', 'path/to/image2.jpg'),
    ('Samsung', 'Galaxy Tab S7', 3, 'Snapdragon 865+', '8GB', 'N/A', '11 inch', '256GB', 'Android', 650.00, '15', 'path/to/image3.jpg');

INSERT INTO Orders (UserId, TotalPrice, Payment, Status)
VALUES 
    (1, 2000.00, 1, 1),
    (2, 999.99, 0, 1),
    (1, 650.00, 1, 1);

INSERT INTO OrderDetails (OrderId, ProductId, Quantity)
VALUES 
    (1, 1, 1),
    (2, 2, 1),
    (3, 3, 1),
    (1, 3, 2); 

INSERT INTO Admins (Username, HashedPassword, Role)
VALUES 
    ('admin1', 'hashedadminpassword1', 1),
    ('admin2', 'hashedadminpassword2', 1);
