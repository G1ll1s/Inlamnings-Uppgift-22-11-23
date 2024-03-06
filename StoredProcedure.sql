USE [Northwind33]
GO
/****** Object:  StoredProcedure [dbo].[AddOrderAndDetails]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddOrderAndDetails]
@CustomerID NCHAR(5),
@ProductID INT,
@OrderDate DATETIME,
@Quantity INT,
@UnitPrice DECIMAL(10,2)
AS
BEGIN
    DECLARE @OrderID INT;

    -- Insert the order record
    INSERT INTO Orders (CustomerID, OrderDate)
    VALUES (@CustomerID, @OrderDate);

    -- Get the order ID
    SET @OrderID = SCOPE_IDENTITY();

    -- Insert the order detail record
    INSERT INTO [Order Details] (OrderID, ProductID, Quantity, UnitPrice)
    VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice);
END;
GO
/****** Object:  StoredProcedure [dbo].[CustOrderHist]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CustOrderHist] @CustomerID nchar(5)
AS
SELECT ProductName, Total=SUM(Quantity)
FROM Products P, [Order Details] OD, Orders O, Customers C
WHERE C.CustomerID = @CustomerID
AND C.CustomerID = O.CustomerID AND O.OrderID = OD.OrderID AND OD.ProductID = P.ProductID
GROUP BY ProductName
GO
/****** Object:  StoredProcedure [dbo].[CustOrdersDetail]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CustOrdersDetail] @OrderID int
AS
SELECT ProductName,
    UnitPrice=ROUND(Od.UnitPrice, 2),
    Quantity,
    Discount=CONVERT(int, Discount * 100), 
    ExtendedPrice=ROUND(CONVERT(money, Quantity * (1 - Discount) * Od.UnitPrice), 2)
FROM Products P, [Order Details] Od
WHERE Od.ProductID = P.ProductID and Od.OrderID = @OrderID
GO
/****** Object:  StoredProcedure [dbo].[CustOrdersOrders]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CustOrdersOrders] @CustomerID nchar(5)
AS
SELECT OrderID, 
	OrderDate,
	RequiredDate,
	ShippedDate
FROM Orders
WHERE CustomerID = @CustomerID
ORDER BY OrderID
GO
/****** Object:  StoredProcedure [dbo].[Employee Sales by Country]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[Employee Sales by Country] 
@Beginning_Date DateTime, @Ending_Date DateTime AS
SELECT Employees.Country, Employees.LastName, Employees.FirstName, Orders.ShippedDate, Orders.OrderID, "Order Subtotals".Subtotal AS SaleAmount
FROM Employees INNER JOIN 
	(Orders INNER JOIN "Order Subtotals" ON Orders.OrderID = "Order Subtotals".OrderID) 
	ON Employees.EmployeeID = Orders.EmployeeID
WHERE Orders.ShippedDate Between @Beginning_Date And @Ending_Date
GO
/****** Object:  StoredProcedure [dbo].[Sales by Year]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[Sales by Year] 
	@Beginning_Date DateTime, @Ending_Date DateTime AS
SELECT Orders.ShippedDate, Orders.OrderID, "Order Subtotals".Subtotal, DATENAME(yy,ShippedDate) AS Year
FROM Orders INNER JOIN "Order Subtotals" ON Orders.OrderID = "Order Subtotals".OrderID
WHERE Orders.ShippedDate Between @Beginning_Date And @Ending_Date
GO
/****** Object:  StoredProcedure [dbo].[SalesByCategory]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SalesByCategory]
    @CategoryName nvarchar(15), @OrdYear nvarchar(4) = '1998'
AS
IF @OrdYear != '1996' AND @OrdYear != '1997' AND @OrdYear != '1998' 
BEGIN
	SELECT @OrdYear = '1998'
END

SELECT ProductName,
	TotalPurchase=ROUND(SUM(CONVERT(decimal(14,2), OD.Quantity * (1-OD.Discount) * OD.UnitPrice)), 0)
FROM [Order Details] OD, Orders O, Products P, Categories C
WHERE OD.OrderID = O.OrderID 
	AND OD.ProductID = P.ProductID 
	AND P.CategoryID = C.CategoryID
	AND C.CategoryName = @CategoryName
	AND SUBSTRING(CONVERT(nvarchar(22), O.OrderDate, 111), 1, 4) = @OrdYear
GROUP BY ProductName
ORDER BY ProductName
GO
/****** Object:  StoredProcedure [dbo].[spCustomerList]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[spCustomerList]
@OrderByColumn nvarchar(100)
As
Begin
set nocount on;
Declare @SqlQuery nvarchar(max);

set @SqlQuery = 'Select CustomerID, CompanyName as [Company Name], ContactName as [Contact Name], Address, City, Country
				 From Customers
				 Order by ' + QUOTENAME(@OrderByColumn);

		Exec sp_executesql @SqlQuery;
	end;
GO
/****** Object:  StoredProcedure [dbo].[spDelCompanyName]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[spDelCompanyName]
@CompanyName nvarchar(60)
as
Begin
DELETE FROM [Order Details]
WHERE OrderID IN (
    SELECT OrderID FROM Orders 
    WHERE CustomerID IN (
        SELECT CustomerID FROM Customers WHERE CompanyName = @CompanyName
    )
)

DELETE FROM Orders
WHERE CustomerID IN (
    SELECT CustomerID FROM Customers WHERE CompanyName = @CompanyName
)

DELETE FROM Customers
WHERE CompanyName = @CompanyName
end
GO
/****** Object:  StoredProcedure [dbo].[spDelCustomerIDandOrder]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spDelCustomerIDandOrder] 
    @CustomerID nchar(5)
AS
BEGIN
    DELETE FROM [Order Details]
    WHERE OrderID IN (
        SELECT OrderID FROM Orders WHERE CustomerID = @CustomerID
    )

    DELETE FROM Orders
    WHERE CustomerID = @CustomerID

    DELETE FROM Customers
    WHERE CustomerID = @CustomerID
END
GO
/****** Object:  StoredProcedure [dbo].[spInsertCustomer]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[spInsertCustomer]
@CustomerID Nchar(5),
@CompanyName Nvarchar(40),
@ContactName Nvarchar(30),
@Address Nvarchar(60),
@City Nvarchar(15),
@Country Nvarchar(15)
As
Begin
insert into Customers(CustomerID ,CompanyName, ContactName, Address, City, Country)
Values (@CustomerID ,@CompanyName, @ContactName, @Address, @City, @Country)
End
GO
/****** Object:  StoredProcedure [dbo].[spSelectSeniorEmployees]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spSelectSeniorEmployees]
AS
Begin
SELECT *
FROM Employees
WHERE DATEDIFF(Year, HireDate, GETDATE()) > 5
End
GO
/****** Object:  StoredProcedure [dbo].[spShitCountry]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[spShitCountry]
@ShipCountry NVARCHAR(30)
AS
Begin
select o.ShipCountry as ShipCountry, e.FirstName+ ' ' + e.LastName as Name, sum(UnitPrice * Quantity) as Sales 
from Orders o
Join Employees e
On o.EmployeeID = e.EmployeeID
Join [Order Details] od
ON o.OrderID = od.OrderID
Where o.ShipCountry = @ShipCountry
group by o.ShipCountry, e.FirstName, e.LastName
order by e.FirstName
end
GO
/****** Object:  StoredProcedure [dbo].[spUpdateCustomer]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[spUpdateCustomer]
	@Address Nvarchar(60),
	@City Nvarchar(15),
	@Country nvarchar(15),
	@CustomerID nchar(5)
as
Begin
	update Customers
	set Address = @Address, City = @City, Country = @Country
	where CustomerID = @CustomerID
end
GO
/****** Object:  StoredProcedure [dbo].[Ten Most Expensive Products]    Script Date: 2023-11-22 21:55:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[Ten Most Expensive Products] AS
SET ROWCOUNT 10
SELECT Products.ProductName AS TenMostExpensiveProducts, Products.UnitPrice
FROM Products
ORDER BY Products.UnitPrice DESC
GO
