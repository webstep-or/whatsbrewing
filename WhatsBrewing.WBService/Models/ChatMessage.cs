using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace WhatsBrewing.WBService.Models
{
    public class ChatMessage : TableEntity
    {
        // ctor is needed
        public ChatMessage() { }
        
        public string Nick { get; set; }
        public string Message { get; set; }
        public string Color { get; set; }
    }
}