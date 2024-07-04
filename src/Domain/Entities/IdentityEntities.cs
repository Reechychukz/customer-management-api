﻿using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public RoleClaim() : base() { }
    }

    public class UserClaim : IdentityUserClaim<Guid>
    {
        public UserClaim() : base() { }
    }

    public class UserRole : IdentityUserRole<Guid>
    {
        public UserRole() : base() { }
    }

    public class UserLogin : IdentityUserLogin<Guid>
    {
        public UserLogin() : base() { }
    }
}

