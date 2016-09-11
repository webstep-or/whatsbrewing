using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhatsBrewing.DAL
{
    public interface IUnitOfwork : IDisposable
    {
        Context DBContext { get; set; }
    }
}
