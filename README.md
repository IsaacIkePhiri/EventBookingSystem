# EventEaseBookingSystem

## Project Overview

**EventEaseBookingSystem** is an ASP.NET Core MVC web application designed to manage venues, events, and bookings. The system allows users to perform full CRUD operations (Create, Read, Update, Delete) on venues, events, and bookings while storing data in a SQL LocalDB database using Entity Framework Core.

The project demonstrates how a data-driven web application can be structured using the Model-View-Controller (MVC) architecture and how relational databases can be integrated with web applications.

---

## Features

- Create, edit, view, and delete **Venues**
- Manage **Events** assigned to venues
- Manage **Bookings** linked to events
- Data persistence using **SQL Server LocalDB**
- Clean user interface built with **Bootstrap**
- Follows **MVC architecture** for separation of concerns

---

## Technologies Used

- ASP.NET Core MVC  
- C#  
- Entity Framework Core  
- SQL Server LocalDB  
- Bootstrap  
- Visual Studio 2022  

---

## System Architecture

The application follows the **Model-View-Controller (MVC)** design pattern.

### Models
- **Venue**  
- **Event**  
- **Booking**  

### Controllers
- **HomeController**  
- **VenuesController**  
- **EventsController**  
- **BookingsController**  

### Views
- Razor Views for displaying and interacting with data

---

## Database Structure

### Venue
- VenueId (Primary Key)  
- Name  
- Location  
- Capacity  
- ImageUrl  

### Event
- EventId (Primary Key)  
- EventName  
- StartDate  
- EndDate  
- VenueId (Foreign Key)  

### Booking
- BookingId (Primary Key)  
- CustomerName  
- BookingDate  
- EventId (Foreign Key)  
- VenueId (Foreign Key)  

---

## Entity Relationships

The database uses **one-to-many relationships** to avoid many-to-many complexity:

- One **Venue** can host many **Events**  
- One **Event** can have many **Bookings**  
- One **Venue** can have many **Bookings**  

---

## Running the Project

1. Open the project in **Visual Studio 2022**  
2. Restore **NuGet packages**  
3. Run the project  
4. Navigate to the following URLs in your browser:
   - `/Venues`  
   - `/Events`  
   - `/Bookings`  

The application will automatically connect to the LocalDB database and load stored data.

---

## References

- Connolly, T. and Begg, C., 2015. *Database Systems: A Practical Approach to Design, Implementation, and Management.* 6th ed. Harlow: Pearson Education.  
- Microsoft, 2023. *ASP.NET Core MVC Overview.* Microsoft Learn.  
- Microsoft, 2023. *Introduction to Entity Framework Core.* Microsoft Learn.  
- Rogers, Y., Sharp, H. and Preece, J., 2015. *Interaction Design: Beyond Human-Computer Interaction.* 4th ed. Chichester: Wiley.  
- Sommerville, I., 2016. *Software Engineering.* 10th ed. Boston: Pearson.
