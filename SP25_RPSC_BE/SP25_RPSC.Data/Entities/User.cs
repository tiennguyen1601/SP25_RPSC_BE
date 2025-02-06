﻿using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public string? Avatar { get; set; }

    public string? Password { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? RoleId { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Feedback> FeedbackReviewees { get; set; } = new List<Feedback>();

    public virtual ICollection<Feedback> FeedbackReviewers { get; set; } = new List<Feedback>();

    public virtual ICollection<Landlord> Landlords { get; set; } = new List<Landlord>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Report> ReportReportedUsers { get; set; } = new List<Report>();

    public virtual ICollection<Report> ReportReporters { get; set; } = new List<Report>();

    public virtual Role? Role { get; set; }
}
