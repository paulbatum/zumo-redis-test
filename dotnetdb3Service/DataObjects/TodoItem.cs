﻿using Microsoft.WindowsAzure.Mobile.Service;

namespace dotnetdb3Service.DataObjects
{
    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}