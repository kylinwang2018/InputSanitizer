namespace InputSanitizer.Demo.WebApi.Dtos
{
    public class TestWriteDto
    {
        public string? TestString { get; set; }

        public List<TestInnerDto>? TestList { get; set;}
    }
}
