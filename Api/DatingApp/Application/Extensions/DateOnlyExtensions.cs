namespace DatingApp.Application.Extensions
{
    public static class DateOnlyExtensions
    {
        public static int ToAge(this DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;
            return age;
        }
    }
}
