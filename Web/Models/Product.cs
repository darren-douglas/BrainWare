﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    using System.Security.AccessControl;
    
    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}