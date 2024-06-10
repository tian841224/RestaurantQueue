namespace RestaurantQueue.Dto
{
    public class GetNumberDto
    {
        /// <summary>
        /// 桌號 s = 小 , m = 中 , l = 大
        /// </summary>
        public int phone { get; set; }

        public string Name { get; set; }
    }
}
