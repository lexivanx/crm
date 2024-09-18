-- Create Database
CREATE DATABASE CRM;

USE CRM;

-- Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL
);

-- Doctors Table
CREATE TABLE Doctors (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    City NVARCHAR(50) NOT NULL,
    Street NVARCHAR(100) NOT NULL,
    StreetNum INT NOT NULL
);

-- Products Table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Expiry DATE NOT NULL
);

-- Appointments Table
CREATE TABLE Appointments (
    Id INT PRIMARY KEY IDENTITY,
    AppointmentDate DATE NOT NULL,
    AppointmentTime TIME NOT NULL,
    DoctorID INT NOT NULL FOREIGN KEY REFERENCES Doctors(Id),
    UserID INT NOT NULL FOREIGN KEY REFERENCES Users(Id)
);

-- Table for many-to-many relationship between Appointments and Products
CREATE TABLE AppointmentProducts (
    AppointmentID INT NOT NULL,
    ProductID INT NOT NULL,
    QuantityLeft INT NOT NULL,
    CONSTRAINT FK_AppointmentProducts_Appointment FOREIGN KEY (AppointmentID) REFERENCES dbo.Appointments(Id),
    CONSTRAINT FK_AppointmentProducts_Product FOREIGN KEY (ProductID) REFERENCES dbo.Products(Id),
    CONSTRAINT PK_AppointmentProducts PRIMARY KEY (AppointmentID, ProductID)
);

-- Table for many-to-many relationship between Users and Products
CREATE TABLE UserProducts (
    UserID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    CONSTRAINT FK_UserProducts_User FOREIGN KEY (UserID) REFERENCES dbo.Users(Id),
    CONSTRAINT FK_UserProducts_Product FOREIGN KEY (ProductID) REFERENCES dbo.Products(Id),
    CONSTRAINT PK_UserProducts PRIMARY KEY (UserID, ProductID)

-- Populate DB with sample entries

INSERT INTO Users (Username, FullName, Password)
VALUES 
('toni', 'Antonia Petrova', 'toni123'),
('joro', 'Georgi Metodiev', 'joro123');

USE CRM;
INSERT INTO Doctors (Name, City, Street, StreetNum)
VALUES 
('Dr. Petar Petrov', 'Sofia', 'Shipchenski prohod', 13),
('Dr. Ivan Georgiev', 'Plovdiv', 'Tsar Simeon', 48),
('Dr. Yana Ivanova', 'Pernik', 'Ivan Asen II', 25),
('Dr. Tanya Stefanova', 'Vratsa', 'Knyaz Boris', 101),
('Dr. Desislava Tosheva', 'Burgas', 'Todor Kableshkov', 4);

INSERT INTO Products (Name, Quantity, Expiry)
VALUES 
('Paracetamol', 500, '2025-12-31'),
('Antibiotics', 300, '2024-06-30'),
('Spazmalgon', 200, '2025-03-15'),
('Aspirin', 400, '2024-11-20'),
('Degan', 250, '2025-08-10');
