﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Entities.General.State
{
    public class State : BaseEntity
    {
        public State()
        {
            Counties = new();
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public List<County.County> Counties { get; set; }
    }
}
