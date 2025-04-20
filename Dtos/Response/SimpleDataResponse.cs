namespace KataSimpleAPI.Dtos.Response
{
    public class SimpleDataResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<SimpleDataItem> Items { get; set; } = new List<SimpleDataItem>();
    }
    public class SimpleDataItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
