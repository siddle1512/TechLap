﻿namespace TechLap.API.DTOs.Requests.DiscountRequests;

public class UpdateAdminDiscountRequest
{
    public string DiscountCode { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime EndDate { get; set; }
    public int UsageLimit { get; set; }
}