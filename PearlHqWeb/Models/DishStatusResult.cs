namespace PearlHqWeb.Models;

public record DishStatusResult(
    int DishId,
    double? RawWeightGrams,
    double? ContentWeightGrams,
    DishStatus Status,
    double? SuggestedRefillGrams);
