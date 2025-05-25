# Movie Reservation System
A **Web API** built with **ASP.NET Core**. The system includes user authentication, movie and showtime management and seat reservation functionalities. It allows users to rate and review movies, react to reviews, reserve seats for showtimes and manage their bookings. Admins can also manage movies and showtimes, promote users to higher roles and generate reports for showtimes.

## Features
- **Authentication and authorization**
    - **Sign-up and Login**: Users can create accounts and log in
    - **JWT-based authentication**: Requests are authenticated by extracting and validating **JSON Web Token**
    - **Role-based authorization**: Endpoints access is secured by checking the user's roles
    - **Role management**: Admins can add and remove roles

- **Movie management**
  - Users:
      - View all the reviews of a movie, sort and filter them by rating, reactions count or date 
      - Get all the actors of a movie
  - Admins: Create, update and delete movies.
  
- **Showtime management**
  - Users:
      - See all the showtimes of a movie for a specific date
      - Get the info of a showtime, including its theater hall number and the availability of the seats
  - Admins:
    - Create, update and delete showtimes

- **Actor management**
  - Users: Get all the movies the actor has starred in
  - Admins: Create, update, delete and assign/remove actors to movies
 
- **Seats reservation**
  - Users:
    - See the seats available for a showtime and select the desired ones
    - Reserve seats by creating a reservation. Users can also cancel upcoming reservations
    - View all their reservations
  - Admins:
    - View all the reservation for a specific showtime
  
- **Movie reviewing**
    - Users can leave review to rate movies. Reviews can be updated and removed.
    - Users can react to reviews by giving a like or dislike.

- **Movie search**
  - **Levenshtein distance** based algorithm for best title matches
  - Search functionality with filtering and sorting options.
    Users can search movies using one or multiple filters, including:
    - Title
    - Rating
    - Genres
    - Runtime
    - Release year
    - Language
    - Country

## Tech Stack
- Framework: **ASP.NET Core**
- Languages: **C#**
- Database: **MySql**
- ORM: **Entity Framework Core**
- Authentication: **ASP.NET Core Identity** + **JWT**
- Validation: **Fluent validation**
- Logging: **Serilog**
- Testing: **XUnit**, **NSubstitute** 
- Others: **FluentResults**, **Problem Details**

## **Database overview**
- **Seeding**: The database is populated with a set of initial data, which involve _Users_ (initial admin), _Roles_, _Genres_, _Halls_ and _Seats_ tables
- **Schema**:
  
  ![Database schema](/Assets/MovieReservationERD.png) 
