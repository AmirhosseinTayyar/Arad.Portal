﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Models.Shared
{
    public class Picture
    {
        public string PictureId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public bool IsMain { get; set; }
    }
}
