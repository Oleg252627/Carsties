using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController(AuctionDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
    {
        var auctions = await context.Auctions
        .Include(z => z.Item)
        .OrderBy(z => z.Item.Make)
        .ToListAsync();

        return mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
    {
        var auction = await context.Auctions
        .Include(z => z.Item)
        .FirstOrDefaultAsync(z => z.Id == id);

        if (auction is null) return NotFound();

        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
    {
        var auction = mapper.Map<Auction>(dto);
        auction.Seller = "test";

        context.Auctions.Add(auction);
        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Failed to create auction");

        var auctionDto = mapper.Map<AuctionDto>(auction);

        return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, auctionDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto dto)
    {
        var auction = await context.Auctions.Include(z => z.Item).FirstOrDefaultAsync(z => z.Id == id);

        if (auction is null) return NotFound();

        auction.Item.Make = dto.Make ?? auction.Item.Make;
        auction.Item.Model = dto.Model ?? auction.Item.Model;
        auction.Item.Year = dto.Year ?? auction.Item.Year;
        auction.Item.Color = dto.Color ?? auction.Item.Color;
        auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;

        var result = await context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Failed to update auction");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await context.Auctions.FindAsync(id);

        if (auction is null) return NotFound();

        context.Auctions.Remove(auction);
        var result = await context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Failed to delete auction");
    }
}