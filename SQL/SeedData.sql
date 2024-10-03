-- Seed Categories
INSERT INTO Categories (Name, Slug) VALUES
('Gaming Laptops', 'gaming-laptops'),
('Business Laptops', 'business-laptops'),
('Ultrabooks', 'ultrabooks'),
('2-in-1 Laptops', '2-in-1-laptops');

-- Seed Products
INSERT INTO Products (Brand, Model, CategoryId, Cpu, Ram, Vga, ScreenSize, HardDisk, Os, Price, Amount, Image) VALUES
('Dell', 'XPS 15', 3, 'Intel Core i7-11800H', '16GB', 'NVIDIA GeForce RTX 3050 Ti', '15.6"', '512GB SSD', 'Windows 11', 1799.99, '10', 'dell-xps-15.jpg'),
('Lenovo', 'ThinkPad X1 Carbon', 2, 'Intel Core i5-1135G7', '8GB', 'Intel Iris Xe Graphics', '14"', '256GB SSD', 'Windows 10 Pro', 1299.99, '15', 'lenovo-thinkpad-x1.jpg'),
('ASUS', 'ROG Zephyrus G14', 1, 'AMD Ryzen 9 5900HS', '32GB', 'NVIDIA GeForce RTX 3060', '14"', '1TB SSD', 'Windows 10', 1999.99, '8', 'asus-rog-zephyrus.jpg'),
('HP', 'Spectre x360', 4, 'Intel Core i7-1165G7', '16GB', 'Intel Iris Xe Graphics', '13.3"', '512GB SSD', 'Windows 11', 1399.99, '12', 'hp-spectre-x360.jpg'),
('Apple', 'MacBook Pro', 3, 'Apple M1 Pro', '16GB', 'Apple M1 Pro GPU', '14"', '512GB SSD', 'macOS', 1999.99, '20', 'apple-macbook-pro.jpg');

-- Seed Users
INSERT INTO Users (FullName, BirthYear, Gender, Email, HashedPassword, AvatarPath, AddressPath, Status) VALUES
('John Doe', '1990', 0, 'john.doe@example.com', 'hashed_password_here', 'avatars/john.jpg', 'addresses/john.txt', 0),
('Jane Smith', '1985', 1, 'jane.smith@example.com', 'hashed_password_here', 'avatars/jane.jpg', 'addresses/jane.txt', 0),
('Bob Johnson', '1995', 0, 'bob.johnson@example.com', 'hashed_password_here', 'avatars/bob.jpg', 'addresses/bob.txt', 0);

-- Seed Admins
INSERT INTO Admins (Username, HashedPassword, Role) VALUES
('admin', 'hashed_admin_password', 0),
('manager', 'hashed_manager_password', 1),
('support', 'hashed_support_password', 2);

-- Seed Orders
INSERT INTO Orders (UserId, TotalPrice, Payment, Status) VALUES
(1, 1799.99, 0, 2),
(2, 3299.98, 1, 1),
(3, 1399.99, 2, 0);

-- Seed OrderDetails
INSERT INTO OrderDetails (OrderId, ProductId, Quantity) VALUES
(1, 1, 1),
(2, 3, 1),
(2, 4, 1),
(3, 4, 1);