﻿namespace BicycleApp.Services.Models.Order
{
    public class OrderStateDto
    {
        public string PartType { get; set; } = null!;
        public string PartModel { get; set; } = null!;
        public string NameOfEmplоyeeProducedThePart { get; set; } = null!;
        public bool IsProduced { get; set; }

    }
}