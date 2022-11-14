namespace InputSanitizer.Demo.WebApi.Dtos
{
    public class TestInnerDto
    {
        public int Id { get; set; }
        [Sanitized(PolicyName = "")]
        public string? Name { get; set; }

        public IList<string>? TestStringList { get; set; }
    }
}
