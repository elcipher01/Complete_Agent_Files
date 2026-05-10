using System;
using System.Collections.Generic;

namespace NextHorizon.Models.Admin_Models
{
    
 public class SuperAdminDashboardViewModel
{
    public PlatformStats Stats { get; set; }
    public List<TopSellerViewModel> TopSellers { get; set; }
    public List<ConsumerLeaderboardViewModel> ConsumerLeaderboard { get; set; }
     public List<DashboardApprovalItem> ApprovalHub { get; set; } = new();

    public int PendingTickets { get; set; }
    public List<PendingTicketViewModel> PendingTicketsList { get; set; } = new List<PendingTicketViewModel>();
    public decimal PlatformRevenue { get; set; }
    public int PendingPayouts { get; set; }
    public int PendingSellers { get; set; }
    public List<DashboardAuditLog> AuditLogs { get; set; } = new();
}


public class DashboardAuditLog
{
    public DateTime Timestamp { get; set; }
    public string AdminName { get; set; } = "";
    public string Action { get; set; } = "";
    public string Target { get; set; } = "";
    public string Status { get; set; } = "";
}

    public class PlatformStats
    {
        public int TotalConsumers { get; set; }
        public int TotalSellers { get; set; }
        public int ActiveChallenges { get; set; }
        public string TotalKudos { get; set; } // String to handle "115.3K" formatting
    }

    public class TopSellerViewModel
    {
        public string ShopName { get; set; }
        public int MostPurchasedCount { get; set; } // Top 5 purchased items
        public int TotalProductsSold { get; set; }
        public string GrowthStatus { get; set; }
    }

    public class ConsumerLeaderboardViewModel
    {
        public int Rank { get; set; }
        public string UserName { get; set; }
        public decimal StravaKM { get; set; }
        public string Pace { get; set; }
        public bool IsVerified { get; set; }
    }

 public class SellerViewModel
{
    public int SellerId { get; set; }
    public int UserId { get; set; }
    public string BusinessName { get; set; }
    public string BusinessEmail { get; set; }
    public string BusinessPhone { get; set; }
    public string BusinessType { get; set; }
    public string BusinessAddress { get; set; }
    public string LogoPath { get; set; }
    
    public string DocumentPath { get; set; }
    public bool HasDocument { get; set; }   // ADD THIS
    public string SellerStatus { get; set; }
    public string OwnerName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int TotalProducts { get; set; }
    public decimal TotalSales { get; set; }
}

public class DashboardApprovalItem
{
    public string RequestType  { get; set; } = ""; // "New Seller", "Withdrawal", "Voucher"
    public string EntityName   { get; set; } = "";
    public string Details      { get; set; } = "";
    public string Status       { get; set; } = "";
    public string ActionLabel  { get; set; } = "";
    public string RedirectUrl  { get; set; } = "";
}

public class PendingTicketViewModel
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string Question { get; set; }
    public string Status { get; set; }
    public string UserType { get; set; }
    public string SenderType { get; set; }
    public string SenderName { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // For display formatting
    public string TimeAgo
    {
        get
        {
            var diff = DateTime.Now - CreatedAt;
            if (diff.TotalMinutes < 1) return "Just now";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} hours ago";
            return $"{(int)diff.TotalDays} days ago";
        }
    }
    
    public string IconClass => Category?.ToLower() switch
    {
        "login" => "bi bi-box-arrow-in-right",
        "payment" => "bi bi-credit-card",
        "verification" => "bi bi-shield-check",
        "order" => "bi bi-truck",
        _ => "bi bi-envelope"
    };
    
    public string IconColor => Category?.ToLower() switch
    {
        "login" => "text-primary",
        "payment" => "text-success",
        "verification" => "text-warning",
        "order" => "text-info",
        _ => "text-secondary"
    };
}

}

