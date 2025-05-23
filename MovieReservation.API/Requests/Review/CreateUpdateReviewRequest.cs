namespace MovieReservation.API.Requests.Review;

public record CreateUpdateReviewRequest(int Rating, string Title, string Content);