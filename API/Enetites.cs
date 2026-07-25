using System;
using Microsoft.AspNetCore.Identity;

namespace API.Enetites  ;

public class Product 
{
    public int Id {get ; set ; }
 public required  string  Name  {get ; set;  }
 public required  string  Description  {get ; set;  }
 public   int Price  {get ; set;  }
  public  required  string PictureUrl {get ; set;  }
  public  required  string  Type   {get ; set;  }

 public  required  string Brand  {get ; set;  }
 public    int QuantitiesInStock  {get ; set;  }
}  

  