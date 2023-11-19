﻿namespace BicycleApp.Services.Models
{
    public class PartDto
    {
        public int Id { get; set; }     
        public string? Name { get; set; } 
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? OEMNumber { get; set; }
        public double Rating { get; set; }
        public double Quantity { get; set; }
        public decimal SalePrice { get; set; }
    }
}
