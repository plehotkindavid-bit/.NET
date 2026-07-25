using API.Enetites;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class StoreContext(DbContextOptions<StoreContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products { get; set; }=null! ; 
     public DbSet<Order> Orders  { get; set; }=null! ; 
}