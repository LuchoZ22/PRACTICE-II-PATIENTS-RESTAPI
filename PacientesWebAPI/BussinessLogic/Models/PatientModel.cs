﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPB.BussinessLogic.Models
{
    public class PatientModel
    {
       
        public string? Name { get; private set; }
        public string? LastName { get; private set; }   
        public string? CI { get; private set; }
        public string? BloodType {  get; private set; }

    }
}