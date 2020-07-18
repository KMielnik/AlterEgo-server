﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Domains
{
    public class Image : MediaResource
    {
        public Image(string filename, User owner, TimeSpan plannedLifetime) : base(filename, owner, plannedLifetime)
        {
        }
    }
}
