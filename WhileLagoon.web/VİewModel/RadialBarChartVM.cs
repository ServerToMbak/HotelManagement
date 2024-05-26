namespace WhiteLagoon.web.VİewModel
{
    public class RadialBarChartVM
    {
        public decimal TotalCount { get; set; }
        public decimal CountCurrentMounth { get; set; }
        public bool HasRatioIncreased { get; set; }
        public int[] Series { get; set; }


    }
}
