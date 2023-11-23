using SamWarehouse.Models;

namespace SamWarehouse.Repositories
    {
    public class AuthRepository
        {
        private readonly ItemDbContext _context;
        public AuthRepository(ItemDbContext context)
            {
            _context = context;
            }

        private AppUser GetUserByUsername(string userName)
            {
            var user = _context.AppUsers.Where(c => c.UserName.Equals(userName)).FirstOrDefault();

            return user;
            }

        public AppUser Authenticate(LoginUserDTO credentials)
            {
            // Look for an account matching the provided username.
            var userDetails = GetUserByUsername(credentials.Username);
            // If no matchin username exists, return.
            if (userDetails == null)
                {
                return null;
                }
            //Check the provided password matches the hashed password for the account.
            if (BCrypt.Net.BCrypt.EnhancedVerify(credentials.Password, userDetails.PasswordHash))
                {
                return userDetails;
                }
            return null;

            }


        // TODO - Remove the salting - as the bcrypt library generates a salt automatically
        public AppUser CreateUser(LoginUserDTO loginDetails, string role)
            {

            // if the string the user provides is not in the 'Roles' enum, then use the Guest role
            // this should never happen when using a dropdown list, but could be circumvented using JS
            if (!System.Enum.IsDefined(typeof(Roles), role))
                {
                role = Roles.Guest.ToString();
                }
            // Checks if there is a user with the desired userrname already and retrieves it.
            var userDetails = GetUserByUsername(loginDetails.Username);
            // Username Exists go no further.
            if (userDetails != null)
                {
                return null;
                }
            //Create new user object
            AppUser user = new AppUser()
                {
                UserName = loginDetails.Username,
                Role = role,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(loginDetails.Password)
                };
            //Save the new user to the database and return it to the caller.
            _context.AppUsers.Add(user);
            _context.SaveChanges();
            return user;
            }


        }

    public enum Roles
        {
        Admin,
        Moderator,
        Guest
        }
    }
