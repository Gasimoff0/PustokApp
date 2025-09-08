using Microsoft.AspNetCore.Identity;

namespace PustokApp
{
    public class CustomErrorDescriber: IdentityErrorDescriber
    {
        override public IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"Password must be at least {length} characters long."
            };
        }
        override public IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Password must contain at least one special character."
            };
        }
        override public IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Password must contain at least one digit ('0'-'9')."
            };
        }
        override public IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Password must contain at least one lowercase letter ('a'-'z')."
            };
        }
        override public IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "Password must contain at least one uppercase letter ('A'-'Z')."
            };
        }
        override public IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"Username '{userName}' is already taken."
            };
        }
        override public IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"Email '{email}' is already taken."
            };
        }
        override public IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = $"Username '{userName}' is invalid, can only contain letters or digits."
            };
        }
        override public IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = $"Email '{email}' is invalid."
            };
        }
        override public IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyHasPassword),
                Description = "User already has a password set."
            };
        }
        override public IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = nameof(PasswordMismatch),
                Description = "Incorrect password."
            };
        }
        override public IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = nameof(InvalidToken),
                Description = "Invalid token."
            };
        }

    }
}
