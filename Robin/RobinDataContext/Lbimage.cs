namespace Robin;

public partial class LBImage
{
	public long ID { get; set; }
	public string Type { get; set; }
	public string FileName { get; set; }
	public string LBRegion { get; set; }
	public long Region_ID { get; set; }
	public long? LBRelease_ID { get; set; }

	public virtual LBRelease LBRelease { get; set; }
}
