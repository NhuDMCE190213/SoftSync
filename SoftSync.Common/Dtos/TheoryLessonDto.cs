namespace SoftSync.Common.Dtos
{
    public class TheoryLessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string ContentMarkdown { get; set; } = "";
        public string? VideoUrl { get; set; }
        public int OrderIndex { get; set; }
    }
}
