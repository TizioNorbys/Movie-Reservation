namespace MovieReservation.Application.DTOs.Review;

public record ReviewDetailsDto(
    int Rating,
    string Title,
    string Content,
    DateTime TimeStamp,
    int Likes,
    int Dislikes);