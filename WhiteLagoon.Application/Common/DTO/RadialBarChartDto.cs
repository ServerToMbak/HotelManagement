namespace WhiteLagoon.web.VİewModel
{
    public class RadialBarChartDto
    {
        public decimal TotalCount { get; set; }
        public decimal CountCurrentMounth { get; set; }
        public bool HasRatioIncreased { get; set; }
        public int[] Series { get; set; }


    }
}
