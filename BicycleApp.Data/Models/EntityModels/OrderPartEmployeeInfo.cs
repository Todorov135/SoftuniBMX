﻿namespace BicycleApp.Data.Models.EntityModels
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    public class OrderPartEmployeeInfo
    {
        [Key]
        [Comment("Id of information for manufacturing part.")]
        public int Id { get; set; }
        
        [Comment("Id of the order from the client")]
        public string? DescriptionForWorker { get; set; }

        [Required]
        [Comment("Timespan for production on part.")]
        public TimeSpan ProductionТime { get; set; }

        [Required]
        [Comment("Id of the order from the client")]
        public int OrderId { get; set; }

        [Required]
        [Comment("Id of the part from the order")]
        public int PartId { get; set; }

        [Required]
        public virtual OrderPartEmployee OrderPartEmployee { get; set; } = null!;
    }
}
