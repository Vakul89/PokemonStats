namespace Backend.Validators
{
    public static class PokemonQueryValidator
    {
        private static readonly string[] ValidSortBy = new[] { "id", "name", "wins", "losses", "ties" };
        private static readonly string[] ValidSortDirection = new[] { "asc", "desc" };

        public static bool IsValidSortBy(string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return false;

            return ValidSortBy.Contains(sortBy.Trim().ToLowerInvariant());
        }

        public static bool IsValidSortDirection(string? sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortDirection))
                return false;

            return ValidSortDirection.Contains(sortDirection.Trim().ToLowerInvariant());
        }
    }
}
