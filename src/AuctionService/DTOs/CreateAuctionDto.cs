using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.DTOs;

public class CreateAuctionDto
{
    [Required]
    public string Make { get; set; } = null!;
    [Required]
    public string Model { get; set; } = null!;
    [Required]
    public int Year { get; set; }
    [Required]
    public string Color { get; set; } = null!;
    [Required]
    public int Mileage { get; set; }
    [Required]
    public string ImageUrl { get; set; } = null!;
    [Required]
    public int ReservePrice { get; set; }
    [Required]
    public DateTime AuctionEnd { get; set; }
}