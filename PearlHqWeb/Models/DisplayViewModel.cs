namespace PearlHqWeb.Models;

public record DisplayViewModel(
    List<(Dish Dish, DishStatusResult Status)> FoodDishes,
    List<(Dish Dish, DishStatusResult Status)> WaterDishes);
