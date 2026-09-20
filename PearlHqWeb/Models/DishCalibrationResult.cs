namespace PearlHqWeb.Models;

public record DishCalibrationResult(
    int DishId,
    double? EmptyWeightGrams,
    double? TargetFullWeightGrams,
    double? LowThresholdGrams);
