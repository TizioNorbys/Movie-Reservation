namespace MovieReservation.Application.DTOs.Review;

public record CreateUpdateReviewDto(int Rating, string Title, string Content);