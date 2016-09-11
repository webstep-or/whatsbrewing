using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace WhatsBrewing.WBService.Models
{
    public class PushMessage : TableEntity
    {
        // ctor is needed
        public PushMessage(){}
        
        public string Message { get; set; }

        public string Category { get; set; }
    }
}