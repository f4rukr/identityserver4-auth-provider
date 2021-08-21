namespace Klika.Identity.Model.Constants.Regex
{
    public class RegexValidators
    {
        public const string EmailRegex = "^[a-zA-Z0-9!#$%&'*+-/=?^_`{|}~.]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$";
        public const string FirstName = "[A-ZČĆŠŽĐ][A-Za-zČčĆćŠšŽžĐđ]+(-[A-ZČĆŠŽĐ][A-Za-zČčĆćŠšŽžĐđ]+)?";
        public const string LastName = "[A-ZČĆŠŽĐ][A-Za-zČčĆćŠšŽžĐđ]+(-[A-ZČĆŠŽĐ][A-Za-zČčĆćŠšŽžĐđ]+)?";
        public const string Password = ".*[a-zA-ZČĆŠĐŽčćšđž].*";
    }
}
