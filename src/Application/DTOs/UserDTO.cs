namespace Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public bool IsVerified { get; set; }
    }

    public class UserSignupDto
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int StateId { get; set; }
        public int LGAId { get; set; }
    }

    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IsVerified { get; set; }
        public string AccessToken { get; set; }
        public DateTimeOffset ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }

    public class UserByIdDto : UserDto
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public bool IsVerified { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class VerifyTokenDTO
    {
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
    }
}

